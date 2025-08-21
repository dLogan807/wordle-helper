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

class RemoveGuessCommand(MainViewModel mainViewModel) : CommandBase
{
    private readonly MainViewModel _mainViewModel = mainViewModel;

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        Guess guess = (Guess)parameter;

        _mainViewModel.Guesses.Remove(guess);
    }
}
