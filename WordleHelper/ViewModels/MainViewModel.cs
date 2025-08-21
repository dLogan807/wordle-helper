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

    public ICommand ShowWindowCommand { get; set; }

    public ICommand AddGuessCommand { get; set; }

    public ICommand RemoveGuessCommand { get; set; }

    public MainViewModel()
    {
        Guesses = [];

        ShowWindowCommand = new ShowWindowCommand();

        AddGuessCommand = new AddGuessCommand(this);

        RemoveGuessCommand = new RemoveGuessCommand(this);

        WordManager = new WordManager();
    }
}
