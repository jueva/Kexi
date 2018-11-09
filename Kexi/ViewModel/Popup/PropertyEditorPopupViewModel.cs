using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class PropertyEditorPopupViewModel : PopupViewModel<PropertyEditorItem>
    {
        [ImportingConstructor]
        public PropertyEditorPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            TitleVisible  = true;
        }

        public void Open(SettingItem settingItem, object baseObject)
        {
            _settingItem  = settingItem;
            Title         = PropertyName = settingItem.Path;
            BaseObject    = baseObject;
            _propertyInfo = BaseObject.GetType().GetProperties().FirstOrDefault(p => p.Name == PropertyName);
            BaseItems     = GetValues().ToList();
            IsFreeText = !BaseItems.Any();
            base.Open();
            if (IsFreeText)
            {
                Text = _propertyInfo.GetValue(Options).ToString();
                SelectAll();
            }
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            if (!IsFreeText)
                base.TextChanged(sender, ea);
        }

        public bool IsFreeText { get; set; }
        public string PropertyName { get; private set; }
        public           object       BaseObject { get; private set; }
        private  PropertyInfo _propertyInfo;
        private  SettingItem  _settingItem;

        protected override void ItemSelected(PropertyEditorItem selectedItem)
        {
            var value = selectedItem?.Value ?? Text;
            var converter = TypeDescriptor.GetConverter(_propertyInfo.PropertyType);
            if (value is string && converter.CanConvertFrom(typeof(string)))
                value = converter.ConvertFrom(value);

            _propertyInfo.SetValue(BaseObject, value);
            var option = BaseObject as Options;
            option?.WriteToConfig(_propertyInfo.Name, value?.ToString() ?? "");
            _settingItem.Value = value;
            IsOpen             = false;
        }

        private IEnumerable<PropertyEditorItem> GetValues()
        {
            if (_propertyInfo.PropertyType == typeof(bool))
                return new[]
                {
                    new PropertyEditorItem("True", true),
                    new PropertyEditorItem("False", false)
                };

            if (_propertyInfo.PropertyType.BaseType == typeof(Enum))
            {
                var type = _propertyInfo.PropertyType;
                return
                    from object pe in Enum.GetValues(type)
                    select new PropertyEditorItem(Enum.GetName(type, pe), pe);
            }

            var dropDownValue = _propertyInfo.GetCustomAttributes<ConfigurationDropdownValuesAttribute>().SingleOrDefault();
            if (dropDownValue != null)
                return dropDownValue.Values.Select(v => new PropertyEditorItem(v, v));

            return Enumerable.Empty<PropertyEditorItem>();
        }
    }
}