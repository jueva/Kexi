using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(SettingsLister))]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsLister : BaseLister<SettingItem>
    {
        [ImportingConstructor]
        public SettingsLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository, PropertyEditorPopupViewModel propertyEditorPopup)
            : base(workspace, notificationHost, options, commandRepository)
        {
            _propertyEditorPopup = propertyEditorPopup;
            Title                = PathName = Path = "Settings";
            Thumbnail            = Utils.GetImageFromRessource("process.png");
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Name", "DisplayName") {Width = 300},
            new Column("Value", "Value") {Width      = 300}
        };

        public override  string                       ProtocolPrefix => "Settings";
        private readonly PropertyEditorPopupViewModel _propertyEditorPopup;

        protected override Task<IEnumerable<SettingItem>> GetItems()
        {
            var settings = typeof(Options).GetProperties().OrderBy(p => p.Name).Select(p => new SettingItem(p.Name, GetValue(p.Name)));
            return Task.FromResult(settings);
        }

        private object GetValue(string propertyName)
        {
            var prop = typeof(Options).GetProperty(propertyName);
            return prop == null ? null : prop.GetValue(Options);
        }

        public override void DoAction(SettingItem item)
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