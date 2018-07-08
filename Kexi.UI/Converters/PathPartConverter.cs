using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Kexi.Annotations;
using Kexi.Common;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.Converters
{
    public class PathPartConverter : IMultiValueConverter 
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string path)
            {
                if (!(values[1] is FileLister))
                {
                    if (values[1] is ILister lister)
                    {
                        var p = new[]
                        {
                            new PathPart(lister.ProtocolPrefix, path)
                            {
                                First = true,
                                Highlighted = true,
                            },
                            new PathPart(path, path)
                        };
                        return p;
                    }
                    return new[] {new PathPart(path, path)};
                }

                var currentPath = "";
                var parts = new List<PathPart>();
                foreach (var p in path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.IsNullOrEmpty(currentPath) && path.StartsWith(@"\\"))
                        currentPath = @"\\";
                    currentPath += p + "\\";
                    ;
                    var part = new PathPart(p, currentPath);
                    parts.Add(part);
                }
                var first = parts.FirstOrDefault();
                if (first != null)
                {
                    first.First = true;
                    parts.Last().Highlighted = true;
                }
                return parts;
            }
            return new[] { new PathPart("My Computer", null) };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }
    }

  
}
