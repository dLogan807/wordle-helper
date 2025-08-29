using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class WordRegexBuilder(int wordLength)
{
    public readonly int _wordLength = wordLength;

    public Regex GenerateRegex(ICollection<Guess> guesses)
    {
        string[] correctLetterRegex = new string[_wordLength];
        HashSet<char> correctLetters = [];
        int prevCorrectLetterIndex = -1;

        HashSet<char> adjustPosLetters = [];
        HashSet<char>[] adjustPosLettersIndexBlacklist = new HashSet<char>[_wordLength];

        HashSet<char> incorrectLetters = [];

        //Loop guesses through by column then row
        for (int letterIndex = 0; letterIndex < _wordLength; letterIndex++)
        {
            foreach (Guess guess in guesses)
            {
                Letter letter = guess.Letters[letterIndex];
                char letterValue = char.ToLower(letter.Value);

                if (
                    letter.Correctness == LetterCorrectness.Correct
                    && string.IsNullOrEmpty(correctLetterRegex[letterIndex])
                )
                {
                    correctLetterRegex[letterIndex] += GetCorrectLettersRegex(
                        letterIndex,
                        letterValue,
                        prevCorrectLetterIndex
                    );
                    correctLetters.Add(letterValue);
                    incorrectLetters.Remove(letterValue);

                    prevCorrectLetterIndex = letterIndex;
                }
                else if (
                    letter.Correctness == LetterCorrectness.AdjustPostion
                    && !correctLetters.Contains(letterValue)
                )
                {
                    if (adjustPosLettersIndexBlacklist[letterIndex] == null)
                    {
                        adjustPosLettersIndexBlacklist[letterIndex] = [];
                    }

                    adjustPosLettersIndexBlacklist[letterIndex].Add(letterValue);
                    adjustPosLetters.Add(letterValue);
                }
                else if (
                    letter.Correctness == LetterCorrectness.NotPresent
                    && !correctLetters.Contains(letterValue)
                )
                {
                    incorrectLetters.Add(letterValue);
                }
            }
        }

        string pattern =
            @""
            + GetStringArrayConcat(correctLetterRegex, "^(?=^", ")")
            + GetLettersRegex(incorrectLetters, "(?=^(?:(?![", "]).)*$)", "", "")
            + GetLettersRegex(adjustPosLetters, "", "", "(?=.*", ")")
            + GetIndexBlacklistRegex(adjustPosLettersIndexBlacklist, "(^(?!(", ")).)")
            + ".*$";

        Debug.WriteLine("Generated regex: " + pattern);

        return new Regex(pattern, RegexOptions.Compiled);
    }

    //Add string to each end
    private static string WrapRegex(StringBuilder sb, string start, string end)
    {
        if (sb == null || sb.Length == 0)
            return "";

        sb.Insert(0, start);
        sb.Append(end);

        return sb.ToString();
    }

    //Regex of letters blacklisted from indexes
    private static string GetIndexBlacklistRegex(
        HashSet<char>[] blacklist,
        string start,
        string end
    )
    {
        //Format: (^(?!(^.{index1}[letter blacklist]|^.{index2}[letter blacklist])).)
        if (blacklist == null)
        {
            return "";
        }

        StringBuilder regex = new();

        for (int i = 0; i < blacklist.Length; i++)
        {
            if (blacklist[i] == null || blacklist[i].Count == 0)
            {
                continue;
            }

            if (regex.Length > 0)
            {
                regex.Append('|');
            }

            regex.Append("^.{" + i + "}[");

            foreach (char letter in blacklist[i])
            {
                regex.Append(letter);
            }

            regex.Append(']');
        }

        return WrapRegex(regex, start, end);
    }

    private static string GetLettersRegex(
        HashSet<char> letters,
        string start,
        string end,
        string letterStart,
        string letterEnd
    )
    {
        if (letters.Count == 0)
        {
            return "";
        }

        StringBuilder regex = new();
        foreach (char letter in letters)
        {
            regex.Append(letterStart + letter + letterEnd);
        }

        return WrapRegex(regex, start, end);
    }

    private static string GetStringArrayConcat(string[] strings, string start, string end)
    {
        StringBuilder regex = new();

        foreach (string s in strings)
        {
            regex.Append(s);
        }

        return WrapRegex(regex, start, end);
    }

    //Return regex matching letters in correct positions
    private static string GetCorrectLettersRegex(int letterIndex, char letter, int prevIndex)
    {
        string regex = "";
        int charsFromPrevLetter = letterIndex - prevIndex - 1;

        if (letterIndex == 0 || charsFromPrevLetter == 0)
        {
            regex += letter;
        }
        else if (charsFromPrevLetter == 1)
        {
            regex = "." + letter;
        }
        else
        {
            regex = ".{" + (charsFromPrevLetter) + "}" + letter;
        }

        return regex;
    }
}
