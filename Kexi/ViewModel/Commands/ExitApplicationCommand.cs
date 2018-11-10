using System;
using System.ComponentModel.Composition;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ExitApplicationCommand : IKexiCommand
    {

        [ImportingConstructor]
        public ExitApplicationCommand()
        {
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