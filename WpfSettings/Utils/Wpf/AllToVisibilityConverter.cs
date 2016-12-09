using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfSettings.Utils.Wpf
{
    /// <summary>
    ///     One-way converter that returns Collapsed if at least one of provided objects is empty, Visible otherwise.
    /// </summary>
    internal class AllToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(value => value.IsEmpty()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }
    }
}
