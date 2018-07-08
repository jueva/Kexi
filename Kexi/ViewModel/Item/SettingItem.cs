using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class SettingItem : BaseItem
    {
        public SettingItem(string name, object value) : base(name)
        {
            Value = value;
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
