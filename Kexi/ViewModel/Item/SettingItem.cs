namespace Kexi.ViewModel.Item
{
    public class SettingItem : BaseItem
    {
        public SettingItem(string name) : base(name)
        {
        }

        public SettingItem(string name, object value) : this(name)
        {
            Value = value;
        }

        public object Value
        {
            get => _value;
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        private object _value;
    }
}