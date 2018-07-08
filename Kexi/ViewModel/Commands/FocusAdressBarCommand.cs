using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class FocusAdressBarCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public FocusAdressBarCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.AdressbarVisible = true;
            _workspace.BreadcrumbViewModel.Mode = BreadcrumbMode.Adressbox;
        }

        public event EventHandler CanExecuteChanged;
    }
}