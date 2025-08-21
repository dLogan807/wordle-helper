using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WordleHelper.Models;
using WordleHelper.ViewModels;

namespace WordleHelper.Commands;

class RemoveGuessCommand : CommandBase
{
    private MainViewModel _mainViewModel;

    public RemoveGuessCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        int index = _mainViewModel.Guesses.IndexOf((Guess)parameter);

        if (index > -1 && index < _mainViewModel.Guesses.Count)
        {
            _mainViewModel.Guesses.RemoveAt(index);
        }
    }
}
