using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;
using Kexi.Annotations;
using Kexi.Interfaces;

namespace Kexi.Common.KeyHandling
{
    [XmlInclude(typeof(KexDoubleBinding))]
    [Serializable]
    public class KexBinding : INotifyPropertyChanged
    {
        protected KexBinding()
        {
        }

        public KexBinding(string group, Key key, ModifierKeys modifier, string commandName, IKexiCommand action, object[] commandArguments = null)
        {
            Key              = key;
            Modifier         = modifier;
            Command          = action;
            CommandName      = commandName;
            CommandArguments = commandArguments;
            Group            = group;
        }

        public string CommandName
        {
            get => _commandName;
            set
            {
                if (value == _commandName)
                    return;
                _commandName = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public object[] CommandArguments
        {
            get => _commandArguments;
            private set
            {
                if (Equals(value, _commandArguments)) return;
                _commandArguments = value;
                OnPropertyChanged();
            }
        }

        public ModifierKeys Modifier
        {
            get => _modifier;
            set
            {
                if (value == _modifier) return;
                _modifier = value;
                OnPropertyChanged();
            }
        }

        public Key Key
        {
            get => _key;
            set
            {
                if (value == _key) return;
                _key = value;
                OnPropertyChanged();
            }
        }

        public string Group { get; set; }

        [XmlIgnore]
        public IKexiCommand Command { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized] private object[] _commandArguments;

        private string       _commandName;
        private Key          _key;
        private ModifierKeys _modifier;

        public override string ToString()
        {
            var firstMod = Modifier == ModifierKeys.None ? "" : Modifier + "-";
            return $"{firstMod}{Key}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KexBinding binding))
                return false;

            return binding.Key == Key && binding.Modifier == Modifier && binding.CommandName == CommandName;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Modifier.GetHashCode() ^ CommandName.GetHashCode();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}