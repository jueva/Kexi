using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SettingsPopupViewModel : PopupViewModel<SettingItem>
    {
        [ImportingConstructor]
        public SettingsPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler, PropertyEditorPopupViewModel propertyEditorPopup) : base(workspace, options, mouseHandler)
        {
            _propertyEditorPopup = propertyEditorPopup;
            Title                = "Settings";
            TitleVisible = true;
        }

        private readonly PropertyEditorPopupViewModel _propertyEditorPopup;

        public override void Open()
        {
            BaseItems = typeof(Options).GetProperties().OrderBy(p => p.Name).Select(p => 
                new SettingItem(p.Name)
                {
                    DisplayName = $"{p.Name} ({GetCurrentValue(p.Name).ToString() })"
                }
            );
            SetHeaderIconByKey("appbar_control_settings");
            base.Open();
        }

        protected override void ItemSelected(SettingItem selectedItem)
        {
            IsOpen = false;
            DoAction(selectedItem);
        }

        private object GetCurrentValue(string propertyName)
        {
            var prop = typeof(Options).GetProperty(propertyName);
            return prop == null ? null : prop.GetValue(Options);
        }

        private void DoAction(SettingItem item)
        {
            if (item == null)
                return;

            var prop = typeof(Options).GetProperty(item.Path);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                var currentValue = (bool) prop.GetValue(Options);
                var newValue     = !currentValue;
                prop.SetValue(Options, newValue);
                Options.WriteToConfig(item.Path, newValue.ToString());
                item.Value = newValue;
                return;
            }

            Workspace.PopupViewModel = _propertyEditorPopup;
            _propertyEditorPopup.Open(item, Options);
        }
    }
}