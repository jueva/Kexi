using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Input;
using Kexi.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Extensions
{
    public class ProcessListerExtensions
    {
        [ImportingConstructor]
        public ProcessListerExtensions(Workspace workspace)
        {
            _workspace = workspace;
        }

        [ExportContextMenuCommand(typeof(ProcessLister), "Open in Process Explorer")]
        public ICommand OpenProcessExplorer
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var processExplorerPath = ConfigurationManager.AppSettings["ProcessExplorerPath"];
                    if (string.IsNullOrEmpty(processExplorerPath))
                        _workspace.NotificationHost.AddError("Path to Process Montior is not provided. (<add key=\"ProcessMonitorPath\" value=\"...\"/> in appSettings)");

                    if (_workspace.CurrentItem is ProcessItem processItem)
                    {
                        var psi = new ProcessStartInfo(processExplorerPath)
                        {
                            Arguments = "/s:" + processItem.Details.Pid
                        };
                        Process.Start(psi);
                    }
                });
            }
        }

        private readonly Workspace _workspace;
    }
}