using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class ElementCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var elements = value as IList;
            return elements?.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}