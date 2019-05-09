using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kexi
{
    [Export]
    [ExportItemDetails(typeof(FileItem), "Windows Filesystem Properties", "Windows")]
    public class ExtendedExtendedPropertyProvider : IExtendedPropertyProvider
    {
        public bool   Active      => true;
        public string Description => "Returns Windows File System Properties";

        public Task<IEnumerable<PropertyItem>> GetItems(IItem item, Detaillevel details)
        {
            var so = ShellObject.FromParsingName(item.Path);
            var ret =
                from sp in so.Properties.DefaultPropertyCollection
                let nameClean = CleanupPropertyName(sp?.CanonicalName)
                where !string.IsNullOrEmpty(sp?.CanonicalName) && sp.ValueAsObject != null
                select new PropertyItem("FileInfos", nameClean, nameClean, sp.ValueAsObject.ToString());
            return Task.FromResult(ret);
        }

        public bool IsMatch(IItem item)
        {
            return true;
        }

        private const int cleanupLength = 7; // "System.".Length

        private static string CleanupPropertyName(string propertyName)
        {
            return string.IsNullOrEmpty(propertyName) 
                ? "" 
                : propertyName.Substring(cleanupLength);
        }
    }
}