using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSettingsListerCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowSettingsListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var settingsLister = KexContainer.Resolve<SettingsLister>();
            _workspace.Open(settingsLister);
            settingsLister.Refresh();
        }

        public event EventHandler CanExecuteChanged;
    }
}