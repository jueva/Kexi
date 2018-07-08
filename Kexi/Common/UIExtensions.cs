using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kexi.Common
{
    public static class UIExtensions
    {

        public static DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(UIExtensions), new UIPropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusedProperty, value);
        }

        public static void OnIsFocusedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)dependencyObject;
            if ((bool)e.NewValue)
                element.Focus();
        }

        public static DependencyProperty IsAllSelectedProperty = DependencyProperty.RegisterAttached("IsAllSelected", typeof(bool), typeof(UIExtensions), new UIPropertyMetadata(false, OnIsAllSelectedChanged));

        public static bool GetIsAllSelected(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsAllSelectedProperty);
        }

        public static void SetIsAllSelected(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsAllSelectedProperty, value);
        }

        public static void OnIsAllSelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var textbox = dependencyObject as TextBox;
            if ((bool)e.NewValue)
                textbox.SelectAll();
        }


    }
}
