using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kexi.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class ThemeConverter : MarkupExtension, IValueConverter
    {
        #region MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? parameter : DependencyProperty.UnsetValue;
        }

        #endregion
    }
}