using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.Annotations;

namespace Kexi.Common
{
    public class OneKeyGesture  : InputGesture
    {
        protected readonly Key Key;

        public OneKeyGesture(Key key) 
        {
            Key = key;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            return Key == ((KeyEventArgs)inputEventArgs).Key;
        }
    }

    public class KexKeyGesture : InputGesture
    {
        protected readonly Key _key;
        protected readonly ModifierKeys _modifiers;
        protected readonly string _displayString;
        protected KeyGesture _keyGesture;

        public KexKeyGesture(Key key, ModifierKeys modifiers = ModifierKeys.None, [NotNull] string displayString = "") 
        {
            if (modifiers != ModifierKeys.None)
            {
                _keyGesture = new KeyGesture(key,modifiers, displayString);
            }
            else
            {
                _key = key;
                _displayString = displayString;
            }
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var keyEvent = inputEventArgs as KeyEventArgs;
            if (keyEvent == null)
                return false;

            if (_keyGesture != null)
                return _keyGesture.Matches(targetElement, inputEventArgs);

            return _key == keyEvent.Key;
        }
    }
}
