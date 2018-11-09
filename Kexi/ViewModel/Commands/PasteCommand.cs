using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class PasteCommand : IKexiCommand
    {
        [ImportingConstructor]
        public PasteCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var handler = _workspace.ActiveLister as ICopyPasteHandler;
            handler?.Paste();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}