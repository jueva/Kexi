using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleHiddenItemsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ToggleHiddenItemsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.ShowHiddenItems = !_workspace.Options.ShowHiddenItems;
            var notification = "Show hidden items: " + (_workspace.Options.Highlights ? "on" : "off");
            _workspace.NotificationHost.AddInfo(notification);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}