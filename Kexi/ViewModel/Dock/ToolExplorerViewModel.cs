using System.ComponentModel.Composition;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolExplorerViewModel : ToolViewModel
    {
        [ImportingConstructor]
        public ToolExplorerViewModel(Workspace workspace) : base("Explorer", workspace)
        {
            ContentId   = "ExplorerView";
            AnchorTitle = "Tree";
            Content     = new ExplorerContentView(workspace);
        }
    }
}