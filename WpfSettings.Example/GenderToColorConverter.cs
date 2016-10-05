using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfSettings.Example
{
    public class GenderToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gender = (UserSettings.EGender) value;
            if (gender == UserSettings.EGender.Female)
                return Brushes.PaleVioletRed;
            if (gender == UserSettings.EGender.Male)
                return Brushes.PaleTurquoise;
            return Brushes.PaleGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (Brush) value;
            if (brush.Equals(Brushes.PaleVioletRed))
                return UserSettings.EGender.Female;
            if (brush.Equals(Brushes.PaleTurquoise))
                return UserSettings.EGender.Male;
            return UserSettings.EGender.Other;
        }
    }
}
