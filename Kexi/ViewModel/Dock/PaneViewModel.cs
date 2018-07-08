using System.Windows.Media;

namespace Kexi.ViewModel.Dock
{
    public class PaneViewModel : ViewModelBase
    {
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AnchorTitle
        {
            get => _anchorTitle;
            set
            {
                if (_anchorTitle != value)
                {
                    _anchorTitle = value;
                    OnPropertyChanged();
                }
            }
        }


        public ImageSource IconSource { get; protected set; }

        public string ContentId
        {
            get => _contentId;
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }


        public double Width
        {
            get => _width;
            set
            {
                if (Equals(_width, value))
                    return;
                _width = value;
                OnPropertyChanged();
            }
        }

        private string _anchorTitle;

        private string _contentId;

        private bool _isActive;

        private bool _isSelected;


        private string _title;
        private double _width;
    }
}