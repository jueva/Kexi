using System;
using System.Collections;
using System.Collections.Generic;
using Kexi.ViewModel.Item;

namespace Kexi.Common
{
    public class FilterSorting : IComparer<BaseItem>, IComparer
    {
        public FilterSorting(string filter)
        {
            _filter = filter ?? "";
        }

        public int Compare(object x, object y)
        {
            return Compare(x as BaseItem, y as BaseItem);
        }


        public int Compare(BaseItem bix, BaseItem biy)
        {
            if (bix == null || biy == null)
                return bix == null && biy == null ? 0 : -1;

            if (bix.FilterString.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase) && !biy.FilterString.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase))
                return -1;
            if (!bix.FilterString.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase) && biy.FilterString.StartsWith(_filter, StringComparison.CurrentCultureIgnoreCase))
                return 1;

            if (bix.IsContainer && !biy.IsContainer) return -1;

            if (!bix.IsContainer && biy.IsContainer)
                return 1;

            return string.Compare(bix.FilterString, biy.FilterString, StringComparison.Ordinal);
        }

        private readonly string _filter;
    }
}