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
            OkButtonCommand = new RelayCommand(OkButtonClicked);
            KeyMode = _workspace.Options.KeyMode == KeyMode.Undefined 
                ? KeyMode.Classic : _workspace.Options.KeyMode;
            Theme = _workspace.ThemeHandler.CurrentThemeName;
        }

        public KeyMode KeyMode
        {
            get => _keyMode;
            set
            {
                if (value == _keyMode) return;
                _keyMode = value;
                OnPropertyChanged();
                _workspace.Options.KeyMode = value;
            }
        }

        public string Theme
        {
            get => _theme;
            set
            {
                if (value == _theme) return;
                _theme = value;
                _workspace.ThemeHandler.ChangeTheme(value);
                OnPropertyChanged();
            }
        }

        private void OkButtonClicked(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
                _workspace.Options.WriteToConfig(nameof(_workspace.Options.KeyMode), KeyMode.ToString());
                _workspace.Options.WriteToConfig(nameof(_workspace.Options.Theme), Theme);
            }
        }

        public ICommand OkButtonCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Workspace               _workspace;
        private KeyMode _keyMode;
        private string _theme;

        [NotifyPropertyChangedInvocator]
        private  void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}