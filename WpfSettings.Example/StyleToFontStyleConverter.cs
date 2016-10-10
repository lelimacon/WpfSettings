using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfSettings.Example
{
    public class StyleToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (BoxStyle.TextStyle) value;
            if (gender == BoxStyle.TextStyle.Italic)
                return FontStyles.Italic;
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (FontStyle) value;
            if (brush.Equals(FontStyles.Italic))
                return BoxStyle.TextStyle.Italic;
            return BoxStyle.TextStyle.Normal;
        }
    }
}
