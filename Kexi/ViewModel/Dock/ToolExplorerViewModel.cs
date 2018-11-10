using System.ComponentModel.Composition;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolExplorerViewModel : ToolViewModel
    {
        public const string Identifier = "ExplorerView";

        [ImportingConstructor]
        public ToolExplorerViewModel(Workspace workspace) : base("Explorer", workspace)
        {
            ContentId   = Identifier;
            AnchorTitle = "Tree";
            Content     = new ExplorerContentView(workspace);
        }
    }
}