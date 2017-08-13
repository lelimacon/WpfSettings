using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfSettings.Utils.Wpf
{
    /// <summary>
    ///     One-way converter that returns false if provided object is empty, true otherwise.
    /// </summary>
    internal class NotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value.IsEmpty();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }
    }
}
