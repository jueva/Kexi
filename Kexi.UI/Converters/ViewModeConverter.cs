using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Kexi.Common;
using Kexi.UI.View;
using Kexi.View;

namespace Kexi.UI.Converters
{
    public class ViewModeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ViewType type)
            {
                var view = values[1] as KexiListView;
                switch (type)
                {
                    case ViewType.Detail:
                        return view.FindResource("ListViewItemDetailStyle") as Style;
                    case ViewType.Icon:
                    case ViewType.Thumbnail:
                        return view.FindResource("ListViewItemThumbStyle") as Style;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(values));
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }
    }
}