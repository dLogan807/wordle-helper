using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Guess : Word
{
    public Letter[] Letters { get; } = new Letter[5];

    public Guess(string word)
        : base(word)
    {
        if (word.Length != 5)
        {
            throw new ArgumentException($"\"{word}\" is not valid. Words must be 5 letters.");
        }

        char[] letters = WordString.ToCharArray();

        for (int i = 0; i < letters.Length; i++)
        {
            Letters[i] = new Letter(letters[i]);
        }
    }
}
