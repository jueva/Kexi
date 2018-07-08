using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class FilterPopupViewModel : PopupViewModel<IItem>
    {

        [ImportingConstructor]
        public FilterPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
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

        public override void PreviewTextInput(object sender, TextCompositionEventArgs ea)
        {
            var result = (_firstInput ? "" : Text) + ea.Text; //newly opened, existing filter selected
            if (new ItemFilter<IItem>(Workspace.CurrentItems, result).IsEmpty && result != ".")
            {
                ea.Handled = true;
            }
            _firstInput = false;
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            var currentItem = Workspace.CurrentItem;
            if (currentItem != null)
                Workspace.ActiveLister.SetSelection(currentItem, false); //first entry is selected when filter is applied
            SetFilter(Text);
            Workspace.ActiveLister.ItemsView.MoveCurrentToFirst();
        }

        private async void SetFilter(string filter)
        {
            Workspace.ActiveLister.Filter = filter;
            var f = await Task.Run(() => GetFilterPredicate(filter));
            Workspace.ActiveLister.ItemsView.Filter = f;

            ((ListCollectionView) Workspace.ActiveLister.ItemsView).CustomSort = new FilterSorting(filter);
        }

        public static Predicate<object> GetFilterPredicate(string filter)
        {
            return item => new ItemFilter<IItem>(item as IItem, filter).Any();
        }

        public override void ItemSelected(IItem selectedItem)
        {
            Workspace.ActiveLister.Filter = null;
            Workspace.ActiveLister.DoAction(selectedItem);
            IsOpen = false;
        }
    }
}