using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
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
    public class KeyCommandsLister : BaseLister<KexBindingItem>, ICanDelete
    {
        [ImportingConstructor]
        public KeyCommandsLister(Workspace workspace, Options options, CommandRepository commandRepository,
            KeyBindingsProvider keyBindingsProvider, SetKeyBindingPopupViewModel keyBindingPopupViewModel) : base(workspace, options, commandRepository)
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

        public override string ProtocolPrefix => "Keybindings";

        [ExportContextMenuCommand(typeof(KeyCommandsLister), "Delete Keybinding")]
        public ICommand DeleteKeybinding
        {
            get
            {
                return new RelayCommand(c => { Workspace.CommandRepository.Execute(nameof(DeleteKeyBindingCommand)); }
                );
            }
        }

        public void Delete()
        {
            var selection        = ItemsView.SelectedItems;
            var keyConfiguration = Workspace.KeyDispatcher.Configuration;
            var keyMode = Workspace.Options.KeyMode == KeyMode.ViStyle
                ? KeyMode.ViStyle
                : KeyMode.Classic;
            var sourceBindings = keyConfiguration.Bindings.SingleOrDefault(b => b.KeyMode == keyMode)?.KeyBindings;

            foreach (var binding in selection)
            {
                var bindings = sourceBindings?.Where(b => b.Equals(binding.Binding)).ToArray();
                if (bindings != null)
                {
                    sourceBindings.RemoveAll(b => b.Equals(binding.Binding));
                    binding.Binding = null;
                }
            }
        }

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

            Workspace.PopupViewModel              = _keyBindingPopupViewModel;
            _keyBindingPopupViewModel.BindingItem = item;
            _keyBindingPopupViewModel.Open();
        }
    }
}