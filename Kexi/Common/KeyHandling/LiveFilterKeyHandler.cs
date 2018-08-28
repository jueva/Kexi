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
        public LiveFilterKeyHandler(Workspace workspace, List<KexBinding> bindings)
        {
            _workspace      = workspace;
            Bindings        = bindings;
            _bindingHandler = new BindingHandler(workspace, bindings);
        }

        public List<KexBinding> Bindings { get; }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            if (_bindingHandler.Handle(args, lister, group))
                return true;

            if (args.Key == Key.Return || args.Key == Key.Escape)
            {
                _workspace.PopupViewModel.Close();
            }

            var modifierKeys = args.KeyboardDevice.Modifiers;
            if (modifierKeys == ModifierKeys.None && args.Key >= Key.A && args.Key <= Key.Z)
            {
                var k = args.Key.ToString().ToLower()[0];
                _fileFilterPopupView = new FileFilterPopupViewModel(_workspace, _workspace.Options, null);
                _filterPopupView     = new FilterPopupViewModel(_workspace, _workspace.Options, null);
                new ShowFilterPopupCommand(_workspace, _fileFilterPopupView, _filterPopupView).Execute();
                args.Handled = false;
                return true;
            }
            return false;
        }

        public           string                   SearchString { get; set; }
        private readonly BindingHandler           _bindingHandler;
        private readonly Workspace                _workspace;
        private          FileFilterPopupViewModel _fileFilterPopupView;
        private          FilterPopupViewModel     _filterPopupView;
    }
}