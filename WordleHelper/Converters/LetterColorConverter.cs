using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WordleHelper.Models;

namespace WordleHelper.Converters;

class LetterColorConverter : IValueConverter
{
    private readonly string _gray = "#3a3a3c";
    private readonly string _yellow = "#b59f3b";
    private readonly string _green = "#538d4e";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LetterCorrectness)
            throw new ArgumentException($"value not of type {nameof(LetterCorrectness)}");

        LetterCorrectness letterCorrectness = (LetterCorrectness)value;

        if (letterCorrectness == LetterCorrectness.NotPresent)
            return _gray;
        else if (letterCorrectness == LetterCorrectness.AdjustPostion)
            return _yellow;

        return _green;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string)
            throw new ArgumentException($"value not of type string");

        string letterColor = (string)value;

        if (letterColor.Equals(_gray))
            return LetterCorrectness.NotPresent;
        else if (letterColor.Equals(_yellow))
            return LetterCorrectness.AdjustPostion;

        return LetterCorrectness.Correct;
    }
}
