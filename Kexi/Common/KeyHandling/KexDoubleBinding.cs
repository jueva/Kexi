using System.Windows.Input;
using Kexi.Interfaces;

namespace Kexi.Common.KeyHandling
{
    public class KexDoubleBinding : KexBinding
    {
        public KexDoubleBinding()
        {
        }

        public KexDoubleBinding(string group, Key key, ModifierKeys modifier, Key secondKey, ModifierKeys secondModifier,
            string action, IKexiCommand command) : base(group, key, modifier, action, command)
        {
            SecondKey      = secondKey;
            SecondModifier = secondModifier;
        }

        public Key SecondKey
        {
            get => _secondKey;
            set
            {
                if (value == _secondKey) return;
                _secondKey = value;
                OnPropertyChanged();
            }
        }

        public ModifierKeys SecondModifier
        {
            get => _secondModifier;
            set
            {
                if (value == _secondModifier) return;
                _secondModifier = value;
                OnPropertyChanged();
            }
        }

        private Key          _secondKey;
        private ModifierKeys _secondModifier;

        public override string ToString()
        {
            var firstMod  = Modifier == ModifierKeys.None ? "" : Modifier + "-";
            var secondMod = SecondModifier == ModifierKeys.None ? "" : SecondModifier + "-";
            return $"{firstMod}{Key}, {secondMod}{SecondKey}";
        }

         public override bool Equals(object obj)
        {
            if (!(obj is KexDoubleBinding binding))
                return false;


            return base.Equals(obj) && binding.SecondKey == SecondKey && binding.SecondModifier == SecondModifier;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Modifier.GetHashCode() ^ CommandName.GetHashCode() ^ SecondKey.GetHashCode() ^ SecondModifier.GetHashCode();
        }

    }
}