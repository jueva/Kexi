namespace Kexi.ViewModel
{
    public class RibbonViewModel : ViewModelBase
    {
        public RibbonViewModel(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }

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

        private bool _ribbonVisible;
    }
}