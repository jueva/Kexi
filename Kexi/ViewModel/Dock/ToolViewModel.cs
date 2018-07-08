using Kexi.Common;

namespace Kexi.ViewModel.Dock
{
    public class ToolViewModel : PaneViewModel
    {
        public ToolViewModel(string name, Workspace workspace)
        {
            Name      = name;
            Title     = name;
            Workspace = workspace;
        }

        public Workspace Workspace { get; }

        public Options Options => Workspace.Options;

        public string Name { get; }

        public object Content
        {
            get => _content;
            set
            {
                if (value == _content)
                    return;

                _content = value;
                OnPropertyChanged();
            }
        }

     

        private object _content;

        #region IsVisible

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}