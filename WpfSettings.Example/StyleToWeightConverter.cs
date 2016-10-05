using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfSettings.Example
{
    public class StyleToWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (StyleSettings.TextStyle)value;
            if (gender == StyleSettings.TextStyle.Bold)
                return FontWeights.Bold;
            return FontWeights.Regular;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (FontWeight)value;
            if (brush.Equals(FontWeights.Bold))
                return StyleSettings.TextStyle.Italic;
            return StyleSettings.TextStyle.Normal;
        }
    }
}
