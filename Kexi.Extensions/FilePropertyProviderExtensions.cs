using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.Extensions
{
    //[Export(typeof(IExtendedPropertyProvider))]
    //[ExportPropertyProvider(typeof(FileItem), "TestPropertyProvider")]
    public class TestPropertyExtension : IExtendedPropertyProvider
    {
        public string Description => "Just a test";

        public Task<IEnumerable<PropertyItem>> GetItems(IItem item)
        {
            var result = new[]{new PropertyItem("Extension", item.Path +" Yo yo check the flow")};
            return Task.FromResult(result.AsEnumerable());
        }
    }
}