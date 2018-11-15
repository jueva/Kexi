using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleTreeViewVisibilityCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ToggleTreeViewVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.Docking.ExplorerViewModel == null)
                return;

            if (!_workspace.Docking.ExplorerViewModel.IsVisible)
            {
                _workspace.Docking.ExplorerViewModel.IsVisible = true;
                _workspace.Docking.PreviewViewModel.IsActive = false;
                _workspace.Docking.ExplorerViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.ExplorerViewModel.IsVisible && !_workspace.Docking.ExplorerViewModel.IsSelected)
            {
                _workspace.Docking.ExplorerViewModel.IsActive = true;
                _workspace.Docking.ExplorerViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.ExplorerViewModel.IsVisible && _workspace.Docking.ExplorerViewModel.IsSelected)
            {
                _workspace.Docking.ExplorerViewModel.IsVisible = false;
                _workspace.Docking.ExplorerViewModel.IsSelected = false;
            }

        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}