using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kexi.Common
{
    public static class Utils
    {

        public static BitmapImage GetImageFromRessource(string file)
        {
            return new BitmapImage(GetMediaUrl(file));
        }
        public static Uri GetMediaUrl(string file)
        {
            return new Uri($"pack://application:,,,/Kexi.UI;Component/Media/{file}");
        }

        public static T FindParent<T>(DependencyObject current) where T : class
        {
            if (!(current is Visual))
                return null;
            var parent = VisualTreeHelper.GetParent(current);

            if (parent == null) return null;
            if (parent is T variable) return variable;
            return FindParent<T>(parent);
        }

        public static object GetDataContextFromOriginalSource(object o)
        {
            object dataContext = null;
            switch (o)
            {
                case FrameworkElement frameworkElement:
                    dataContext = frameworkElement.DataContext;
                    break;
                case FrameworkContentElement frameworkContentElement:
                    dataContext = frameworkContentElement.DataContext;
                    break;
            }

            ;
            return dataContext;
        }

        public static bool HitsTextBlock(MouseButtonEventArgs e, ListBox listView)
        {
            var dpi = VisualTreeHelper.GetDpi(listView);
            var result = VisualTreeHelper.HitTest(listView, e.GetPosition(listView));
            if (result == null)
                return false;

            if (result.VisualHit is Run run)
            {
                var x = e.GetPosition(run).X;
                if (x < GetBlockSize(run, dpi).Width)
                    return true;
            }

            if (result.VisualHit is TextBlock textblock)
            {
                var x = e.GetPosition(textblock).X;
                if (x < GetBlockSize(textblock, dpi).Width)
                    return true;
            }

            return false;
        }

        private static Size GetBlockSize(Run block, DpiScale dpi)
        {
            var formattedText = new FormattedText(
                block.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(block.FontFamily, block.FontStyle, block.FontWeight, block.FontStretch),
                block.FontSize,
                Brushes.Black, dpi.PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private static Size GetBlockSize(TextBlock block, DpiScale dpi)
        {
            var formattedText = new FormattedText(
                block.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(block.FontFamily, block.FontStyle, block.FontWeight, block.FontStretch),
                block.FontSize,
                Brushes.Black, dpi.PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

    }
}