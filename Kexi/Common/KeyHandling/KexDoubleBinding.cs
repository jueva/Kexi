using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class KexDoubleBinding : KexBinding
    {
        public KexDoubleBinding() : base() { }

        public KexDoubleBinding(string group, Key key, ModifierKeys modifier, Key secondKey, ModifierKeys secondModifier,
                                string action, IKexiCommand command) : base(group, key, modifier, action, command)
        {
            SecondKey = secondKey;
            SecondModifier = secondModifier;
        }

        public Key SecondKey
        {
            get { return _secondKey; }
            set
            {
                if (value == _secondKey) return;
                _secondKey = value;
                OnPropertyChanged();
            }
        }

        public ModifierKeys SecondModifier
        {
            get { return _secondModifier; }
            set
            {
                if (value == _secondModifier) return;
                _secondModifier = value;
                OnPropertyChanged();
            }
        }

        private Key _secondKey;
        private ModifierKeys _secondModifier;
    }
}