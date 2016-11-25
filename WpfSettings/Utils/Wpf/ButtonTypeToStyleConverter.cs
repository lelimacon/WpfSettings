using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WpfSettings.SettingElements;

namespace WpfSettings.Utils.Wpf
{
    internal class ButtonTypeToStyleConverter : IValueConverter
    {
        public Style FlatStyle { get; set; }
        public Style LinkStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (ButtonType) value;
            switch (type)
            {
                case ButtonType.Flat:
                    return FlatStyle;
                case ButtonType.Link:
                    return LinkStyle;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }
    }
}
