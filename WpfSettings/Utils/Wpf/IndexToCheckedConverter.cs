using System;
using System.Globalization;
using System.Windows.Data;
using WpfSettings.SettingElements;

namespace WpfSettings.Utils.Wpf
{
    internal class IndexToCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SettingField selected = (SettingField) values[0];
            SettingField actual = (SettingField) values[1];
            bool isChecked = selected.Value.Equals(actual.Value);
            return isChecked;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used.");
        }
    }
}
