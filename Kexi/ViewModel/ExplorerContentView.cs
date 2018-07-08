using Kexi.ViewModel.TreeView;

namespace Kexi.ViewModel
{
    public class ExplorerContentView
    {
        private RootViewModel _explorerRoot;

        public ExplorerContentView(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }

        public RootViewModel ExplorerRoot
        {
            get => _explorerRoot ?? (_explorerRoot = new RootViewModel(Workspace));
            set => _explorerRoot = value;
        }
    }
}