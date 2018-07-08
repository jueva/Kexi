using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CopyCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public CopyCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is ICopyPasteHandler;
        }

        public void Execute(object parameter)
        {
            if (_workspace.ActiveLister is ICopyPasteHandler handler) 
                handler.Copy();
        }

        public event EventHandler CanExecuteChanged;
    }
}