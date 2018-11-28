using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    [Export]
    public class BreadcrumbViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public BreadcrumbViewModel(Workspace workspace, Options options)
        {
            Workspace         = workspace;
            //CommandRepository = commandRepository;
            Options           = options;
        }

        public Workspace            Workspace         { get; }
        //public CommandRepository    CommandRepository { get; }
        public Options              Options           { get; }

        public bool AdressVisible
        {
            get => _adressVisible;
            set
            {
                if (value.Equals(_adressVisible)) return;
                _adressVisible = value;
                OnPropertyChanged();
            }
        }

        public bool BreadcrumbVisible
        {
            get => _breadcrumbVisible;
            set
            {
                if (value.Equals(_breadcrumbVisible)) return;
                _breadcrumbVisible = value;
                OnPropertyChanged();
            }
        }

        public BreadcrumbMode Mode
        {
            get => _mode;
            set
            {
                if (value == _mode) return;
                _mode = value;
                ModeChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public RelayCommand PathPartSelectedCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var path = c as string;
                    Workspace.ActiveLister.Path = path;
                    Workspace.ActiveLister.Refresh();
                }, a => Workspace.ActiveLister is IBreadCrumbProvider);
            }
        }
        public RelayCommand LastPathPartSelectedCommand
        {
            get
            {
                return new RelayCommand(c => { Mode = BreadcrumbMode.Adressbox; }, a => Workspace.ActiveLister is IBreadCrumbProvider);
            }
        }

        public RelayCommand PathSeparatorSelectedCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    if (c is Button button)
                    {
                        var part   = button.DataContext as PathPart;
                        PopupTarget        = button;
                        SeparatorPopupPath = part?.Path;
                        Open();
                    }
                }, a => Workspace.ActiveLister is IBreadCrumbProvider);
            }
        }

        public async void Open()
        {
            PopupVisible  = true;
            Items = Directory.EnumerateDirectories(SeparatorPopupPath).Select(i => new FileItem(i, ItemType.Container));
            foreach (var i in Items)
            {
                await i.SetDetailsAsync().ConfigureAwait(false);
            }
        }

        public Control PopupTarget
        {
            get => _popupTarget;
            set
            {
                if (Equals(value, _popupTarget)) return;
                _popupTarget = value;
                OnPropertyChanged();
            }
        }

        public string SeparatorPopupPath
        {
            get => _separatorPopupPath;
            set
            {
                if (value == _separatorPopupPath) return;
                _separatorPopupPath = value;
                OnPropertyChanged();
            }
        }

        public bool PopupVisible
        {
            get => _popupVisible;
            set
            {
                if (value.Equals(_popupVisible)) return;
                _popupVisible = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<FileItem> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        private bool _adressVisible;
        private bool _breadcrumbVisible = true;

        private IEnumerable<FileItem> _items;
        private BreadcrumbMode        _mode = BreadcrumbMode.Breadcrumb;


        private Control _popupTarget;
        private bool    _popupVisible;
        private string  _separatorPopupPath;


        public event EventHandler ModeChanged;
    }
}