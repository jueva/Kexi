using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowNetworkListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowNetworkListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var network = KexContainer.Resolve<NetworkLister>();
            _workspace.Open(network);
            await network.Refresh().ConfigureAwait(false);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}