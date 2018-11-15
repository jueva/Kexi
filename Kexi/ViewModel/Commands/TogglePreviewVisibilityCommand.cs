using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class TogglePreviewVisibilityCommand : IKexiCommand
    {
        [ImportingConstructor]
        public TogglePreviewVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.Docking.PreviewViewModel == null)
                return;

            if (!_workspace.Docking.PreviewViewModel.IsVisible)
            {
                _workspace.Docking.PreviewViewModel.IsVisible = true;
                _workspace.Docking.PreviewViewModel.IsActive = false;
                _workspace.Docking.PreviewViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.PreviewViewModel.IsVisible && !_workspace.Docking.PreviewViewModel.IsSelected)
            {
                _workspace.Docking.PreviewViewModel.IsActive = true;
                _workspace.Docking.PreviewViewModel.IsSelected = true;
                return;
            }

            if (_workspace.Docking.PreviewViewModel.IsVisible && _workspace.Docking.PreviewViewModel.IsSelected)
            {
                _workspace.Docking.PreviewViewModel.IsVisible = false;
                _workspace.Docking.PreviewViewModel.IsSelected = false;
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}