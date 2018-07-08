using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    public class ShowLastLocationsViewModel : ViewModelBase
    {
        private readonly Workspace _workspace;

        public ShowLastLocationsViewModel(Workspace workspace)
        {
            _workspace = workspace;
        }

        private IEnumerable<FileItem> _items;

        public IEnumerable<FileItem> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        private Control _popupTarget;
        private bool _separatorPopupVisible;

        public Control PopupTarget
        {
            get { return _popupTarget; }
            set
            {
                if (Equals(value, _popupTarget)) return;
                _popupTarget = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand PathSeparatorSelectedCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var button = c as Button;
                    PopupTarget = button;
                });
            }
        }

        public bool PopupVisible
        {
            get { return _separatorPopupVisible; }
            set
            {
                if (value.Equals(_separatorPopupVisible)) return;
                _separatorPopupVisible = value;
                if (value)
                {
                    Items = new List<FileItem>();
                }
                OnPropertyChanged();
            }
        }


    }
}
