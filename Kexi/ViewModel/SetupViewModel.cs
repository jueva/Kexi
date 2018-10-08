using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Kexi.Annotations;
using Kexi.Common.KeyHandling;

namespace Kexi.ViewModel
{
    public class SetupViewModel : INotifyPropertyChanged
    {
        public SetupViewModel(Workspace workspace)
        {
            _workspace      = workspace;
            OkButtonCommand = new RelayCommand(CloseWindow);
            KeyMode = KeyMode.Classic;
        }

        public KeyMode KeyMode
        {
            get => _keyMode;
            set
            {
                if (value == _keyMode) return;
                _keyMode = value;
                OnPropertyChanged();
            }
        }

        public string Theme
        {
            get => _theme;
            set
            {
                if (value == _theme) return;
                _theme = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkButtonCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Workspace               _workspace;
        private KeyMode _keyMode;
        private string _theme;

        private void CloseWindow(object parameter)
        {
            if (parameter is Window window)
                window.Close();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}