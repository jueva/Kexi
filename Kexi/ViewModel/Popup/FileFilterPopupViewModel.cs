using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Commands;

namespace Kexi.ViewModel.Popup
{
    //Keeps Open on Selection, Hacky
    [Export]
    public class FileFilterPopupViewModel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public FileFilterPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title              = "Filter";
            HideInputAtStartup = false;
        }

        public override void Open()
        {
            base.Open();
            Text = Workspace.ActiveLister.Filter;
            SetHeaderIconByKey("appbar_filter");
        }

        public override void Close()
        {
            _firstInput = true;
            base.Close();
        }

        private bool _firstInput = true;
        private bool _pauseFilter;

        public override void PreviewTextInput(object sender, TextCompositionEventArgs ea)
        {
            var result = (_firstInput ? "" : Text) + ea.Text; //newly opened, existing filter selected
            if (new ItemFilter<IItem>(Workspace.CurrentItems, result).IsEmptyIgnoringNegated && result != ".")
            {
                if (Workspace.Options.BeepOnNoMatch)
                    System.Media.SystemSounds.Beep.Play();
                ea.Handled = true;
            }
            _firstInput = false;
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            var currentItem = Workspace.CurrentItem;
            if (currentItem != null)
                Workspace.ActiveLister.SetSelection(currentItem, false);
             SetFilter(Text);
            Workspace.ActiveLister.ItemsView.MoveCurrentToFirst();
        }

        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            var shift = (ea.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (ea.Key == Key.Tab && !shift)
            {
                var v = Workspace.ActiveLister.ItemsView;
                if (v != null) //maybe no items in Listview (filter)
                {
                    v.MoveCurrentToNext();
                    if (v.IsCurrentAfterLast)
                        v.MoveCurrentToFirst();
                }
                ea.Handled = true;
            }
            else if (ea.Key == Key.Tab && shift)
            {
                var v = Workspace.ActiveLister.ItemsView;
                if (v != null)
                {
                    v.MoveCurrentToPrevious();
                    if (v.IsCurrentBeforeFirst)
                        v.MoveCurrentToLast();
                }
                ea.Handled                                         = true;
            }
            else if (ea.Key == Key.OemPeriod)
            {
                if (Workspace.ActiveLister is IHistorisationProvider history)
                {
                    if (Text == ".")
                    {
                        var previousItem = history.History.Peek();
                        if (previousItem != null)
                        {
                            var filter = previousItem.Filter;
                            CommandRepository.GetCommandByName(nameof(HistoryBackKeepFilterCommand)).Execute();
                            Text = filter;
                            SelectAll();
                            _firstInput = true;
                        }
                        ea.Handled  = true;
                    }
                }
            }
            else
            {
                base.PreviewKeyDown(sender, ea);
            }
        }

        private void SetFilter(string filter)
        {
            if (_pauseFilter)
                return;

            Workspace.ActiveLister.Filter           = filter;
            Workspace.ActiveLister.ItemsView.Filter = item => new ItemFilter<IItem>(item as IItem, filter).Any();
            ((ListCollectionView) Workspace.ActiveLister.ItemsView).CustomSort = new FilterSorting(filter);
        }

        protected override void ItemSelected(IItem selectedItem)
        {
            CommandRepository.GetCommandByName(nameof(DoActionCommand)).Execute();
            //avoid flickering on filter popup, when current list is filtered, filter is set to null
            //and all items from current list are shown before refresh happens
            _pauseFilter = true; 
            Text = "";
            _pauseFilter = false;
        }

    }
}