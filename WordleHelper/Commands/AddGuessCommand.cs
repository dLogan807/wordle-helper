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
        if (
            string.IsNullOrEmpty(_mainViewModel.TypedGuess)
            || _mainViewModel.TypedGuess.Length != 5
            || _mainViewModel.Guesses.Count > 5
        )
            return false;

        char[] guessChars = _mainViewModel.TypedGuess.ToCharArray();

        foreach (char c in guessChars)
        {
            if (!char.IsLetter(c))
                return false;
        }

        return true;
    }

    //Add guess to collection and clear field
    public override void Execute(object? parameter)
    {
        Guess guess = new(_mainViewModel.TypedGuess);
        _mainViewModel.Guesses.Insert(0, guess);
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
