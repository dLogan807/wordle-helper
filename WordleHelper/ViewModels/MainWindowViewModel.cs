using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.Storage;

namespace WordleHelper;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int Counter { get; set; }

    [ObservableProperty]
    partial List<string> WordList { get; set; }

    [ObservableProperty]
    public partial List<string> Guess { get; set; }

    readonly string _filePath;

    public string FilePath
    {
        get => _filePath;
    }

    public MainWindowViewModel()
    {
        _filePath = AppContext.BaseDirectory + @"Assets\words.txt";
        WordList = LoadWords();
        Counter = 0;
        CurrentWord = "";
    }

    [RelayCommand]
    public void Go()
    {
        Counter++;
    }

    private List<string> LoadWords()
    {
        List<string> wordList = [];

        System.Diagnostics.Debug.WriteLine("Loading words from " + _filePath);
        using FileStream fs = new(_filePath, FileMode.Open);
        using BufferedStream bs = new(fs);
        using StreamReader sr = new(bs);

        while (!sr.EndOfStream)
        {
            string? word = sr.ReadLine();
            if (word is null)
            {
                continue;
            }

            wordList.Add(word.Trim());
        }

        return wordList;
    }
}
