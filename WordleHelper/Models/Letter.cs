using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Letter
{
    public char Value { get; }

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
        switch (Correctness)
        {
            case LetterCorrectness.Wrong:
                Correctness = LetterCorrectness.AdjustPostion;
                break;
            case LetterCorrectness.AdjustPostion:
                Correctness = LetterCorrectness.Correct;
                break;
            case LetterCorrectness.Correct:
                Correctness = LetterCorrectness.Wrong;
                break;
        }

        return Correctness;
    }
}
