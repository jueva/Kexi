using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ChangeDocumentLaneCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ChangeDocumentLaneCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = _workspace.Docking.Files.SkipWhile(f => !f.IsActive).FirstOrDefault(f => f.IsSelected && !f.IsActive) 
                ?? _workspace.Docking.Files.FirstOrDefault(f => f.IsSelected && !f.IsActive);
            if (w != null)
                _workspace.ActiveLayoutContent = w;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}