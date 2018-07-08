using System;
using System.Globalization;
using System.Windows.Data;
using Kexi.ViewModel;

namespace Kexi.UI.Converters
{
    public class NameToCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var commandName = parameter as string;
            if (value is Workspace workspace && commandName!= null) return workspace.CommandRepository.GetCommandByName(commandName);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}