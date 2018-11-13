using System.Collections.Generic;
using System.ComponentModel;

namespace Kexi.Common
{
    public class BrowsingHistory
    {
        public HistoryItem Current => _currentIndex < 0 ? null : Locations[_currentIndex];

        public HistoryItem Peek()
        {
            return _currentIndex > 0 
                ? Locations[_currentIndex - 1] 
                : null;
        }

        public HistoryItem Previous
        {
            get
            {
                if (_currentIndex > 0)
                    _currentIndex--;
                return Current;
            }
        }

        public HistoryItem Next
        {
            get
            {
                if (_currentIndex < Locations.Count - 1 && _currentIndex <= _maxIndex)
                    _currentIndex++;
                return Current;
            }
        }

        public Dictionary<int, HistoryItem> Locations { get; } = new Dictionary<int, HistoryItem>(20);

        private int _currentIndex = -1;
        private int _maxIndex     = -1;

        public void Push(string newLocation, string filter, string groupBy, SortDescription sortDescription)
        {
            if (_currentIndex > -1 && newLocation == Locations[_currentIndex].FullPath)
                return;
            if (Current != null)
            {
                Current.SelectedPath    = newLocation;
                Current.Filter          = filter;
                Current.GroupBy         = groupBy;
                Current.SortDescription = sortDescription;
            }

            _currentIndex++;
            _maxIndex = _currentIndex;

            Locations[_currentIndex] = new HistoryItem(newLocation, _currentIndex);
        }
    }

    public class HistoryItem
    {
        public HistoryItem(string fullpath, int index)
        {
            FullPath     = fullpath;
            SelectedPath = null;
            Index        = index;
        }

        public string          FullPath        { get; set; }
        public string          SelectedPath    { get; set; }
        public int             Index           { get; set; }
        public string          Filter          { get; set; }
        public string          GroupBy         { get; set; }
        public SortDescription SortDescription { get; set; }
    }
}