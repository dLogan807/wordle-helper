using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class GuessManager
{
    public static ObservableCollection<Guess> Guesses { get; } = [];

    public void AddGuess(Guess guess)
    {
        if (Guesses.Count > 6)
        {
            throw new InvalidOperationException(
                "List of guesses is full ("
                    + Guesses.Count
                    + " guesses). Cannot add guess \""
                    + guess.GuessString
                    + "\"."
            );
        }

        Guesses.Add(guess);
    }

    public void RemoveGuess(string guess)
    {
        if (Guesses.Count == 0)
            return;

        foreach (Guess thisGuess in Guesses)
        {
            if (thisGuess.GuessString.Equals(guess))
            {
                Guesses.Remove(thisGuess);
                return;
            }
        }
    }
}
