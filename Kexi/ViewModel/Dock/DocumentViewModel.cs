using System.Windows.Input;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Dock
{
    public class DocumentViewModel : PaneViewModel
    {
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => Close())); }
        }

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                if (Equals(value, _isClosed))
                    return;
                _isClosed = value;
                OnPropertyChanged();
            }
        }

        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (Equals(value, _canClose))
                    return;

                _canClose = value;
                OnPropertyChanged();
            }
        }

        public ILister Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public string ToolTip
        {
            get => _toolTip;
            set
            {
                _toolTip = value;
                OnPropertyChanged();
            }
        }

        private bool     _canClose;
        private ICommand _closeCommand;
        private ILister  _content;
        private bool     _isClosed;
        private string   _toolTip;

        public void Close()
        {
            IsClosed = true;
        }
    }
}