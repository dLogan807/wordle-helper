# Wordle Helper

A WPF (Windows Presentation Foundation) .NET 8 app for offering solutions to the day's Wordle based on what you've already guessed.

Useful if you simply don't know what the word could be!

<img width="436" height="544" alt="image" src="https://github.com/user-attachments/assets/8a7f8a48-8e1d-4b59-bfa2-f8cf2c59a875" />

## Requirements
- Windows 10 or 11
- .NET 8.0 Desktop Runtime

## How to use

1. Download and install the [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Download and run the latest release of this app
3. Enter the guesses you've made
4. Click letters to cycle them to the correct colour
5. Press `Get Words`
6. Done!

## How it works

The app generates a regex pattern from the guesses you have entered. It then compares that pattern with each valid Wordle guess and lists every possible word.
Architecturally, the app uses the MVVM pattern.

## Caveats
The app does not account for words that:
- Have already been used as the Wordle of the day
- Are valid guesses, but cannot appear as Wordles
