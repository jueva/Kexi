using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.View;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleViewModeCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ToggleViewModeCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.ActiveLister.CurrentViewMode == ViewType.Detail)
                new ViewHandler(_workspace.ActiveLister).SetThumbView();
            else
                new ViewHandler(_workspace.ActiveLister).SetDetailsView();
            _workspace.FocusCurrentOrFirst();
        }

        public event EventHandler CanExecuteChanged;
    }
}