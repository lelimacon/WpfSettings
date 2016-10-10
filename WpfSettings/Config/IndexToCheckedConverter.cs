using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfSettings.Config
{
    public class IndexToCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string selected = (string)values[0];
            string actual = (string)values[1];
            bool isChecked = selected == actual;
            return isChecked;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used.");
        }
    }
}
