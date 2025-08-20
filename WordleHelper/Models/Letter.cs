using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Letter
{
    public char Value { get; set; }

    public LetterCorrectness Correctness { get; set; }

    public Letter(char value)
    {
        ValidateIsAlphabetical(value);

        Value = char.ToUpper(value);
        Correctness = LetterCorrectness.Wrong;
    }

    public Letter(char value, LetterCorrectness correctness)
    {
        ValidateIsAlphabetical(value);

        Value = value;
        Correctness = correctness;
    }

    private static void ValidateIsAlphabetical(char letter)
    {
        if (!char.IsLetter(letter))
        {
            throw new ArgumentException(
                letter + " is not a letter. Acceptable chars include a-z and A-Z."
            );
        }
    }

    public LetterCorrectness CycleLetterCorrectness()
    {
        switch (this.Correctness)
        {
            case LetterCorrectness.Wrong:
                this.Correctness = LetterCorrectness.AdjustPostion;
                break;
            case LetterCorrectness.AdjustPostion:
                this.Correctness = LetterCorrectness.Correct;
                break;
            case LetterCorrectness.Correct:
                this.Correctness = LetterCorrectness.Wrong;
                break;
        }

        return this.Correctness;
    }
}
