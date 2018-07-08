using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class RibbonMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ribbonVisible = (bool) value;
            return ribbonVisible
                ? new Thickness(0, 0, 0, 0)
                : new Thickness(0, -0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
