using System.Collections.ObjectModel;

namespace Kexi.Common
{
    public class TemporaryFavorites<T> where T : class
    {
        public ObservableCollection<T> Favorites { get; } = new ObservableCollection<T>();

        private int _current = -1;

        public void Add(T item)
        {
            if (Favorites.Contains(item))
                return;

            Favorites.Add(item);
            _current = Favorites.Count - 1;
        }

        public T Next()
        {
            if (_current == -1)
                return null;

            var result = Favorites[_current];
            _current--;

            if (_current < 0)
                _current = Favorites.Count - 1;
            return result;
        }

        public T Previous()
        {
            if (_current == -1)
                return null;

            var result = Favorites[_current];
            _current++;

            if (_current > Favorites.Count - 1)
                _current = 0;

            return result;
        }
    }
}