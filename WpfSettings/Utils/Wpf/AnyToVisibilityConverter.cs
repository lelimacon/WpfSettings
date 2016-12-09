using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfSettings.Utils.Wpf
{
    /// <summary>
    ///     One-way converter that returns Collapsed if all provided objects are empty, Visible otherwise.
    /// </summary>
    internal class AnyToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(value => !value.IsEmpty()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }
    }
}
