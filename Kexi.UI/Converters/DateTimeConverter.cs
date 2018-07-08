using System;
using System.Globalization;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class DateTimeConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime?) value;
            return date.HasValue ? date.Value.ToShortDateString() + " " + date.Value.ToLongTimeString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
