using System.Collections.ObjectModel;
using System.Windows;
using Kexi.Common;

namespace Kexi.ViewModel.Dock
{
    public class DockViewModel : ViewModelBase
    {
        public DockViewModel(Workspace workspace)
        {
            Workspace         = workspace;
            DetailViewModel = new ToolDetailViewModel(workspace);
            ExplorerViewModel = new ToolExplorerViewModel(workspace);
            PreviewViewModel = new ToolPreviewViewModel(workspace);
            Tools = new ToolViewModel[] { DetailViewModel, ExplorerViewModel, PreviewViewModel };
            DockWidth = new GridLength(250);
        }

        public ObservableCollection<DocumentViewModel> Files
        {
            get => _files;
            set
            {
                if (Equals(value, _files))
                    return;

                _files = value;
                OnPropertyChanged();
            }
        }

        public object ActiveLayoutContent
        {
            get => _activeLayoutContent;
            set
            {
                _activeLayoutContent = value;

                if (value is DocumentViewModel docview)
                    Workspace.ActiveLister = docview.Content;
                OnPropertyChanged();
            }
        }

        public ToolViewModel[] Tools { get; set; }

        public Workspace           Workspace       { get; }
        public Options             Options         => Workspace.Options;
        public ToolDetailViewModel DetailViewModel { get; }

        public ToolExplorerViewModel ExplorerViewModel { get; }

        public  ToolPreviewViewModel                    PreviewViewModel { get; }
        private object                                  _activeLayoutContent;
        private ObservableCollection<DocumentViewModel> _files = new ObservableCollection<DocumentViewModel>();

        private GridLength _dockWidth;

        public GridLength DockWidth
        {
            get => _dockWidth;
            set
            {
                if (value.Equals(_dockWidth)) return;
                _dockWidth = value;
                OnPropertyChanged();
            }
        }
    }
}