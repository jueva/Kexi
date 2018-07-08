using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class AvalonDumpManagerCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public AvalonDumpManagerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //_workspace.ActiveLayoutDocument.ConsoleDump(0);
            _workspace.NotificationHost.AddInfo("Avalonstructure dumped");

        }

        public event EventHandler CanExecuteChanged;
    }
}