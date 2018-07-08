using System.ComponentModel.Composition;
using Kexi.Property;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolDetailViewModel : ToolViewModel
    {
        [ImportingConstructor]
        public ToolDetailViewModel(Workspace workspace) : base("Details", workspace)
        {
            ContentId   = "DetailView";
            AnchorTitle = "Details";
            Content     = new FilePropertyProvider(workspace);
            IsVisible   = true;
        }
    }
}