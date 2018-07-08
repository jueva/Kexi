using System;
using System.Collections.Concurrent;
using System.Linq;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kexi.Files
{
    public class FilePropertyProvider : IDisposable
    {
        public FilePropertyProvider(ShellObject shellObject)
        {
            _shellObject = shellObject;
            Items = new ConcurrentDictionary<string, PropertyItem>();
        }

        public ConcurrentDictionary<string, PropertyItem> Items { get; }

        public void Parse()
        {
            var so = _shellObject;
            var sys = so.Properties.DefaultPropertyCollection;
            foreach (
                var shellProperty in
                    sys.Where(sp => !string.IsNullOrEmpty(sp.CanonicalName) && sp.ValueAsObject != null))
            {
                try
                {
                    var propertyItem = new PropertyItem("", shellProperty.CanonicalName, shellProperty.CanonicalName,
                        shellProperty.ValueAsObject.ToString());
                    Items.TryAdd(shellProperty.CanonicalName, propertyItem);
                }
                catch
                {
                    // ignored
                }
            }
            _parsed = true;
        }

        public PropertyItem GetPropertyItem(string key)
        {
            lock (_locker)
            {
                if (!_parsed)
                    Parse();
                return Items.ContainsKey(key) ? Items[key] : null;
            }
        }

        public object GetValue(string key)
        {
            var prop = GetPropertyItem(key);
            return prop == null ? "" : prop.Value;
        }

        public bool ContainsKey(string key)
        {
            return GetPropertyItem(key) != null;
        }

        private readonly ShellObject _shellObject;

        private readonly object _locker = new object();
        private bool _parsed;

        public void Dispose()
        {
            _shellObject?.Dispose();
        }
    }
}