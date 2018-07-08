using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Kexi.Common;

namespace Kexi.UI.Converters
{
    public class ViewTypeBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewType  = (ViewType) value;
            var viewParam = Enum.Parse(typeof(ViewType), parameter as string);
            if (viewType.Equals(viewParam)) return Application.Current.FindResource("KexSelectionBackground") as Brush;
            return Application.Current.FindResource("ListerBackground") as Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}