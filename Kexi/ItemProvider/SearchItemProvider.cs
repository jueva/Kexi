using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Kexi.ViewModel.Item;

namespace Kexi.ItemProvider
{
    public class SearchItemProvider
    {
        private readonly object _locker = new object();

        public async Task GetItems(string initialDirectory, string searchPattern, ObservableCollection<FileItem> items = null)
        {
            items?.Clear();
            if (items != null)
                BindingOperations.EnableCollectionSynchronization(items, _locker);
            await Task.Run(() => GetFilesSave(initialDirectory, searchPattern, items));
            SearchFinished?.Invoke();
        }

        private void GetFilesSave(string container, string pattern, ObservableCollection<FileItem> items)
        {
            var stack = new Stack<string>();
            do
            {
                try
                {
                    if (stack.Any())
                        container = stack.Pop();

                    var files =
                        Directory.EnumerateDirectories(container).Select(d => new DirectoryInfo(d)).Where(s => s.Name.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) > -1)
                            .Select(d => new FileItem(d.FullName))
                            .Union(Directory.EnumerateFiles(container).Select(f => new FileInfo(f)).Where(s => s.Name.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) > -1)
                                .Select(s => new FileItem(s.FullName))
                            );

                    foreach (var fi in files)
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            items?.Add(fi);
                            ItemAdded?.Invoke(fi);
                        });

                    foreach (var d in Directory.EnumerateDirectories(container, "*.*", SearchOption.TopDirectoryOnly))
                        stack.Push(d);
                }
                catch (Exception)
                {
                    if (stack.Any())
                        container = stack.Pop();
                }
            } while (stack.Count != 0);
        }

        public event Action<FileItem> ItemAdded;
        public event Action SearchFinished;
    }
}