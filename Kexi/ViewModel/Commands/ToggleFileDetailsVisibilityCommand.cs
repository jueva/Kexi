using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleFileDetailsVisibilityCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ToggleFileDetailsVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.Docking.DetailViewModel == null)
                return;

            if (!_workspace.Docking.DetailViewModel.IsVisible)
            {
                _workspace.Docking.DetailViewModel.IsVisible  = true;
                _workspace.Docking.DetailViewModel.IsActive = false;
                _workspace.Docking.DetailViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.DetailViewModel.IsVisible && !_workspace.Docking.DetailViewModel.IsSelected)
            {
                _workspace.Docking.DetailViewModel.IsActive = true;
                _workspace.Docking.DetailViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.DetailViewModel.IsVisible && _workspace.Docking.DetailViewModel.IsSelected)
            {
                _workspace.Docking.DetailViewModel.IsVisible  = false;
                _workspace.Docking.DetailViewModel.IsSelected = false;
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}