using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class Word : IEquatable<Word>, IComparable<Guess>
{
    public string WordString { get; }

    private readonly int _hashCode;

    public Word(string word)
    {
        if (word.Length != 5)
        {
            throw new ArgumentException($"\"{word}\" is not valid. Words must be 5 letters.");
        }

        WordString = word;

        _hashCode = WordString.GetHashCode();
    }

    public bool Equals(Word? other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return other.WordString.Equals(WordString);
    }

    public override bool Equals(object? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return obj is Word word && Equals(word);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public int CompareTo(Guess? other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return other.WordString.CompareTo(WordString);
    }
}
