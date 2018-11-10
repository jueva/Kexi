using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowDriveListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowDriveListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var drives = KexContainer.Resolve<DriveLister>();
            _workspace.ReplaceCurrentLister(drives);
            _workspace.ActiveLister.Refresh();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}