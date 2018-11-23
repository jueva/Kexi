using System;
using System.ComponentModel.Composition;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CyclePanelSizeCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CyclePanelSizeCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var half = Application.Current.MainWindow.ActualWidth / 2;
            _workspace.Docking.DockWidth = Math.Abs(_workspace.Docking.DockWidth.Value - 250) < 0.1 ? new GridLength(half) : new GridLength(250);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}