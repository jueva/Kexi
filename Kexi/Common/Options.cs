using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Kexi.Annotations;
using Kexi.Common.KeyHandling;

namespace Kexi.Common
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Options : INotifyPropertyChanged
    {
        private bool _adressbarVisible;
        private bool _anchorablesMoveable;

        private double _anchorableWidth;
        private bool _beepOnNoMatch;
        private bool _centerPopup;
        private int _consoleEncoding;
        private ViewType _defaultViewMode;
        private int _doubleClickTime;
        private bool _extentedNavigationIcons;
        private FontFamily _fontFamily;
        private double _fontSize;
        private bool _globalAdressebarVisible;
        private string _globalHotKey;
        private bool _highlights;
        private bool _loadLastLayout;
        private double _notificationDuration;
        private PopupAnimation _popupAnimation;
        private bool _popupTitleVisible;
        private string _preferredEditorLocation;
        private bool _ribbonVisible;
        private bool _showHiddenItems;
        private bool _showPathPartDividers;
        private bool _statusbarVisible;
        private string _theme;
        private string _viewClipboardHotKey;
        private KeyMode _keyMode;
        private bool _showSearchStringInClassicMode;

        public Options()
        {
            var settings = new AppSettingsReader();
            FontFamily = new FontFamily((string) settings.GetValue("FontFamily", typeof(string)));
            FontSize = double.Parse((string) settings.GetValue("FontSize", typeof(string)));
            Theme = (string) settings.GetValue("Theme", typeof(string));
            BeepOnNoMatch = (bool) settings.GetValue("BeepOnNoMatch", typeof(bool));
            PopupTitleVisible = (bool) settings.GetValue("PopupTitleVisible", typeof(bool));
            LoadLastLayout = (bool) settings.GetValue("LoadLastLayout", typeof(bool));
            PreferredEditorLocation = (string) settings.GetValue("PreferredEditorLocation", typeof(string));
            StatusbarVisible = false;
            GlobalAdressbarVisible = true;
            GlobalHotKey = (string) settings.GetValue("GlobalHotKey", typeof(string));
            ViewClipboardHotKey = (string) settings.GetValue("ViewClipboardHotKey", typeof(string));
            PopupAnimation = (PopupAnimation)Enum.Parse(typeof(PopupAnimation),  (string) settings.GetValue("PopupAnimation", typeof(string)));
            DoubleClickTime = (int) GetDoubleClickTime();
            ShowHiddenItems = (bool) settings.GetValue("ShowHiddenItems", typeof(bool));
            RibbonVisible = false;
            DefaultViewMode = (ViewType) Enum.Parse(typeof(ViewType), (string) settings.GetValue("DefaultViewMode", typeof(string)));
            ConsoleEncoding = (int) settings.GetValue("ConsoleEncoding", typeof(int));
            NotificationDuration = (double) settings.GetValue("NotificationDuration", typeof(double));
            AnchorablesMoveable = false;
            AnchorablesWidth = 0;
            ExtentedNavigationIcons = false;
            ShowPathPartDividers = true;
            StatusbarVisible = true;
            CenterPopup = (bool) settings.GetValue("CenterPopup", typeof(bool));;
            AdressbarVisible = true;
            Highlights = (bool) settings.GetValue("Highlights", typeof(bool));
            ShowSearchStringInClassicMode = (bool) settings.GetValue("ShowSearchStringInClassicMode", typeof(bool));
            var keyString = (string) settings.GetValue("KeyMode", typeof(string));
            KeyMode = Enum.TryParse<KeyMode>(keyString, true, out var mode) 
                ? mode : KeyMode.Undefined;
        }
        public bool ShowSearchStringInClassicMode
        {
            get => _showSearchStringInClassicMode;
            set
            {
                if (value == _showSearchStringInClassicMode) return;
                _showSearchStringInClassicMode = value;
                OnPropertyChanged();
            }
        }

        public bool LoadLastLayout
        {
            get => _loadLastLayout;
            set
            {
                if (value == _loadLastLayout) return;
                _loadLastLayout = value;
                OnPropertyChanged();
            }
        }

        public bool PopupTitleVisible
        {
            get => _popupTitleVisible;
            set
            {
                if (value == _popupTitleVisible) return;
                _popupTitleVisible = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPathPartDividers
        {
            get => _showPathPartDividers;
            set
            {
                if (value == _showPathPartDividers) return;
                _showPathPartDividers = value;
                OnPropertyChanged();
            }
        }

        public bool ExtentedNavigationIcons
        {
            get => _extentedNavigationIcons;
            set
            {
                if (value == _extentedNavigationIcons) return;
                _extentedNavigationIcons = value;
                OnPropertyChanged();
            }
        }

        public bool AnchorablesMoveable
        {
            get => _anchorablesMoveable;
            set
            {
                if (value == _anchorablesMoveable)
                    return;
                _anchorablesMoveable = value;
                OnPropertyChanged();
                AnchorablesWidth = value ? double.NaN : 0;
            }
        }

        public double AnchorablesWidth
        {
            get => _anchorableWidth;
            set
            {
                if (Math.Abs(value - _anchorableWidth) < 0.1)
                    return;
                _anchorableWidth = value;
                OnPropertyChanged();
            }
        }

        public string ViewClipboardHotKey
        {
            get => _viewClipboardHotKey;
            set
            {
                if (_viewClipboardHotKey == value)

                    return;

                _viewClipboardHotKey = value;
                OnPropertyChanged();
            }
        }

        public int ConsoleEncoding
        {
            get => _consoleEncoding;
            set
            {
                if (value == _consoleEncoding) return;
                _consoleEncoding = value;
                OnPropertyChanged();
            }
        }

        public ViewType DefaultViewMode
        {
            get => _defaultViewMode;
            set
            {
                if (value == _defaultViewMode) return;
                _defaultViewMode = value;
                OnPropertyChanged();
            }
        }

        public int DoubleClickTime
        {
            get => _doubleClickTime;
            set
            {
                _doubleClickTime = value;
                OnPropertyChanged();
            }
        }

        [ConfigurationDropdownValues(Values = new[] {
            "Century Gothic", 
            "Segoe UI",
            "Eras ITC",
            "Consolas",
            "Microsoft Sans Serif",
            "Courier",
        })]
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                OnPropertyChanged();
            }
        }

        public double FontSize

        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        public bool BeepOnNoMatch
        {
            get => _beepOnNoMatch;
            set
            {
                _beepOnNoMatch = value;
                OnPropertyChanged();
            }
        }

        public string PreferredEditorLocation
        {
            get => _preferredEditorLocation;
            set
            {
                _preferredEditorLocation = value;

                OnPropertyChanged();
            }
        }

        public bool AdressbarVisible
        {
            get => _adressbarVisible;
            set
            {
                if (value.Equals(_adressbarVisible)) return;
                _adressbarVisible = value;
                OnPropertyChanged();
            }
        }

        public bool GlobalAdressbarVisible
        {
            get => _globalAdressebarVisible;
            set
            {
                if (value.Equals(_globalAdressebarVisible)) return;
                _globalAdressebarVisible = value;
                AdressbarVisible = value;
                OnPropertyChanged();
            }
        }

        public bool StatusbarVisible
        {
            get => _statusbarVisible;
            set
            {
                if (value.Equals(_statusbarVisible)) return;
                _statusbarVisible = value;
                OnPropertyChanged();
            }
        }

        public string GlobalHotKey
        {
            get => _globalHotKey;
            set
            {
                if (value == _globalHotKey) return;
                _globalHotKey = value;
                OnPropertyChanged();
            }
        }

        public PopupAnimation PopupAnimation
        {
            get => _popupAnimation;
            set
            {
                if (value == _popupAnimation) return;
                _popupAnimation = value;
                OnPropertyChanged();
            }
        }

        public bool ShowHiddenItems
        {
            get => _showHiddenItems;
            set
            {
                if (value.Equals(_showHiddenItems)) return;
                _showHiddenItems = value;
                OnPropertyChanged();
            }
        }

        public bool RibbonVisible
        {
            get => _ribbonVisible;
            set
            {
                _ribbonVisible = value;
                OnPropertyChanged();
            }
        }

        public double NotificationDuration
        {
            get => _notificationDuration;
            set
            {
                _notificationDuration = value;
                OnPropertyChanged();
            }
        }

        public bool Highlights
        {
            get => _highlights;
            set
            {
                _highlights = value;
                OnPropertyChanged();
            }
        }

        public bool CenterPopup
        {
            get => _centerPopup;
            set
            {
                if (value == _centerPopup) return;
                _centerPopup = value;
                OnPropertyChanged();
            }
        }

        public KeyMode KeyMode
        {
            get => _keyMode;
            set
            {
                if (value == _keyMode) 
                    return;

                _keyMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsInitialized => KeyMode != KeyMode.Undefined;

        public event PropertyChangedEventHandler PropertyChanged;

        public void WriteToConfig(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = config.AppSettings.Settings[key];
            if (setting != null)
            {
                setting.Value = value;
                config.Save(ConfigurationSaveMode.Full);
            }
        }

        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

        public void RestoreAdressbarVisibility()
        {
            if (GlobalAdressbarVisible != AdressbarVisible)
                AdressbarVisible = GlobalAdressbarVisible;
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}