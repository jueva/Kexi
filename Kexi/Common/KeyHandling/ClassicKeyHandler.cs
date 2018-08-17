using System.Collections.Generic;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;

namespace Kexi.Common.KeyHandling
{
    public class ClassicKeyHandler : IKeyHandler 
    {
        private readonly Workspace _workspace;

        public ClassicKeyHandler(Workspace workspace)
        {
            this._workspace = workspace;
        }

        public List<KexBinding> Bindings { get; set; }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            var modifierKeys = args.KeyboardDevice.Modifiers;
            switch (args.Key)
            {
                case Key.F1:
                    new ShowCommandsPopupCommand(_workspace, new CommandsPopupViewModel(_workspace, _workspace.Options, new MouseHandler(_workspace))).Execute();
                    break;
                case Key.Tab:
                    if ((modifierKeys & ModifierKeys.Shift) != 0)
                    {
                        new CycleTabsBackwardsCommand(_workspace).Execute();
                    }
                    else
                    {
                        new CycleTabsCommand(_workspace).Execute();
                    }
                    break;
                case Key.Return:
                    new DoActionCommand(_workspace).Execute();
                    break;
            }
            return false;
        }
    }
}