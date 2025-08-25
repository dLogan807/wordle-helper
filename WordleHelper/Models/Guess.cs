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
        for (int i = 0; i < word.Length; i++)
        {
            Letters[i] = new(word[i]);
        }
    }
}
