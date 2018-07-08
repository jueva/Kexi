using System.Windows;
using System.Windows.Input;
using Kexi.UI.Base;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.View
{
    /// <summary>
    ///     Interaction logic for BreadcrumbPopup.xaml
    /// </summary>
    public partial class BreadcrumbPopup : BaseControl<BreadcrumbViewModel>

    {
        public BreadcrumbPopup()
        {
            InitializeComponent();
            listView.Loaded += (o, e) =>
            {
                listView.Items.MoveCurrentToFirst();
                listView.Focus();
                Keyboard.Focus(listView);
            };
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement) e.OriginalSource).DataContext is FileItem item)
            {
                popup.IsOpen = false;
                if (ViewModel.Workspace.ActiveLister is FileLister lister)
                {
                    lister.Path = item.Path;
                    lister.Refresh();
                }
            }
        }

        private void Popup_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
                ViewModel.Workspace.FocusCurrentOrFirst();
            }
            else if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (listView.SelectedItem is FileItem current && ViewModel.Workspace.ActiveLister is FileLister lister)
                {
                    lister.Path  = current.Path;
                    lister.Refresh();
                    popup.IsOpen = false;
                    ViewModel.Workspace.FocusCurrentOrFirst();
                }
            }
        }
    }
}