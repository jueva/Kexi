using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Kexi.Common;
using Kexi.ViewModel;

namespace Kexi.UI.View
{
    public partial class PopupView
    {
        public PopupView()
        {
            InitializeComponent();
            DataContextChanged                 += PopupView_DataContextChanged;
            popup.CustomPopupPlacementCallback =  GetPopupPlacement;
        }

        public RelayCommand CloseCommand
        {
            get { return new RelayCommand(c => { ViewModel.IsOpen = false; }); }
        }

        private FrameworkElement _adressbar;
        private FrameworkElement _dockManager;

        private TextBox          _input;
        private Border           _inputBorder;
        private FrameworkElement _ribbonbar;

        private CustomPopupPlacement[] GetPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            {
                var x = targetSize.Width - popupSize.Width - 4;
                var y = offset.Y - 3;
                if (ViewModel.Options.CenterPopup && !ViewModel.MenuButtonPlacement)
                    x                                                                      =  x / 2;
                else if (ViewModel.MenuButtonPlacement && ViewModel.Options.CenterPopup) y += 26;
                return new[]
                {
                    new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.Horizontal)
                };
            }
        }

        private void PopupView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_input == null)
            {
                ViewModel.Workspace.TitleTextBox    = TitleText;
                ViewModel.Workspace.TitleTextBorder = TitleTextBorder;
                _input                              = ViewModel.Workspace.TitleTextBox;
                _inputBorder                        = ViewModel.Workspace.TitleTextBorder;
            }
            else
            {
                if (e.OldValue != null)
                    UnregisterKeyHandlers(e.OldValue as PopupViewModel);
            }

            ViewModel.SetCaret         =  i => _input.CaretIndex = i;
            ViewModel.SelectAll        =  () => _input.SelectAll();
            ViewModel.FocusTextbox     =  FocusTb;
            ViewModel.UnSelectListView =  () => _listView.UnselectAll();
            ViewModel.PropertyChanged  += ViewModel_PropertyChanged;
            SetPlacement();
        }

        private void SetPlacement()
        {
            if (Application.Current.MainWindow == null) //the warnings..
                return;

            popup.Placement = ViewModel.MousePlacement ? PlacementMode.Mouse : PlacementMode.Custom;
            _dockManager    = ((FrameworkElement) VisualParent).FindName("DockManager") as FrameworkElement;
            _adressbar      = ((FrameworkElement) VisualParent).FindName("AdressBar") as FrameworkElement;
            _ribbonbar      = ((FrameworkElement) VisualParent).FindName("RibbonBar") as FrameworkElement;
            if (ViewModel.Workspace.Options.CenterPopup && ViewModel.Workspace.Options.GlobalAdressbarVisible)
            {
                popup.PlacementTarget = Application.Current.MainWindow;
                popup.MaxHeight       = Application.Current.MainWindow.ActualHeight + 7;
            }
            else
            {
                popup.PlacementTarget = _ribbonbar != null && _ribbonbar.Visibility == Visibility.Visible ? _ribbonbar : _adressbar != null && _adressbar.Visibility == Visibility.Visible ? _adressbar : _dockManager;
                popup.MaxHeight       = Application.Current.MainWindow.ActualHeight - 23;
            }
        }

        private void FocusTb()
        {
            _input.Focus();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsOpen))
            {
                var model = sender as PopupViewModel;
                if (model?.IsOpen ?? false)
                {
                    SetPlacement();
                    _inputBorder.Visibility = Visibility.Visible;
                    RegisterKeyHandlers(model);
                    _input.Focus();
                }
                else
                {
                    ViewModel?.Workspace?.FocusListView(false);
                    UnregisterKeyHandlers(model);
                    model?.Close();
                    _inputBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RegisterKeyHandlers(PopupViewModel viewModel)
        {
            if (viewModel.HandlerRegistered)
                return;

            _input.KeyDown              += viewModel.KeyDown;
            _input.PreviewKeyDown       += viewModel.PreviewKeyDown;
            _input.PreviewTextInput     += viewModel.PreviewTextInput;
            _input.TextChanged          += viewModel.TextChanged;
            viewModel.HandlerRegistered =  true;
        }

        private void UnregisterKeyHandlers(PopupViewModel viewModel)
        {
            if (!viewModel.HandlerRegistered)
                return;

            _input.KeyDown              -= viewModel.KeyDown;
            _input.PreviewKeyDown       -= viewModel.PreviewKeyDown;
            _input.PreviewTextInput     -= viewModel.PreviewTextInput;
            _input.TextChanged          -= viewModel.TextChanged;
            viewModel.HandlerRegistered =  false;
        }

        private void _listView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null && ViewModel.IsOpen)
            {
                ViewModel.SelectionChanged(e);
                if (!e.Handled)
                    _listView.ScrollIntoView(_listView.SelectedItem);
            }
        }

        private void _listView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Utils.FindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) != null)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                ViewModel.MouseSelection(((FrameworkElement) e.OriginalSource).DataContext);
                e.Handled = true;
            }
        }
    }
}