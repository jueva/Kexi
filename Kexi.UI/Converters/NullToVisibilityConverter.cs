﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility) value) == Visibility.Visible ? (int?)1 : null;
        }
    }
}
