using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

internal class Letter(char value, LetterCorrectness correctness)
{
    public char Value { get; set; } = value;

    public LetterCorrectness Correctness { get; set; } = correctness;
}
