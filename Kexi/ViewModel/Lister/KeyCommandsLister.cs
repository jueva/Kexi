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
    [Export]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyCommandsLister : BaseLister<KexBindingItem>
    {
        [ImportingConstructor]
        public KeyCommandsLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository,
            [ImportMany] IEnumerable<IKexiCommand> commands, SetKeyBindingPopupViewModel keyBindingPopupViewModel) : base(workspace, notificationHost, options, commandRepository)
        {
            _commands = commands;
            _keyBindingPopupViewModel = keyBindingPopupViewModel;
            Title     = PathName = Path = "Key Bindings";
        }

        public override bool ShowInMenu => true;

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Lister", "Lister"),
            new Column("Key", "Key"),
            new Column("Command", "CommandName")
        };

        public override  string                    ProtocolPrefix => "Keybindings";
        private readonly IEnumerable<IKexiCommand> _commands;
        private readonly SetKeyBindingPopupViewModel _keyBindingPopupViewModel;

        protected override Task<IEnumerable<KexBindingItem>> GetItems()
        {
            var allCommands = _commands.Select(n => new KexBindingItem(n.GetType().Name, "")).ToList();
            foreach (var c in allCommands.ToArray())
            {
                foreach (var b in Workspace.KeyDispatcher.Bindings.Where(bi => bi.CommandName == c.CommandName && bi.Group != c.Lister)) //TODO: SecondKey
                {
                    allCommands.Remove(c);
                    allCommands.Add(new KexBindingItem(b, b.Group));
                }
            }

            return Task.FromResult(allCommands.OrderBy(k => k.CommandName).AsEnumerable());
        }

        public override void DoAction(KexBindingItem item)
        {
            Workspace.PopupViewModel = _keyBindingPopupViewModel;
            _keyBindingPopupViewModel.Open();
        }
    }
}