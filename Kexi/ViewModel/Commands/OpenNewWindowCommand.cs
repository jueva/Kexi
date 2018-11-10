using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class OpenNewWindowCommand : IKexiCommand
    {
        [ImportingConstructor]
        public OpenNewWindowCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var exeName   = Process.GetCurrentProcess().MainModule.FileName;
            var startInfo = new ProcessStartInfo(exeName);
            Process.Start(startInfo);
        }

        public event EventHandler CanExecuteChanged;
    }
}