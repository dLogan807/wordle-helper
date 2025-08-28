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
        int lastCorrectLetterIndex = -1;

        HashSet<char> wrongPosLetters = [];
        HashSet<char>[] wrongPosLettersIndexBlacklist = new HashSet<char>[_wordLength];

        HashSet<char> incorrectLetters = [];

        for (int letterIndex = 0; letterIndex < guesses[0].Letters.Length; letterIndex++)
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
                        lastCorrectLetterIndex
                    );
                    correctLetters.Add(letterValue);

                    lastCorrectLetterIndex = letterIndex;

                    incorrectLetters.Remove(letterValue);
                }
                else if (
                    letter.Correctness == LetterCorrectness.AdjustPostion
                    && !correctLetters.Contains(letterValue)
                )
                {
                    if (wrongPosLettersIndexBlacklist[letterIndex] == null)
                    {
                        wrongPosLettersIndexBlacklist[letterIndex] = [];
                    }

                    wrongPosLettersIndexBlacklist[letterIndex].Add(letterValue);
                    wrongPosLetters.Add(letterValue);
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

        string posBlacklistPattern =
            wrongPosLetters.Count > 0
                ? GetIndexBlacklistRegex(wrongPosLettersIndexBlacklist)
                : string.Empty;
        string pattern =
            @"^(?=^"
            + GetStringArrayConcat(correctLetterRegex)
            + ")"
            + GetLettersRegex(
                "(?=^(?:(?![",
                "]).)*$)",
                string.Empty,
                string.Empty,
                incorrectLetters
            )
            + GetLettersRegex(string.Empty, string.Empty, "(?=.*", ")", wrongPosLetters)
            + posBlacklistPattern
            + ".*$";

        Debug.WriteLine("Generated regex: " + pattern);

        return new Regex(pattern, RegexOptions.Compiled);
    }

    private static string GetIndexBlacklistRegex(HashSet<char>[] blacklist)
    {
        //(^(?!(^.{index1}[letter blacklist]|^.{index2}[letter blacklist])).)
        if (blacklist == null)
        {
            return string.Empty;
        }

        StringBuilder regex = new();

        regex.Append("(^(?!(");
        int baseLength = regex.Length;

        for (int i = 0; i < blacklist.Length; i++)
        {
            if (blacklist[i] == null || blacklist[i].Count == 0)
            {
                continue;
            }

            if (regex.Length > baseLength)
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

        regex.Append(")).)");

        return regex.ToString();
    }

    private static string GetLettersRegex(
        string start,
        string end,
        string letterStart,
        string letterEnd,
        HashSet<char> letters
    )
    {
        if (letters.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder regex = new(start);
        foreach (char letter in letters)
        {
            regex.Append(letterStart + letter + letterEnd);
        }
        regex.Append(end);

        return regex.ToString();
    }

    private static string GetStringArrayConcat(string[] strings)
    {
        StringBuilder regex = new();

        foreach (string s in strings)
        {
            regex.Append(s);
        }

        return regex.ToString();
    }

    //Return regex matching letters in correct positions
    private static string GetCorrectLettersRegex(int letterIndex, char letter, int lastIndex)
    {
        string regex = string.Empty;
        int charsFromLastLetter = letterIndex - lastIndex - 1;

        if (letterIndex == 0 || charsFromLastLetter == 0)
        {
            regex += letter;
        }
        else if (charsFromLastLetter == 1)
        {
            regex = "." + letter;
        }
        else
        {
            regex = ".{" + (charsFromLastLetter) + "}" + letter;
        }

        return regex;
    }
}
