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

    private readonly WordRegexBuilder _wordRegexBuilder;

    public Model(string wordAssetPath, int wordLength, int maxGuesses)
    {
        _wordLength = wordLength;
        _maxGuesses = maxGuesses;
        Guesses = [];
        Results = [];
        _wordRegexBuilder = new(_wordLength);

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
        Regex regex = _wordRegexBuilder.GenerateRegex(Guesses);

        foreach (Word word in Words)
        {
            if (regex.IsMatch(word.WordString))
            {
                Results.Add(word);
            }
        }
    }
}
