using System;
using System.Globalization;
using System.Windows.Data;

namespace Kexi.Converter
{
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "NAN";
            var length = (long) value;
            if (length > 1048576)
                return $"{length / 1048576:#####.##} MB"; 
            if (length > 1073741824)
                return $"{length / 1073741824:#####.##} GB";
            return (length / 1024) + " KB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
