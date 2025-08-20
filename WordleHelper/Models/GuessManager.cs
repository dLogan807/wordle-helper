using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class GuessManager
{
    public ObservableCollection<Guess> Guesses = [];

    public void AddGuess(Guess guess)
    {
        if (this.Guesses.Count > 6)
        {
            throw new InvalidOperationException(
                "List of guesses is full ("
                    + this.Guesses.Count
                    + "). Cannot add guess \""
                    + guess.GuessString
                    + "\"."
            );
        }

        Guesses.Add(guess);
    }

    public void RemoveGuess(string guess)
    {
        if (this.Guesses.Count == 0)
            return;

        foreach (Guess thisGuess in this.Guesses)
        {
            if (thisGuess.GuessString.Equals(guess))
            {
                Guesses.Remove(thisGuess);
                return;
            }
        }
    }
}
