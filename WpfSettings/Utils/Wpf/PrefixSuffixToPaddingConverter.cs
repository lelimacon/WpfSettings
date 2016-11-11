using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfSettings.Utils.Wpf
{
    internal class PrefixSuffixToPaddingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var prefix = (string) values[0];
            var suffix = (string) values[1];
            var family = (FontFamily) values[2];
            var style = (FontStyle) values[3];
            var weight = (FontWeight) values[4];
            var stretch = (FontStretch) values[5];
            var size = (double) values[6];
            double prefixWidth = MeasureString(prefix, size, new Typeface(family, style, weight, stretch));
            double suffixWidth = MeasureString(suffix, size, new Typeface(family, style, weight, stretch));
            Thickness thickness = new Thickness(prefixWidth, 0, suffixWidth, 0);
            return thickness;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }

        private double MeasureString(string text, double fontSize, Typeface typeface)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
            return formattedText.Width + 8;
        }
    }
}
