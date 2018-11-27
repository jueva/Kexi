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

            CancelCommand = new RelayCommand((o) =>
            {
                if (o is Window window)
                    window.Close();
            });

            FocusNextCommand = new RelayCommand(_ =>
            {
                if (Keyboard.FocusedElement is UIElement keyboardFocus)
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            });

            FocusPreviousCommand = new RelayCommand(_ =>
            {
                if (Keyboard.FocusedElement is UIElement keyboardFocus)
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            });

            KeyMode = _workspace.Options.KeyMode == KeyMode.Undefined 
                ? KeyMode.Classic 
                : _workspace.Options.KeyMode;

            Theme = _workspace.ThemeHandler.CurrentThemeName;
            TransparentBackground = _workspace.Options.TransparentBackground;
        }

        public bool TransparentBackground
        {
            get => _transparentBackground;
            set
            {
                if (value == _transparentBackground) return;
                _transparentBackground = value;
                OnPropertyChanged();
                _workspace.Options.TransparentBackground = value;
            }
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
                _workspace.Options.WriteToConfig(nameof(_workspace.Options.TransparentBackground), TransparentBackground.ToString());
            }
        }

        public ICommand OkButtonCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand FocusNextCommand { get; }
        public ICommand FocusPreviousCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Workspace               _workspace;
        private KeyMode _keyMode;
        private string _theme;
        private bool _transparentBackground;

        [NotifyPropertyChangedInvocator]
        private  void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}