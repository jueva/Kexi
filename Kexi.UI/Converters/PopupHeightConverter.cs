using System;
using System.Globalization;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    public class PopupHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var contentHeight = (double) value;
            return contentHeight - 30;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
