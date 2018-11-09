using System;
using System.Globalization;
using System.Windows.Data;
using Kexi.Common;
using Kexi.View;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.Converters
{
    public class ViewTypeConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ViewType type)
            {
                var lister      = values[1] as ILister;
                var viewHandler = new ViewHandler(lister);
                switch (type)
                {
                    case ViewType.Detail:
                        return viewHandler.GetDetailView();
                    case ViewType.Icon:
                        return viewHandler.GetIconView();
                    case ViewType.Thumbnail:
                        return viewHandler.GetThumbView();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(values), "Unknown View Type");
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }

    public class ViewItemStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

}
