using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Kexi.Common;
using Kexi.UI.Base;
using Kexi.ViewModel;

namespace Kexi.UI.View
{
    public partial class PopupView : BaseControl<PopupViewModel>
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

        private TextBox          _input;
        private FrameworkElement adressbar;
        private FrameworkElement dockManager;
        private Border           InputBorder;
        private FrameworkElement ribbonbar;

        private CustomPopupPlacement[] GetPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            {
                var x = targetSize.Width - popupSize.Width - 4;
                if (ViewModel.Options.CenterPopup)
                    x = x / 2;
                return new[]
                {
                    new CustomPopupPlacement(new Point(x, offset.Y - 3), PopupPrimaryAxis.Horizontal)
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
                InputBorder                         = ViewModel.Workspace.TitleTextBorder;
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
            popup.Placement = ViewModel.MousePlacement ? PlacementMode.Mouse : PlacementMode.Custom;
            dockManager     = ((FrameworkElement) VisualParent).FindName("DockManager") as FrameworkElement;
            adressbar       = ((FrameworkElement) VisualParent).FindName("AdressBar") as FrameworkElement;
            ribbonbar       = ((FrameworkElement) VisualParent).FindName("RibbonBar") as FrameworkElement;
            if (ViewModel.Workspace.Options.CenterPopup && ViewModel.Workspace.Options.GlobalAdressbarVisible)
            {
                popup.PlacementTarget = Application.Current.MainWindow;
                popup.MaxHeight       = Application.Current.MainWindow.ActualHeight + 7;
            }
            else
            {
                popup.PlacementTarget = ribbonbar != null && ribbonbar.Visibility == Visibility.Visible ? ribbonbar : adressbar != null && adressbar.Visibility == Visibility.Visible ? adressbar : dockManager;
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
                    InputBorder.Visibility = Visibility.Visible;
                    RegisterKeyHandlers(model);
                    _input.Focus();
                    ShowInput();
                }
                else
                {
                    ViewModel?.Workspace?.FocusListView(false);
                    UnregisterKeyHandlers(model);
                    model?.Close();
                    InputBorder.Visibility = Visibility.Collapsed;
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
            ItemSelected(sender, e);
        }

        private void ItemSelected(object sender, MouseButtonEventArgs e)
        {
            if (Utils.FindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) != null)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                ViewModel.MouseSelection(((FrameworkElement) e.OriginalSource).DataContext);
                e.Handled = true;
            }
        }

        protected void ShowInput()
        {
            //if (ViewModel.Options.PopupAnimation != PopupAnimation.None)
            //{
            //    var storyboard = new Storyboard();
            //    var duration = TimeSpan.FromMilliseconds(500);

            //    var fadeAnimation = new DoubleAnimation
            //    {
            //        From = 0.0,
            //        To = 1.0,
            //        Duration = new Duration(duration)
            //    };
            //    Storyboard.SetTarget(fadeAnimation, InputBorder);
            //    Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(OpacityProperty));
            //    storyboard.Children.Add(fadeAnimation);
            //    storyboard.Begin(this);
            //}

            InputBorder.Visibility = Visibility.Visible;
        }
    }
}