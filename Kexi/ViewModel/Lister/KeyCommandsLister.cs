using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.ViewModel.Commands;
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
        public KeyCommandsLister(Workspace workspace,  Options options, CommandRepository commandRepository,
            KeyBindingsProvider keyBindingsProvider, SetKeyBindingPopupViewModel keyBindingPopupViewModel) : base(workspace,  options, commandRepository)
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
            if (item == null)
                return;

            Workspace.PopupViewModel = _keyBindingPopupViewModel;
            _keyBindingPopupViewModel.BindingItem = item;
            _keyBindingPopupViewModel.Open();
        }

        [ExportContextMenuCommand(typeof(KeyCommandsLister), "Delete Keybinding")]
        public ICommand DeleteKeybinding
        {
            get
            {
                return new RelayCommand(c =>
                    {
                        Workspace.CommandRepository.Execute(nameof(DeleteKeyBindingCommand));
                    }
                );
            }
        }
    }
}