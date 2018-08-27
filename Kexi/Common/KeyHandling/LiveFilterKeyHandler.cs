using System.Collections.Generic;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;

namespace Kexi.Common.KeyHandling
{
    public class LiveFilterKeyHandler : IKeyHandler
    {
        private readonly Workspace _workspace;
        private  FileFilterPopupViewModel _fileFilterPopupView;
        private  FilterPopupViewModel _filterPopupView;


        public LiveFilterKeyHandler(Workspace workspace)
        {
            _workspace = workspace;

        }

        public List<KexBinding> Bindings { get; set; }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            var modifierKeys = args.KeyboardDevice.Modifiers;
            switch (args.Key)
            {
                case Key.Escape:
                    new ClearFilterCommand(_workspace).Execute();
                    _workspace.PopupViewModel.Close();
                    break;
                case Key.F1:
                    new ShowCommandsPopupCommand(_workspace, new CommandsPopupViewModel(_workspace, _workspace.Options, new MouseHandler(_workspace))).Execute();
                    break;
                case Key.F4:
                    if ((modifierKeys & ModifierKeys.Control) != 0)
                        new WindowCloseCommand(_workspace).Execute();
                    break;
                case Key.Back:
                    new HistoryBackCommand(_workspace).Execute();
                    break;
                case Key.Tab:
                    if ((modifierKeys & ModifierKeys.Shift) != 0)
                        new CycleTabsBackwardsCommand(_workspace).Execute();
                    else
                        new CycleTabsCommand(_workspace).Execute();
                    break;
                case Key.Return:
                    new DoActionCommand(_workspace).Execute();
                    _workspace.PopupViewModel.Close();
                    break;
                default:
                    var k = args.Key.ToString().ToLower()[0];
                    if (modifierKeys == ModifierKeys.None && k >= 'a' && k <= 'z')
                    {
                        _fileFilterPopupView = new FileFilterPopupViewModel(_workspace, _workspace.Options, null);
                        _filterPopupView     = new FilterPopupViewModel(_workspace, _workspace.Options, null);
                        new ShowFilterPopupCommand(_workspace, _fileFilterPopupView, _filterPopupView).Execute();
                        args.Handled = false;
                        return true;
                    }
                    break;
            }

            args.Handled = true;
            return false;
        }

        public string SearchString { get; set; }
    }
}