using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Lister
{
    [Export]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyCommandsLister : BaseLister<KexBindingItem>
    {
        [ImportingConstructor]
        public KeyCommandsLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository,
            KeyBindingsProvider keyBindingsProvider, SetKeyBindingPopupViewModel keyBindingPopupViewModel) : base(workspace, notificationHost, options, commandRepository)
        {
            _keyBindingsProvider      = keyBindingsProvider;
            _keyBindingPopupViewModel = keyBindingPopupViewModel;
            Title                     = PathName = Path = "Key Bindings";
        }

        public override bool ShowInMenu => true;

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Lister", "Lister"),
            new Column("Key", "Key"),
            new Column("Command", "CommandName")
        };

        public override  string                      ProtocolPrefix => "Keybindings";
        private readonly SetKeyBindingPopupViewModel _keyBindingPopupViewModel;
        private readonly KeyBindingsProvider         _keyBindingsProvider;

        protected override Task<IEnumerable<KexBindingItem>> GetItems()
        {
            return Task.FromResult(_keyBindingsProvider.GetBindings());
        }

        public override void DoAction(KexBindingItem item)
        {
            Workspace.PopupViewModel = _keyBindingPopupViewModel;
            _keyBindingPopupViewModel.Open();
        }
    }
}