using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyCommandsLister : BaseLister<KexBindingItem>
    {
        [ImportingConstructor]
        public KeyCommandsLister(Workspace       workspace,     INotificationHost                      notificationHost, Options options, CommandRepository commandRepository,
            [ImportMany] IEnumerable<KeyDispatcher> keyHandlerses, [ImportMany] IEnumerable<IKexiCommand> commands) : base(workspace, notificationHost, options, commandRepository)
        {
            _commands    = commands;
            Title        = PathName = Path ="Key Bindings";
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

        protected override Task<IEnumerable<KexBindingItem>> GetItems()
        {
            var allCommands = _commands.Select(n => new KexBindingItem(n.GetType().Name, "")).ToList();
            //TODO: Keymode....
            //foreach (var c in allCommands.ToImmutableArray())
            //foreach (var b in KeyHandler.Bindings.Where(bi => bi.CommandName == c.CommandName && bi.Group != c.Lister)) //TODO: SecondKey
            //{
            //    allCommands.Remove(c);
            //    allCommands.Add(new KexBindingItem(b, b.Group));
            //}

            return Task.FromResult(allCommands.OrderBy(k => k.CommandName).AsEnumerable());
        }

        public override void DoAction(KexBindingItem item)
        {
        }
    }
}