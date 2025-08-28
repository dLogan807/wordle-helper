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

        _mainViewModel.Model.Guesses.CollectionChanged += OnViewModelGuessesChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _mainViewModel.Model.Guesses.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.Model.GenerateResults();
    }

    private void OnViewModelGuessesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCanExecuteChanged();
    }
}
