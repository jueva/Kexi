using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SortPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public SortPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Sort";
            TitleVisible = true;
        }

        public override void Open()
        {
            _allColumns = Workspace.ActiveLister.Columns.Where(co => !string.IsNullOrEmpty(co.Header))
                .ToDictionary(co => co.Header, co => co.BindingExpression);

            SetHeaderIconByKey("appbar_sort");
            BaseItems = _allColumns.Select(s => new BaseItem(s.Key));
            base.Open();
        }

        protected Dictionary<string, string> _allColumns;

        public override void ItemSelected(BaseItem selectedItem)
        {
            var position = Workspace.ActiveLister.View.ListView.SelectedIndex;
            new SortHandler(Workspace.ActiveLister).HandleSorting(selectedItem);
            var item  = Workspace.ActiveLister.View.ListView.Items.GetItemAt(position) as IItem;
            Workspace.ActiveLister.View.FocusItem(item);
            Text   = "";
            IsOpen = false;
        }
    }
}