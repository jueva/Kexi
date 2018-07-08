using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class AnyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable;
            return (enumerable != null && enumerable.GetEnumerator().MoveNext()) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
