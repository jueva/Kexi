using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Kexi.Interfaces
{
    interface IBackgroundLoader
    {
        void LoadBackgroundData(IEnumerable items, CancellationToken cancellationToken);
    }

    interface IBackgroundLoader<in T> : IBackgroundLoader
        where T : class, IItem
    {
        void LoadBackgroundData(IEnumerable<T> items, CancellationToken cancellationToken);
    }
}