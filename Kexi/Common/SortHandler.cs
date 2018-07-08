using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Kexi.Interfaces;
using Kexi.View;
using Kexi.ViewModel.Lister;

namespace Kexi.Common
{
    public class SortHandler
    {
        public SortHandler(ILister lister)
        {
            _lister = lister;
        }

        public SortDescription CurrentSortDescription
        {
            get
            {
                if (_lister?.ItemsView == null)
                    return new SortDescription();

                var items = _lister.ItemsView;
                return items.SortDescriptions.FirstOrDefault();
            }
        }

        private readonly ILister _lister;

        public void HandleSorting(GridViewColumnHeader header)
        {
            var allColumns  = _lister.Columns.Where(co => !string.IsNullOrEmpty(co.Header)).ToDictionary(co => co.Header, co => co.BindingExpression);
            var items       = _lister.View.ListView.Items;
            var currentSort = items.SortDescriptions.FirstOrDefault();
            var content     = header.Content as string;
            if (string.IsNullOrEmpty(content))
            {
                items.SortDescriptions.Clear();
                ClearSort();
                return;
            }

            var newSort = allColumns[content];

            var direction = ListSortDirection.Ascending;
            if (currentSort.PropertyName == newSort)
                direction = currentSort.Direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

            if (!string.IsNullOrEmpty(newSort))
            {
                items.SortDescriptions.Clear();
                items.SortDescriptions.Add(new SortDescription(newSort, direction));
            }

            UpdateSortAdorner(header, direction, _lister);
        }

        public void HandleSorting(IItem selectedItem, ListSortDirection? sortDirection = null)
        {
            var allColumns = _lister.Columns.Where(co => !string.IsNullOrEmpty(co.Header)).ToDictionary(co => co.Header, co => co.BindingExpression);
            var items       = _lister.View?.ListView.Items;
            if (items == null) //Lister closed
                return;

            var currentSort = items.SortDescriptions.FirstOrDefault();
            var newSort     = allColumns[selectedItem.DisplayName];

            var direction = sortDirection ?? ListSortDirection.Ascending;
            if (currentSort.PropertyName == newSort)
                direction = currentSort.Direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

            using (items.DeferRefresh())
            {
                items.SortDescriptions.Clear();
                var sortDescription = new SortDescription(newSort, direction);
                items.SortDescriptions.Add(sortDescription);
            }

            if (_lister.View.ListView.View is GridView gridView)
            {
                var headers = gridView.Columns.Select(c => c.Header as GridViewColumnHeader).Where(c => c?.Content != null).ToDictionary(c => c.Content as string, c => c);
                var header  = headers[selectedItem.Path];
                UpdateSortAdorner(header, direction, _lister);
            }
        }

        public void ClearSort()
        {
            if (_lister?.View?.CurrentSortColumn != null)
            {
                _lister.View.ListView.Items.SortDescriptions.Clear();
                var layer = AdornerLayer.GetAdornerLayer(_lister.View.CurrentSortColumn);
                layer?.Remove(_lister.View.CurrentSortAdorner);
            }

            if (_lister?.ItemsView is ListCollectionView col) col.CustomSort = null;
        }

        private static void UpdateSortAdorner(GridViewColumnHeader header, ListSortDirection direction, ILister lister)
        {
            if (header == null)
                return;

            if (lister.View.CurrentSortColumn != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(lister.View.CurrentSortColumn);
                adornerLayer?.Remove(lister.View.CurrentSortAdorner);
            }

            lister.View.CurrentSortColumn  = header;
            lister.View.CurrentSortAdorner = new SortAdorner(lister.View.CurrentSortColumn, direction);
            AdornerLayer.GetAdornerLayer(lister.View.CurrentSortColumn)?.Add(lister.View.CurrentSortAdorner);
        }
    }
}