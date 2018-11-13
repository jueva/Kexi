using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class UndoCommand : IKexiCommand
    {
        [ImportingConstructor]
        public UndoCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
            _workspace.NotificationHost.AddInfo("Undo actually not supported. To  undo last fileaction open windows explorer and press ctrl+z");
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}