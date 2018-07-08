using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class GroupByPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public GroupByPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Group by";
        }

        private Dictionary<string, string> _allColumns;

        public override void Open()
        {
            _allColumns = Workspace.ActiveLister.Columns.Where(co => !string.IsNullOrEmpty(co.Header))
                .ToDictionary(co => co.Header, co => co.BindingExpression);

            SetHeaderIconByKey("appbar_group");
            BaseItems = new[] {new BaseItem("")}.Union(_allColumns.Select(s => new BaseItem(s.Key)));
            base.Open();
        }

        public override void ItemSelected(BaseItem selectedItem)
        {
            Workspace.ActiveLister.GroupBy = string.IsNullOrEmpty(selectedItem.Path)
                ? null
                : _allColumns[selectedItem.Path];
            IsOpen = false;
        }
    }
}