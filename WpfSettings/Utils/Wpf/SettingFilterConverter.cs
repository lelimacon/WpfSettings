﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WpfSettings.SettingElements;

namespace WpfSettings.Utils.Wpf
{
    internal class SettingFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sections = (IEnumerable<SettingSection>) values[0];
            var filter = values[1] as string;
            if (sections == null || string.IsNullOrEmpty(filter))
                return sections;
            var settingSections = sections.Where(s => s.Visible);
            return settingSections;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new ApplicationException("Convert back should not be used");
        }
    }
}
