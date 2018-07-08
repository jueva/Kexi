using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Kexi.Common;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.Converters
{
    public class SubDirectoriesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var cont = Application.Current.MainWindow.DataContext as Workspace;
                if (!(cont?.ActiveLister is FileLister))
                {
                    return Visibility.Collapsed;
                }

                var pathpart = value as PathPart;
                var path = pathpart?.Path;
                var visibility = cont.Options.ShowPathPartDividers && path != null && !pathpart.Highlighted;

                return  visibility ? Visibility.Visible: Visibility.Collapsed;
            }
            catch (Exception) //Unauthorized & Co
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}