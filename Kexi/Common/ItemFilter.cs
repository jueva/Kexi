using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kexi.Interfaces;

namespace Kexi.Common
{
    public class ItemFilter<T> : IEnumerable<T> where T : class, IItem
    {
        public ItemFilter(T item, string filterString) : this(new[] {item}, filterString)
        {
        }

        public ItemFilter(IEnumerable<T> items, string filterString)
        {
            _items = items;
            if (string.IsNullOrEmpty(filterString))
            {
                _filterParts = new List<FilterPart>();
                return;
            }

            var filter = filterString.Trim();
            if (filterString.IndexOf(' ') == -1)
                _filterParts = new List<FilterPart> {new FilterPart(filter)};
            else
                _filterParts = filter
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => new FilterPart(p)).Where(p => p.HasFilter).ToList();
        }

        public bool IsEmpty => !Matches.Any();

        public bool IsSinglePart => FilterParts.Count == 1;

        public bool IsEmptyAndSinglePart => IsEmpty && IsSinglePart;

        protected List<FilterPart> FilterParts => _filterParts;

        public IEnumerable<T> MatchesBeginning
        {
            get { return _items.Where(i => MatchItemStartsWith(i, FilterParts.FirstOrDefault())); }
        }

        public IEnumerable<T> MatchesContaining
        {
            get { return FilterParts.Aggregate(_items, (current, part) => current.Where(i => MatchItemContaining(i, part))); }
        }

        public virtual IEnumerable<T> Matches
        {
            get
            {
                var ret = IsSinglePart && !FilterParts.First().Negate
                    ? MatchesBeginning.Union(MatchesContaining, new ItemFilterComparer())
                    : MatchesContaining;
                return ret;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Matches == null
                ? Enumerable.Empty<T>().GetEnumerator()
                : Matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<FilterPart> _filterParts;
        private readonly IEnumerable<T>   _items;

        protected virtual bool MatchItemStartsWith(T item, FilterPart part)
        {
            if (item == null)
                return false;

            if (FilterParts.Count == 0)
                return true;
            var ret = item.FilterString.StartsWith(FilterParts.First().FilterString, StringComparison.OrdinalIgnoreCase);
            return part.Negate ? !ret : ret;
        }

        protected virtual bool MatchItemContaining(T item, FilterPart part)
        {
            if (item?.FilterString == null)
                return false;

            if (part.Negate)
                return item.FilterString.IndexOf(part.FilterString, StringComparison.OrdinalIgnoreCase) == -1;
            return item.FilterString.IndexOf(part.FilterString, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public bool IsMatch(object item)
        {
            var popup = item as T;
            return
                _filterParts.All(p => MatchItemStartsWith(popup, p))
                || _filterParts.All(p => MatchItemContaining(popup, p));
        }

        private class ItemFilterComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x?.FilterString == y?.FilterString;
            }

            public int GetHashCode(T obj)
            {
                return obj.FilterString == null
                    ? 0
                    : obj.FilterString.GetHashCode();
            }
        }
    }
}