using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class AdjustListerWidthCommand : IKexiCommand
    {
        [ImportingConstructor]
        public AdjustListerWidthCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var ui = new UIHelper(_workspace);
            ui.FitWidth();
            ui.FitWidth();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}