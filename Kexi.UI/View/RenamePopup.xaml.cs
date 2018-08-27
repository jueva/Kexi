using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Popup;

namespace Kexi.UI.View
{
    public partial class RenamePopup
    {
        public RenamePopup()
        {
            InitializeComponent();
            DataContextChanged        += RenamePopup_DataContextChanged;
            RenameTextBox.TextChanged += RenameTextBox_TextChanged;
            pixelPerDip               =  VisualTreeHelper.GetDpi(this).PixelsPerDip;
        }

        private bool _cancelRename;

        private          double               _originalSize;
        private          IRenameable          _renameable;
        private          IItem                _targetItem;
        private readonly double               pixelPerDip;
        private          RenamePopupViewModel ViewModel;

        private void RenamePopup_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = e.NewValue as RenamePopupViewModel;
        }

        private void RenamePopup_Opened(object sender, EventArgs e)
        {
            SetTextboxContentAndSize();
            _targetItem = ViewModel.Workspace.ActiveLister.View.ListView.SelectedItem as IItem;
            if (_targetItem is IRenameable renameable)
            {
                _cancelRename = false;
                _renameable   = renameable;
                var selectionBorders = _renameable.GetRenameSelectonBorder();
                RenameTextBox.SelectionStart  = selectionBorders.Item1;
                RenameTextBox.SelectionLength = selectionBorders.Item2;
            }
            else
            {
                return;
            }

            RenameTextBox.Focus();
        }

        private void RenamePopupOnClosed(object sender, EventArgs eventArgs)
        {
            if (!_cancelRename)
                DoRename();

            ViewModel.IsOpen = false;
            ViewModel.Workspace.FocusCurrentOrFirst();
        }

        private void RenameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var typeFace = new Typeface(ViewModel.Workspace.Options.FontFamily.ToString());
            var ft       = new FormattedText(RenameTextBox.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, ViewModel.Workspace.Options.FontSize, Brushes.Black, pixelPerDip);
            if (ft.Width + 20 > _originalSize)
                RenameTextBox.Width = ft.Width + 20;
        }

        private void RenameTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Return:
                    ViewModel.IsOpen = false;
                    break;
                case KeyDispatcher.AlternateEscape:
                case Key.Escape:
                    _cancelRename    = true;
                    ViewModel.IsOpen = false;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void DoRename()
        {
            try
            {
                _renameable?.Rename(RenameTextBox.Text);
            }
            catch (Exception ex)
            {
                ViewModel.Workspace.NotificationHost.AddError("Fehler beim Umbenennen", ex);
            }
            finally
            {
                ViewModel.IsOpen = false;
            }
        }

        private void SetTextboxContentAndSize()
        {
            var view = ViewModel.Workspace.ActiveLister.View;
            var item = (IItem) view.ListView.SelectedItem;
            RenameTextBox.Text = item.DisplayName;
            var typeFace = new Typeface(ViewModel.Workspace.Options.FontFamily.ToString());
            var ft       = new FormattedText(RenameTextBox.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, ViewModel.Workspace.Options.FontSize, Brushes.Black, pixelPerDip);
            _originalSize = ft.Width + 20;
        }
    }
}