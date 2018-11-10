using System;
using System.ComponentModel.Composition;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class IncreaseFontSizeCommand : IKexiCommand
    {
        [ImportingConstructor]
        public IncreaseFontSizeCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.FontSize++;
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.FontSize = _workspace.Options.FontSize;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}