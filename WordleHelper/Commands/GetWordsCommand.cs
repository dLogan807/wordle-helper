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
        HashSet<char>[] wrongPosLettersIndexBlacklist = new HashSet<char>[5];

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

    private void OnViewModelGuessesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCanExecuteChanged();
    }
}
