using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.Converters
{
    public class UniquePathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var path = values[0] as string;
            if (values[1] is FileLister lister)
                foreach (var l in lister.Workspace.Docking.Files.Where(l => l.Content != lister && l.Content.PathName == lister.PathName && l.Content.Path != lister.Path))
                {
                    var up = Directory.GetParent(l.Content.Path);
                    if (up != null)
                    {
                        var parent  = Path.GetFileName(up.FullName);
                        l.Content.PathName = parent + "/" + l.Content.PathName;
                    }

                    up = Directory.GetParent(lister.Path);
                    if (up != null)
                    {
                        var parent  = Path.GetFileName(up.FullName);
                        return parent + "/" + lister.PathName;
                    }
                }

            return path;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }
    }
}