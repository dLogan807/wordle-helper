using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using WordleHelper.Commands;
using WordleHelper.Models;
using WordleHelper.Views;

namespace WordleHelper.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _typedGuess = "";

    public string TypedGuess
    {
        get => _typedGuess;
        set
        {
            _typedGuess = value;
            OnPropertyChanged(nameof(TypedGuess));
        }
    }

    WordManager WordManager { get; set; }

    public ObservableCollection<Guess> Guesses { get; set; }

    public CommandBase ShowWindowCommand { get; set; }

    public CommandBase AddGuessCommand { get; set; }

    public CommandBase RemoveGuessCommand { get; set; }

    public CommandBase CycleLetterCommand { get; set; }

    public MainViewModel()
    {
        Guesses = [];

        ShowWindowCommand = new ShowWindowCommand();

        AddGuessCommand = new AddGuessCommand(this);

        RemoveGuessCommand = new RemoveGuessCommand(this);

        CycleLetterCommand = new CycleLetterCommand();

        WordManager = new WordManager();
    }
}
