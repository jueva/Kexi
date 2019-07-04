using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Kexi.Interfaces
{

    interface IHasBackgroundData<in T>
        where T : class, IItem
    {
        bool BackgroundDataLoaded { get; }
        void LoadBackgroundData(T item);
    }
}