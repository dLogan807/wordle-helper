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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LetterCorrectness)
            throw new ArgumentException($"value not of type {nameof(LetterCorrectness)}");

        LetterCorrectness letterCorrectness = (LetterCorrectness)value;

        if (letterCorrectness == LetterCorrectness.NotPresent)
            return "Gray";
        else if (letterCorrectness == LetterCorrectness.AdjustPostion)
            return "Yellow";

        return "Green";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string)
            throw new ArgumentException($"value not of type string");

        string letterColor = (string)value;

        if (letterColor.Equals("Gray"))
            return LetterCorrectness.NotPresent;
        else if (letterColor.Equals("Yellow"))
            return LetterCorrectness.AdjustPostion;

        return LetterCorrectness.Correct;
    }
}
