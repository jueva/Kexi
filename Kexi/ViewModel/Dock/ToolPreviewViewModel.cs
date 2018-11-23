using System.ComponentModel.Composition;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolPreviewViewModel : ToolViewModel
    {
        public const string Identifier = "Preview";

        [ImportingConstructor]
        public ToolPreviewViewModel(Workspace workspace) : base("Preview", workspace)
        {
            ContentId   = Identifier;
            AnchorTitle = "Preview";
            Content     = new PreviewContentView(Workspace);
        }
    }
}