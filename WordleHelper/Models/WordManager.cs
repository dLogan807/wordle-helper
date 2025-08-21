using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WordleHelper.Models;

internal class WordManager
{
    private readonly List<string> _words;

    public WordManager()
    {
        _words = LoadWords();
    }

    private static List<string> LoadWords()
    {
        string filePath = "WordleHelper.Assets.words.txt";
        Debug.WriteLine($"Loading words from {filePath}.");

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream =
            assembly.GetManifestResourceStream(filePath)
            ?? throw new FileNotFoundException($"Could not find file at {filePath}");
        using BufferedStream bs = new(stream);
        using StreamReader sr = new(bs);

        List<string> wordList = [];

        while (!sr.EndOfStream)
        {
            string? word = sr.ReadLine();
            if (string.IsNullOrEmpty(word) || word.Length > 5)
            {
                continue;
            }

            wordList.Add(word.Trim());
        }

        Debug.WriteLine($"Loaded {wordList.Count} words!");

        return wordList;
    }
}
