using System;
using System.ComponentModel.Composition;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class QuitCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public QuitCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown(); 
        }

        public event EventHandler CanExecuteChanged;
    }
}