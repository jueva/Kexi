using System.ComponentModel.Composition;

namespace Kexi.ViewModel
{
    [Export]
    public class RibbonViewModel : ViewModelBase
    {
        public Workspace Workspace { get; }
        private bool _ribbonVisible;

        [ImportingConstructor]
        public RibbonViewModel(Workspace workspace)
        {
            Workspace = workspace;
        }

        public CommandRepository CommandRepository => Workspace.CommandRepository;

        public bool RibbonVisible
        {
            get => _ribbonVisible;
            set
            {
                if (value.Equals(_ribbonVisible)) return;
                _ribbonVisible = value;
                OnPropertyChanged();
            }
        }
    }
}