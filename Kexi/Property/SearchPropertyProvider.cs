using System.ComponentModel.Composition;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(SearchLister), "Search Properties")]

    public class SearchItemPropertyProbider : FilePropertyProvider
    {
        [ImportingConstructor]
        public SearchItemPropertyProbider(Workspace workspace) : base(workspace)
        {
        }
    }
}