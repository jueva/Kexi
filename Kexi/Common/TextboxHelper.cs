using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kexi.Common
{
    public static class TextBoxHelper
    {

        public static string GetSelectedText(DependencyObject obj)
        {
            return (string)obj.GetValue(SelectedTextProperty);
        }

        public static void SetSelectedText(DependencyObject obj, string value )
        {
            obj.SetValue(SelectedTextProperty, value);
        }

        public static string GetCaretPositon(DependencyObject obj)
        {
            return (string)obj.GetValue(CaretPositionProperty);
        }

        public static void SetCaretPosition(DependencyObject obj, int value)
        {
            obj.SetValue(CaretPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached(
                "SelectedText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.RegisterAttached(
                "CaretPosition",
                typeof(int),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CaretPositionChanged));

        private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = obj as TextBox;
            if (tb != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    tb.SelectionChanged += tb_SelectionChanged;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    tb.SelectionChanged -= tb_SelectionChanged;
                }

                string newValue = e.NewValue as string;

                if (newValue != null && newValue != tb.SelectedText)
                {
                    tb.SelectedText = newValue as string;
                }
            }
        }

        static void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                SetSelectedText(tb, tb.SelectedText);
            }
        }

        private static void CaretPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TextBox;
            if (tb != null)
            {
                var newValue = (int)e.NewValue;

                if (newValue != tb.CaretIndex)
                {
                    tb.CaretIndex = newValue;
                }
            }
        }

    }
}
