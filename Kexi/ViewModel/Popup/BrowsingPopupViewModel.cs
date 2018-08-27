using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel.Commands;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class BrowsingPopupViewModel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public BrowsingPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Navigate";
        }

        public override void Open()
        {
            BaseItems                       =  Workspace.ActiveLister.Items.ToList();
            Workspace.ActiveLister.GotItems += ActiveListerGotItems;
            SetHeaderIconByKey("appbar_folder");
            ItemsView.MoveCurrentTo(Workspace.ActiveLister.ItemsView.CurrentItem);
            base.Open();
        }

        public override void Close()
        {
            Workspace.ActiveLister.GotItems -= ActiveListerGotItems;
            Workspace.ActiveLister.HighlightString = null;
            base.Close();
        }

        private void ActiveListerGotItems(object sender, EventArgs e)
        {
            BaseItems = Items = Workspace.ActiveLister.Items;
            FocusInput();
            OnGotItems(this, EventArgs.Empty);
            ItemsView.MoveCurrentTo(Workspace.ActiveLister.ItemsView.CurrentItem);
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            base.TextChanged(sender, ea);
            if (Options.Highlights)
                Workspace.ActiveLister.HighlightString = Text;
        }

        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            var ctrl = (ea.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            if (ctrl)
            {
                if (ea.Key == KeyDispatcher.MoveLeftKey)
                {
                    CommandRepository.GetCommandByName(nameof(HistoryBackCommand)).Execute();
                    BaseItems = Items = Workspace.CurrentItems;
                    Text = "";
                    ea.Handled = true;
                }
                else if (ea.Key == KeyDispatcher.MoveRightKey)
                {
                    CommandRepository.GetCommandByName(nameof(HistoryForwardCommand)).Execute();
                    BaseItems = Items = Workspace.CurrentItems;
                    Text = "";
                    ea.Handled = true;
                }
            }
            base.PreviewKeyDown(sender, ea);
            FocusInput();
        }

        public override void SelectionChanged(SelectionChangedEventArgs ea)
        {
            base.SelectionChanged(ea);
            SynchronizeSelection();
            FocusInput();
        }

        protected override void ItemSelected(IItem selectedItem)
        {
            IgnoreTextChanged = true;
            Text = "";
            IgnoreTextChanged = false;
            if (selectedItem == null)
            {
                return;
            }
            base.ItemSelected(selectedItem);

            if (selectedItem.TargetType == null || selectedItem.TargetType()  == ItemType.Item)
                IsOpen = false;

            Workspace.ActiveLister.DoAction(selectedItem);
        }

        private void SynchronizeSelection()
        {
            if (ItemsView.CurrentItem == null)
            {
                return;
            }

            Workspace.ActiveLister.View.ListView.SelectedItem = null;
            var selected = BaseItems.FirstOrDefault(i => i.Path == ((IItem) ItemsView.CurrentItem).Path);
            Workspace.ActiveLister.View.FocusItem(selected);
        }
    }
}