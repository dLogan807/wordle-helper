using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WordleHelper.ViewModels;

namespace WordleHelper.Models;

public class Model
{
    public readonly int _wordLength;
    public readonly int _maxGuesses;

    public readonly HashSet<Word> Words;
    public ObservableCollection<Guess> Guesses { get; set; }
    public ObservableCollection<Word> Results { get; set; }

    public Model(string wordAssetPath, int wordLength, int maxGuesses)
    {
        Guesses = [];
        Results = [];
        _wordLength = wordLength;
        _maxGuesses = maxGuesses;

        Words = LoadWords(wordAssetPath);
    }

    private HashSet<Word> LoadWords(string wordAssetPath)
    {
        Debug.WriteLine($"Loading words from {wordAssetPath}");

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream =
            assembly.GetManifestResourceStream(wordAssetPath)
            ?? throw new FileNotFoundException(
                $"Could not find word list resource at {wordAssetPath}"
            );
        using BufferedStream bs = new(stream);
        using StreamReader sr = new(bs);

        HashSet<Word> wordList = [];

        while (!sr.EndOfStream)
        {
            string? word = sr.ReadLine();

            if (string.IsNullOrEmpty(word) || word.Length != _wordLength)
                continue;

            wordList.Add(new Word(word.Trim()));
        }

        Debug.WriteLine($"Loaded {wordList.Count} words!");

        return wordList;
    }

    public bool IsValidGuess(string guess)
    {
        if (string.IsNullOrEmpty(guess))
            return false;

        guess = guess.ToLower();

        if (guess.Length != _wordLength || Guesses.Count > _maxGuesses || !guess.All(char.IsLetter))
        {
            return false;
        }

        return IsExistingAndNotGuessed(guess);
    }

    private bool IsExistingAndNotGuessed(string guess)
    {
        Word word = new(guess);

        return !Guesses.Contains(word) && Words.Contains(word);
    }

    public void GenerateResults()
    {
        Results.Clear();
        Regex regex = GenerateRegex(Guesses);

        foreach (Word word in Words)
        {
            if (regex.IsMatch(word.WordString))
            {
                Results.Add(word);
            }
        }
    }

    private Regex GenerateRegex(ObservableCollection<Guess> guesses)
    {
        string[] correctLetterRegex = new string[_wordLength];
        HashSet<char> correctLetters = [];
        int prevCorrectLetterIndex = -1;

        HashSet<char> adjustPosLetters = [];
        HashSet<char>[] adjustPosLettersIndexBlacklist = new HashSet<char>[_wordLength];

        HashSet<char> incorrectLetters = [];

        for (int letterIndex = 0; letterIndex < _wordLength; letterIndex++)
        {
            foreach (Guess guess in guesses)
            {
                Letter letter = guess.Letters[letterIndex];
                char letterValue = char.ToLower(letter.Value);

                if (
                    letter.Correctness == LetterCorrectness.Correct
                    && string.IsNullOrEmpty(correctLetterRegex[letterIndex])
                )
                {
                    correctLetterRegex[letterIndex] += GetCorrectLettersRegex(
                        letterIndex,
                        letterValue,
                        prevCorrectLetterIndex
                    );
                    correctLetters.Add(letterValue);
                    incorrectLetters.Remove(letterValue);

                    prevCorrectLetterIndex = letterIndex;
                }
                else if (
                    letter.Correctness == LetterCorrectness.AdjustPostion
                    && !correctLetters.Contains(letterValue)
                )
                {
                    if (adjustPosLettersIndexBlacklist[letterIndex] == null)
                    {
                        adjustPosLettersIndexBlacklist[letterIndex] = [];
                    }

                    adjustPosLettersIndexBlacklist[letterIndex].Add(letterValue);
                    adjustPosLetters.Add(letterValue);
                }
                else if (
                    letter.Correctness == LetterCorrectness.NotPresent
                    && !correctLetters.Contains(letterValue)
                )
                {
                    incorrectLetters.Add(letterValue);
                }
            }
        }

        string pattern =
            @""
            + GetStringArrayConcat(correctLetterRegex, "^(?=^", ")")
            + GetLettersRegex(
                incorrectLetters,
                "(?=^(?:(?![",
                "]).)*$)",
                string.Empty,
                string.Empty
            )
            + GetLettersRegex(adjustPosLetters, string.Empty, string.Empty, "(?=.*", ")")
            + GetIndexBlacklistRegex(adjustPosLettersIndexBlacklist, "(^(?!(", ")).)")
            + ".*$";

        Debug.WriteLine("Generated regex: " + pattern);

        return new Regex(pattern, RegexOptions.Compiled);
    }

    private static string WrapRegex(StringBuilder sb, string start, string end)
    {
        if (sb == null || sb.Length == 0)
            return string.Empty;

        sb.Insert(0, start);
        sb.Append(end);

        return sb.ToString();
    }

    //Regex of letters blacklisted from indexes
    private static string GetIndexBlacklistRegex(
        HashSet<char>[] blacklist,
        string start,
        string end
    )
    {
        //Format: (^(?!(^.{index1}[letter blacklist]|^.{index2}[letter blacklist])).)
        if (blacklist == null)
        {
            return string.Empty;
        }

        StringBuilder regex = new();

        for (int i = 0; i < blacklist.Length; i++)
        {
            if (blacklist[i] == null || blacklist[i].Count == 0)
            {
                continue;
            }

            if (regex.Length > 0)
            {
                regex.Append('|');
            }

            regex.Append("^.{" + i + "}[");

            foreach (char letter in blacklist[i])
            {
                regex.Append(letter);
            }

            regex.Append(']');
        }

        return WrapRegex(regex, start, end);
    }

    private static string GetLettersRegex(
        HashSet<char> letters,
        string start,
        string end,
        string letterStart,
        string letterEnd
    )
    {
        if (letters.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder regex = new();
        foreach (char letter in letters)
        {
            regex.Append(letterStart + letter + letterEnd);
        }

        return WrapRegex(regex, start, end);
    }

    private static string GetStringArrayConcat(string[] strings, string start, string end)
    {
        StringBuilder regex = new();

        foreach (string s in strings)
        {
            regex.Append(s);
        }

        return WrapRegex(regex, start, end);
    }

    //Return regex matching letters in correct positions
    private static string GetCorrectLettersRegex(int letterIndex, char letter, int prevIndex)
    {
        string regex = string.Empty;
        int charsFromPrevLetter = letterIndex - prevIndex - 1;

        if (letterIndex == 0 || charsFromPrevLetter == 0)
        {
            regex += letter;
        }
        else if (charsFromPrevLetter == 1)
        {
            regex = "." + letter;
        }
        else
        {
            regex = ".{" + (charsFromPrevLetter) + "}" + letter;
        }

        return regex;
    }
}
