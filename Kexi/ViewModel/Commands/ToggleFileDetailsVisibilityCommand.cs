using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleFileDetailsVisibilityCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ToggleFileDetailsVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (_workspace.Docking.DetailViewModel == null)
                return;

            if (_workspace.Docking.DetailViewModel.IsVisible && !_workspace.Docking.DetailViewModel.IsSelected)
            {
                _workspace.Docking.DetailViewModel.IsSelected = true;
                return;
            }

            _workspace.Docking.DetailViewModel.IsVisible = !_workspace.Docking.DetailViewModel.IsVisible;
            if (_workspace.Docking.DetailViewModel.IsVisible)
            {
                await _workspace.ActiveLister.View.ShowDetail();
                _workspace.Docking.DetailViewModel.IsSelected = true;
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}