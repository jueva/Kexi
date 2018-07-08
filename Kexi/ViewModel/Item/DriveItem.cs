using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;

namespace Kexi.ViewModel.Item
{
    public class DriveItem : BaseItem
    {
        private bool _isReady;
        public string _driveLetter;

        public DriveItem(string driveLetter) : base(driveLetter)
        {
            _driveLetter = driveLetter;
            Path = $"{_driveLetter}:/";
        }

        public bool IsReady
        {
            get { return _isReady; }
            set
            {
                if (Equals(value, _isReady)) return;

                _isReady = value;
                OnPropertyChanged();
            }
        }

        public string DriveLetter
        {
            get
            {
                return _driveLetter;
            }
            set
            {
                if (value == _driveLetter)
                    return;

                _driveLetter = value;
                OnPropertyChanged();
            }
        }
    }
}
