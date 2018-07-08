using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.View;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ViewIconsCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ViewIconsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            new ViewHandler(_workspace.ActiveLister).SetIconView();
            _workspace.FocusCurrentOrFirst();
        }

        public event EventHandler CanExecuteChanged;
    }
}