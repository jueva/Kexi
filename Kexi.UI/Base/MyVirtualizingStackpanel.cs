using System.Windows.Controls;

namespace Kexi.UI.Base
{
    public class MyVirtualizingStackPanel : VirtualizingStackPanel
    {
        protected override void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {
            if (e.UIElement is ListBoxItem item && item.IsSelected)
            {
                e.Cancel  = true;
                e.Handled = true;
                return;
            }

            if (e.UIElement is TreeViewItem item2 && item2.IsSelected)
            {
                e.Cancel  = true;
                e.Handled = true;
                return;
            }

            base.OnCleanUpVirtualizedItem(e);
        }
    }
}