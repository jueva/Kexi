using System;
using System.ComponentModel.Composition;
using Kex.Controller.Popups;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowNetworkComputersPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowNetworkComputersPopupCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var networkPopup         = new NetworkComputersPopupViewModel();
            _workspace.PopupViewModel = networkPopup;
            networkPopup.IsOpen      = true;
        }

        public event EventHandler CanExecuteChanged;
    }
}