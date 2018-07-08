using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class FullPathToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (path != null)
            {
                return Path.GetFileName(path);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
