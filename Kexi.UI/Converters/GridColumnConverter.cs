using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Kexi.UI.Converters
{
    class GridColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var columns = value as IEnumerable<Column>;
            if (columns == null)
                return null;

            var cols = new GridViewColumnCollection();
            foreach (var col in columns)
            {
                var gc = new GridViewColumn();

                gc.Header = col.Header;
                if (col.Type == ColumnType.Number || col.Type == ColumnType.RightAligned)
                {
                    gc.CellTemplate = getRightAlignDataTemplate(col.BindingExpression);
                }
                else
                {
                    var binding = new Binding(col.BindingExpression);
                    if (col.OneTimeBinding)
                        binding.Mode = BindingMode.OneTime;
                    gc.DisplayMemberBinding = binding;
                }

                if (col.Size == ColumnSize.FullWidth)
                {
                    //TODO: Hack
                    var width = Application.Current.MainWindow.Width - 60;
                    gc.Width = width;
                }
                else if (col.Size == ColumnSize.Auto)
                {
                    gc.Width = double.NaN;
                }

                if (col.Width.HasValue)
                {
                    gc.Width = col.Width.Value;
                }

                cols.Add(gc);
            }
            return cols;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        private static DataTemplate getRightAlignDataTemplate(string binding)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
            factory.SetBinding(TextBlock.TextProperty, new Binding(binding));
            template.VisualTree = factory;

            return template;
        }
    }
}
