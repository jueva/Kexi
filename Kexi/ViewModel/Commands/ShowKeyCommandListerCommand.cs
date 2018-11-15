using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowKeyCommandListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowKeyCommandListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var lister = KexContainer.Resolve<KeyCommandsLister>();
            _workspace.Open(lister);
            await lister.Refresh().ConfigureAwait(false);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}