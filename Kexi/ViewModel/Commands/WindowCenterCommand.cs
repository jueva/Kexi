using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class WindowCenterCommand : IKexiCommand
    {
        [ImportingConstructor]
        public WindowCenterCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            UIHelper.CenterToScreen(UIHelper.GetCurrentScreen());
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}