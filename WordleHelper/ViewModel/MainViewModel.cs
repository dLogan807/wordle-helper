using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using WordleHelper.Models;
using WordleHelper.Views;

namespace WordleHelper.ViewModel;

public class MainViewModel
{
    public string? TypedGuess { get; set; } = "";

    public ObservableCollection<Guess> Guesses { get; set; }

    public ICommand ShowWindowCommand { get; set; }

    public ICommand AddGuessCommand { get; set; }

    public MainViewModel()
    {
        Guesses = GuessManager.Guesses;

        ShowWindowCommand = new RelayCommand(ShowWindow, CanShowWindow);

        AddGuessCommand = new RelayCommand(AddGuess, CanAddGuess);
    }

    private bool CanAddGuess()
    {
        return true;
        //return TypedGuess?.Length == 5 && Guesses.Count < 7;
    }

    private void AddGuess()
    {
        Guesses.Add(new Guess(TypedGuess));
        Trace.WriteLine("Guess: " + TypedGuess);
    }

    private bool CanShowWindow()
    {
        return true;
    }

    private void ShowWindow()
    {
        Trace.WriteLine("Showing other view");

        PreviousDays previousDays = new();
        previousDays.Show();
    }
}
