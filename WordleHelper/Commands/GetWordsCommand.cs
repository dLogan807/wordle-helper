using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WordleHelper.Models;
using WordleHelper.ViewModels;

namespace WordleHelper.Commands;

class GetWordsCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public GetWordsCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        _mainViewModel.Guesses.CollectionChanged += OnViewModelGuessesChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _mainViewModel.Guesses.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        Regex regex = GenerateRegex(_mainViewModel.Guesses);
        _mainViewModel.Results.Clear();

        foreach (Word word in _mainViewModel.WordManager.Words)
        {
            if (regex.IsMatch(word.WordString))
            {
                _mainViewModel.Results.Add(word);
            }
        }
    }

    private static Regex GenerateRegex(ObservableCollection<Guess> guesses)
    {
        string[] correctLetterRegex = new string[5];
        HashSet<char> correctLetters = [];
        int lastCorrectLetterIndex = -1;

        HashSet<char> wrongPosLetters = [];

        HashSet<char> incorrectLetters = [];

        for (int letterIndex = 0; letterIndex < guesses[0].Letters.Length; letterIndex++)
        {
            foreach (Guess guess in guesses)
            {
                Letter letter = guess.Letters[letterIndex];

                if (
                    letter.Correctness == LetterCorrectness.Correct
                    && string.IsNullOrEmpty(correctLetterRegex[letterIndex])
                )
                {
                    correctLetterRegex[letterIndex] += GetCorrectLettersRegex(
                        letterIndex,
                        char.ToLower(letter.Value),
                        lastCorrectLetterIndex
                    );
                    correctLetters.Add(letter.Value);

                    lastCorrectLetterIndex = letterIndex;
                }
                else if (
                    letter.Correctness == LetterCorrectness.AdjustPostion
                    && !correctLetters.Contains(letter.Value)
                )
                {
                    wrongPosLetters.Add(char.ToLower(letter.Value));
                }
                else if (letter.Correctness == LetterCorrectness.NotPresent)
                {
                    incorrectLetters.Add(char.ToLower(letter.Value));
                }
            }
        }

        //^(?=^h.{2}l)(?=^(?:(?![456]).)*$).*$
        //^(?=^h.{2}l)(?=^(?:(?![abc]).)*$)(?=.*f)(?=.*z).*$
        //Add (^(?!a).{0}) to check char is not at index
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
            + ".*$";

        Debug.WriteLine("Generated regex: " + pattern);

        return new Regex(pattern, RegexOptions.Compiled);
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

        StringBuilder sb = new(start);
        foreach (char letter in letters)
        {
            sb.Append(letterStart + letter + letterEnd);
        }
        sb.Append(end);

        return sb.ToString();
    }

    private static string GetStringArrayConcat(string[] strings)
    {
        StringBuilder sb = new();

        foreach (string s in strings)
        {
            sb.Append(s);
        }

        return sb.ToString();
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

    private void OnViewModelGuessesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCanExecuteChanged();
    }
}
