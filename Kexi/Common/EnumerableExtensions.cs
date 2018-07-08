using System;
using System.Collections.Generic;

namespace Kexi.Common
{
    public static class EnumerableExtensions
    {
        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var i in items)
                action(i);
        }

       
    }
}
