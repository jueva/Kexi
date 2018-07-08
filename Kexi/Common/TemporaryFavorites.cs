using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.Common
{
    [Export(typeof(TemporaryFavorites<>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TemporaryFavorites<T> where T : class
    {
        private readonly ObservableCollection<T> _favorites = new ObservableCollection<T>();
        private int current = -1;

        public ObservableCollection<T> Favorites
        {
            get { return _favorites; }
        }

        public void Add(T item)
        {
            if (_favorites.Contains(item))
                return;

            _favorites.Add(item);
            current = _favorites.Count - 1;
        }

        public T Next()
        {
            if (current == -1)
                return null;

            var result = _favorites[current];
            current--;

            if (current < 0)
                current = _favorites.Count -1;
            return result;
        }

        public T Previous()
        {
            if (current == -1)
                return null;

            var result = _favorites[current];
            current++;

            if (current > _favorites.Count - 1)
                current = 0;

            return result;
        }

    }
}
