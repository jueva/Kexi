using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class TransparencyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var transparent = false;
            if (values[0] is bool t)
                transparent = t;

            return transparent ? Brushes.Transparent : Application.Current.FindResource("ListerBackground");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}