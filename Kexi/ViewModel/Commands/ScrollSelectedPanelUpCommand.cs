using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Interfaces;
using Kexi.ViewModel.Dock;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ScrollSelectedPanelUpCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ScrollSelectedPanelUpCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var models =new ToolViewModel[]
            {
                _workspace.Docking.DetailViewModel, 
                _workspace.Docking.ExplorerViewModel, 
                _workspace.Docking.PreviewViewModel
            };
            models.FirstOrDefault(m => m.IsVisible && m.IsSelected)?.ScrollUp();;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}