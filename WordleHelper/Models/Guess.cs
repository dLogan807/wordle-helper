using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

internal class Guess
{
    readonly Letter[] _letters = new Letter[5];

    public void SetLetter(Letter letter, int position)
    {
        _letters[position] = letter;
    }
}
