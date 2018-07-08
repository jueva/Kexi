using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class PropertyEditorItem : BaseItem
    {
        public PropertyEditorItem(string displayName, object value) : base(displayName)
        {
            Value = value;
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
