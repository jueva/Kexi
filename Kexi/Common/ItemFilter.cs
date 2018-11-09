using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kexi.Interfaces;

namespace Kexi.Common
{
    public class ItemFilter<T> : IEnumerable<T> where T : class, IItem
    {
        private string _filterString;

        public ItemFilter(T item, string filterString) : this(new[] {item}, filterString)
        {
        }

        public ItemFilter(IEnumerable<T> items, string filterString)
        {
            _filterString = filterString;
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
            _firstPart = _filterParts.FirstOrDefault();
        }

        private readonly FilterPart _firstPart;
        private static ItemFilterComparer _comparer = new ItemFilterComparer();

        public bool IsEmpty => !Matches.Any();

        public bool IsSinglePart => FilterParts.Count == 1;

        public bool IsEmptyAndSinglePart => IsEmpty && IsSinglePart;

        protected List<FilterPart> FilterParts => _filterParts;

        public IEnumerable<T> MatchesEquals
        {
            get { return _items.Where(i => MatchItemEquals(i, _firstPart)); }
        }

        public IEnumerable<T> MatchesBeginning
        {
            get { return _items.Where(i => MatchItemStartsWith(i, _firstPart)); }
        }

        public IEnumerable<T> MatchesContaining
        {
            get { return FilterParts.Aggregate(_items, (current, part) => current.Where(i => MatchItemContaining(i, part))); }
        }

        public IEnumerable<T> Matches
        {
            get
            {
                if (_filterString == "-")
                    return _items;

                var ret = IsSinglePart && !_firstPart.Negate
                    ? MatchesEquals.Union(MatchesBeginning.Union(MatchesContaining, _comparer), _comparer)
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

        protected virtual bool MatchItemEquals(T item, FilterPart part)
        {
            if (item?.FilterString == null)
                return false;
            var ret = item.FilterString.Equals(part.FilterString, StringComparison.OrdinalIgnoreCase);
            return part.Negate ? !ret : ret;
        }

        protected virtual bool MatchItemStartsWith(T item, FilterPart part)
        {
            if (item?.FilterString == null)
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