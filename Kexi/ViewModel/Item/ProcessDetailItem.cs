using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Kexi.Annotations;

namespace Kexi.ViewModel.Item
{
    public class ProcessDetailItem : INotifyPropertyChanged
    {
        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (Equals(value, _thumbnail)) return;
                _thumbnail = value;
                OnPropertyChanged();
            }
        }


        public string Description
        {
            get => _description;
            set
            {
                if (value == _description)
                    return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public long Memory
        {
            get => _memory;
            set
            {
                if (value == _memory) return;
                _memory = value;
                OnPropertyChanged();
            }
        }

        public int Cpu
        {
            get => _cpu;
            set
            {
                if (value == _cpu) return;
                _cpu = value;
                OnPropertyChanged();
            }
        }

        public int Pid
        {
            get => _pid;
            set
            {
                if (value == _pid) return;
                _pid = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private int                              _cpu;
        private string                           _description;
        private long                             _memory;
        private int                              _pid;
        private BitmapSource                     _thumbnail;

        [NotifyPropertyChangedInvocator]
        internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}