using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleHelper.Models;
using WordleHelper.ViewModels;

namespace WordleHelper.Commands;

class AddGuessCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public AddGuessCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        _mainViewModel.PropertyChanged += OnViewModelPropertyChanged;
        _mainViewModel.Guesses.CollectionChanged += OnViewModelGuessesChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return IsValidGuess() && base.CanExecute(parameter);
    }

    private bool IsValidGuess()
    {
        string guess = _mainViewModel.TypedGuess;

        if (
            string.IsNullOrEmpty(guess)
            || guess.Length != 5
            || _mainViewModel.Guesses.Count > 5
            || !guess.All(char.IsLetter)
        )
        {
            return false;
        }

        //Check word is not already guessed and is in word list
        Word word = new(guess);
        return !_mainViewModel.Guesses.Contains(word)
            && _mainViewModel.WordManager.Words.Contains(word);
    }

    //Add guess to collection and clear field
    public override void Execute(object? parameter)
    {
        Guess guess = new(_mainViewModel.TypedGuess);

        _mainViewModel.Guesses.Add(guess);
        _mainViewModel.TypedGuess = "";

        OnCanExecuteChanged();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mainViewModel.TypedGuess))
        {
            OnCanExecuteChanged();
        }
    }

    private void OnViewModelGuessesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCanExecuteChanged();
    }
}
