using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.View;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.View
{
    public partial class RecentLocationsPopup
    {
        public RecentLocationsPopup()
        {
            InitializeComponent();
            popup.Opened += Popup_Opened;
        }

        private void Popup_Opened(object sender, System.EventArgs e)
        {
            Keyboard.Focus(listView);
            ViewModel.ItemsView.MoveCurrentToFirst();
        }

        private RecentLocationPopupViewModel ViewModel => DataContext as RecentLocationPopupViewModel;

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement) e.OriginalSource).DataContext as IItem;
            if (item != null)
            {
                popup.IsOpen = false;
                ViewModel.Workspace.ActiveLister.Path = item.Path;
            }
        }

        private void Popup_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                    
                case Key.Escape :
                case KeyHandler.AlternateEscape:
                    popup.IsOpen = false;
                    ViewModel.Workspace.FocusCurrentOrFirst();
                    break;
                case Key.Enter:
                {
                    var current = listView.SelectedItem as BaseItem;
                    var lister = ViewModel.Workspace.ActiveLister as FileLister;
                    if (current != null && lister != null)
                    {
                        lister.Path = current.Path;
                        lister.Refresh();
                        popup.IsOpen = false;
                        ViewModel.Workspace.FocusCurrentOrFirst();
                    }
                }
                    break;
            }
        }
    }
}