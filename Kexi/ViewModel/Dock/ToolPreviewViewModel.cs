using System.ComponentModel.Composition;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolPreviewViewModel : ToolViewModel
    {
        [ImportingConstructor]
        public ToolPreviewViewModel(Workspace workspace) : base("Preview", workspace)
        {
            ContentId   = "Preview";
            AnchorTitle = "Preview";
            Content     = new PreviewContentView(Workspace);
        }
    }
}