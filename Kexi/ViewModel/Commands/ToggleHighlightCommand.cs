using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleHighlightCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ToggleHighlightCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.Highlights = !_workspace.Options.Highlights;
            var notification = "Highlighting turned " + (_workspace.Options.Highlights ? "on" : "off");
            _workspace.NotificationHost.AddInfo(notification);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}