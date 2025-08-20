using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Guess
{
    readonly Letter[] Letters = new Letter[5];
    public readonly string GuessString;

    public Guess(string word)
    {
        if (word.Length != 5)
        {
            throw new ArgumentException(word + " is not valid. Words must be 5 letters.");
        }

        this.GuessString = word;

        char[] letters = this.GuessString.ToCharArray();

        for (int i = 0; i < letters.Length; i++)
        {
            this.Letters[i] = new Letter(letters[i]);
        }
    }
}
