namespace Kexi.ViewModel.Item
{
    public class PropertyItem : BaseItem
    {
        public PropertyItem(string group, string key, string displayName, string value) : base(key + "_" + value)
        {
            DisplayName = Path = FilterString = displayName;
            _group      = group;
            _key        = key;
            _value      = value;
        }

        public PropertyItem(string key, object val) : base(key + "_" + val)
        {
            DisplayName   = key;
            _key          = key;
            OriginalValue = val;
            _value        = val == null ? "" : val.ToString();
        }

        public object OriginalValue { get; }

        public string Key
        {
            get => _key;
            set
            {
                if (_key == value)
                    return;
                _key = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                _group = value;
                OnPropertyChanged();
            }
        }

        private string _group;

        private string _key;

        private string _value;
    }
}