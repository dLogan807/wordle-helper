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
        int lastMustIncIndex = -1;

        foreach (Guess guess in guesses)
        {
            Letter[] letters = guess.Letters;

            for (int i = 0; i < letters.Length; i++)
            {
                Letter letter = letters[i];

                //Letter MUST be a index
                if (letter.Correctness == LetterCorrectness.Correct)
                {
                    mustIncludeAtIndex += GetMustIncludeAtIndexRegex(
                        i,
                        letter.Value,
                        lastMustIncIndex
                    );

                    lastMustIncIndex = i;
                }
            }
        }

        return "/" + mustIncludeAtIndex;
    }

    private static string GetMustIncludeAtIndexRegex(int letterIndex, char letter, int lastIndex)
    {
        //Shouldn't {0} always be at the start??????
        string regex = string.Empty;

        int charsFromLastLetter = letterIndex - lastIndex - 1;

        //No previously included letters
        if (lastIndex == -1)
        {
            //Not beginning of word
            if (letterIndex > 0)
            {
                regex = "^.{" + (letterIndex) + "}" + letter;
            }
            else
            {
                regex += letter;
            }
        }
        else
        {
            //If this letter directly follows last must-include letter
            if (charsFromLastLetter == 0)
            {
                regex += letter;
            }
            else
            {
                regex = ".{" + (charsFromLastLetter) + "}" + letter;
            }
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
