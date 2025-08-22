using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Guess
{
    public Letter[] Letters { get; } = new Letter[5];

    public string GuessString { get; }

    public Guess(string word)
    {
        if (word.Length != 5)
        {
            throw new ArgumentException($"\"{word}\" is not valid. Words must be 5 letters.");
        }

        GuessString = word;

        char[] letters = GuessString.ToCharArray();

        for (int i = 0; i < letters.Length; i++)
        {
            Letters[i] = new Letter(letters[i]);
        }
    }
}
