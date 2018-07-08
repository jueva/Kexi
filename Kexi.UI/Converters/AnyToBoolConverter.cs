using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class AnyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable;
            return (enumerable != null && enumerable.GetEnumerator().MoveNext());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
