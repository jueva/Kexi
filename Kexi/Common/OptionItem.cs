using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kexi.Annotations;

namespace Kexi.Common
{
    public class OptionItem<T> : INotifyPropertyChanged
        where T : class
    {
        public string Group
        {
            get => _group;
            set
            {
                if (value == _group) return;
                _group = value;
                OnPropertyChanged();
            }
        }

        public T Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _group;
        private T      _value;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}