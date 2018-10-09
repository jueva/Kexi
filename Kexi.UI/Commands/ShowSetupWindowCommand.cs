using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel;

namespace Kexi.UI.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSetupWindowCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowSetupWindowCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var setup = new SetupWindow(_workspace);
            setup.ShowDialog();
            _workspace.FocusListView();
        }

        public event EventHandler CanExecuteChanged;
    }
}
