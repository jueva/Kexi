using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Kexi.ViewModel;

namespace Kexi.UI.Converters
{
    public class WindowTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var path      = values[0] as string;
            var workspace = values[1] as Workspace;
            if (path != null && workspace?.Options != null && workspace.Options.AdressbarVisible)
            {
                if (!string.IsNullOrEmpty(workspace.ActiveLister.ProtocolPrefix) && path.StartsWith(workspace.ActiveLister.ProtocolPrefix))
                {
                    var separatorIndex = path.LastIndexOf(@"\") + 1;
                    var item           = separatorIndex == 0 ? "" : path.Substring(separatorIndex);
                    return workspace.ActiveLister.ProtocolPrefix + item;
                }

                if (Directory.Exists(path)) return new DirectoryInfo(path).Name;
            }

            return path ?? workspace?.ActiveLister?.PathName ?? Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}