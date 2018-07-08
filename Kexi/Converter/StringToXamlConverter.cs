using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Kexi.ViewModel;

namespace Kexi.Converter
{
    public class StringToXamlConverter : IMultiValueConverter
    {
        private static readonly Brush HighlightBrush = (SolidColorBrush)Application.Current.FindResource("ItemHighlightColor");

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var input = values[0] as string;
            var workspace = values[1] as Workspace;
            var filter = workspace?.ActiveLister.HighlightString;
            var textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.NoWrap
            };

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(filter) || (!workspace?.Options.Highlights ?? false))
            {
                textBlock.Text = input;
                return textBlock;
            }
            
            var parts = Regex.Split(input, filter, RegexOptions.IgnoreCase);
            if (parts.Length == 0)
            {
                textBlock.Text = input;
                return textBlock;
            }

            var currentIndex = 0;
            for(var i = 0;i< parts.Length;i++)
            {
                var part = parts[i];
                if (!string.IsNullOrWhiteSpace(part))
                {
                    textBlock.Inlines.Add(new Run(part));
                }
                currentIndex += part.Length;
                if (i < parts.Length - 1)
                {
                    textBlock.Inlines.Add(new Run(input.Substring(currentIndex, filter.Length))
                    {
                        FontWeight = FontWeights.Normal,
                        Background = HighlightBrush
                    });
                }
            }
            return textBlock;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
