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

        _mainViewModel.PropertyChanged += OnViewModelPropertyChanged;
        _mainViewModel.Guesses.CollectionChanged += OnViewModelGuessesChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _mainViewModel.Guesses.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        string matchExpression = GenerateRegex(_mainViewModel.Guesses);
        Debug.WriteLine(matchExpression);
    }

    private static string GenerateRegex(ObservableCollection<Guess> guesses)
    {
        string[] correctLetterRegex = new string[5];
        int lastCorrectLetterIndex = -1;

        string wrongPosLetterRegex = string.Empty;

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
                        letter.Value,
                        lastCorrectLetterIndex
                    );

                    lastCorrectLetterIndex = letterIndex;
                }
                else if (letter.Correctness == LetterCorrectness.AdjustPostion) { }
                else if (letter.Correctness == LetterCorrectness.NotPresent)
                {
                    incorrectLetters.Add(letter.Value);
                }
            }
        }

        return "/"
            + GetWrongLettersRegex(incorrectLetters)
            + GetStringArrayConcat(correctLetterRegex);
    }

    private static string GetWrongLettersRegex(HashSet<char> letters)
    {
        if (letters.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new("(?!");
        foreach (char letter in letters)
        {
            if (sb.Length == 3)
            {
                sb.Append(letter);
            }
            else
            {
                sb.Append("|" + letter);
            }
        }
        sb.Append(')');

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

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mainViewModel.Guesses.Count))
        {
            OnCanExecuteChanged();
        }
    }

    private void OnViewModelGuessesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCanExecuteChanged();
    }
}
