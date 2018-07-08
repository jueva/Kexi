using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DoActionCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public DoActionCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var item = _workspace.ActiveLister.CurrentItem;
            _workspace.ActiveLister.DoAction(item);
        }

        public event EventHandler CanExecuteChanged;
    }
}