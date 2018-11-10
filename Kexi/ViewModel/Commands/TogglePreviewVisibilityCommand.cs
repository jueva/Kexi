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
            if (_workspace.Docking.PreviewViewModel.IsVisible && !_workspace.Docking.PreviewViewModel.IsSelected)
            {
                _workspace.Docking.PreviewViewModel.IsSelected = true;
                return;
            }

            _workspace.Docking.PreviewViewModel.IsVisible = !_workspace.Docking.PreviewViewModel.IsVisible;
            if (_workspace.Docking.PreviewViewModel.IsVisible)
            {
                _workspace.Docking.PreviewViewModel.IsSelected = true;
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}