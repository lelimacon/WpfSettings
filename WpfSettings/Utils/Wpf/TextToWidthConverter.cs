using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfSettings.Utils.Wpf
{
    internal class TextToWidthConverter : IMultiValueConverter
    {
        [SuppressMessage("ReSharper", "PossibleInvalidCastException")]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string) values[0];
            var family = (FontFamily) values[0];
            var style = (FontStyle) values[0];
            var weight = (FontWeight) values[0];
            var stretch = (FontStretch) values[0];
            var size = (double) values[0];
            int width = MeasureString(text, size, new Typeface(family, style, weight, stretch));
            return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }

        private int MeasureString(string text, double fontSize, Typeface typeface)
        {
            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
            return (int) formattedText.Width;
        }
    }
}
