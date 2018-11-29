using System;
using System.ComponentModel.Composition;
using Kexi.Property;

namespace Kexi.ViewModel.Dock
{
    [Export]
    [Export(typeof(ToolViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolDetailViewModel : ToolViewModel
    {
        public const string Identifier = "DetailView";
        [ImportingConstructor]
        public ToolDetailViewModel(Workspace workspace) : base("Details", workspace)
        {
            ContentId   = Identifier;
            AnchorTitle = "Details";
            Content     = new FilePropertyProvider(workspace);
            IsVisible   = true;
        }


    }
}