using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowNotificationListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowNotificationListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var notifications = KexContainer.Resolve<NotificationLister>();
            _workspace.Open(notifications);
            await notifications.Refresh().ConfigureAwait(false);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}