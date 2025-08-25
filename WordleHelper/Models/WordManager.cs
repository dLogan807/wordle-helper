using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

public class WordManager
{
    public readonly HashSet<Word> Words;

    public WordManager()
    {
        Words = LoadWords();
    }

    private static HashSet<Word> LoadWords()
    {
        string filePath = "WordleHelper.Assets.words.txt";
        Debug.WriteLine($"Loading words from {filePath}");

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream =
            assembly.GetManifestResourceStream(filePath)
            ?? throw new FileNotFoundException($"Could not find word list resource at {filePath}");
        using BufferedStream bs = new(stream);
        using StreamReader sr = new(bs);

        HashSet<Word> wordList = [];

        while (!sr.EndOfStream)
        {
            string? word = sr.ReadLine();
            if (string.IsNullOrEmpty(word) || word.Length > 5)
                continue;

            wordList.Add(new Word(word.Trim()));
        }

        Debug.WriteLine($"Loaded {wordList.Count} words!");

        return wordList;
    }
}
