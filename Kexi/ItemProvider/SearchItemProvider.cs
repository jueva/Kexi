using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ItemProvider
{
    public class SearchItemProvider : IDisposable
    {
        private readonly object _locker = new object();
        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        public async Task GetItems(string initialDirectory, string searchPattern, ObservableCollection<FileItem> items = null)
        {
            if (items != null)
            {
                items.Clear();
                BindingOperations.EnableCollectionSynchronization(items, _locker);
            }
            await Task.Run(() => GetFilesSave(initialDirectory, searchPattern, items), CancellationTokenSource.Token).ConfigureAwait(false);
            SearchFinished?.Invoke();
        }

        private void GetFilesSave(string container, string pattern, ICollection<FileItem> items)
        {
            var stack = new Stack<string>();
            do
            {
                try
                {
                    if (stack.Any())
                        container = stack.Pop();

                    var files =
                        Directory.EnumerateDirectories(container).Select(d => new DirectoryInfo(d))
                            .Select(d => new FileItem(d.FullName)).Where(s => IsMatch(s, pattern))
                            .Union(Directory.EnumerateFiles(container).Select(f => new FileInfo(f))
                                .Select(s => new FileItem(s.FullName)).Where(s => IsMatch(s, pattern))
                            );

                    foreach (var fi in files)
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            items?.Add(fi);
                            ItemAdded?.Invoke(fi);
                        });
                    }

                    foreach (var d in Directory.EnumerateDirectories(container, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        stack.Push(d);
                    }
                }
                catch (Exception)
                {
                    if (stack.Any())
                        container = stack.Pop();
                }
            } while (stack.Count != 0 && !CancellationTokenSource.IsCancellationRequested);
        }

        private static bool IsMatch(FileItem fi, string pattern)
        {
            return new ItemFilter<FileItem>(fi, pattern).Any();
        }

        public event Action<FileItem> ItemAdded;
        public event Action SearchFinished;

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
        }
    }
}