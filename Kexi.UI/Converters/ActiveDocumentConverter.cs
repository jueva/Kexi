using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kexi.UI.Converters
{
    internal class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var layoutDocument = value as LayoutDocument;
            if (layoutDocument != null)
            {
                return layoutDocument;
            }
            ;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var layoutDocument = value as LayoutDocument;
            if (layoutDocument != null)
            {
                return layoutDocument;
            }

            return Binding.DoNothing;
        }
    }
}