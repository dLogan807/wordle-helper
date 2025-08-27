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
        //1. Must contain char at position
        //2. Must contain char somewhere
        //3. Must not contain char
        string mustIncludeAtIndex = string.Empty;
        int lastCorrectLetterIndex = -1;

        foreach (Guess guess in guesses)
        {
            Letter[] letters = guess.Letters;

            for (int i = 0; i < letters.Length; i++)
            {
                Letter letter = letters[i];

                //Letter MUST be a index
                if (letter.Correctness == LetterCorrectness.Correct)
                {
                    mustIncludeAtIndex += GetCorrectLettersRegex(
                        i,
                        letter.Value,
                        lastCorrectLetterIndex
                    );

                    lastCorrectLetterIndex = i;
                }
            }
        }

        return "/" + mustIncludeAtIndex;
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
