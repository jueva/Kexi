using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Security.Principal;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class RestartAsAdminCommand : IKexiCommand
    {
        [ImportingConstructor]
        public RestartAsAdminCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var identity  = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var admin     = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!admin)
            {
                var exeName   = Process.GetCurrentProcess().MainModule.FileName;
                var startInfo = new ProcessStartInfo(exeName) {Verb = "runas"};
                Process.Start(startInfo);
            }
            else
            {
                _workspace.NotificationHost.AddError("Already Admin");
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}