using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ExecuteConsoleCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ExecuteConsoleCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var parameters = parameter as string;
            if (string.IsNullOrEmpty(parameters))
                return;

            var command = parameters;

            if (_workspace.ActiveLister is ConsoleLister fromLister)
            {
                fromLister.Command = command;
                fromLister.Refresh();
            }
            else
            {
                var console = KexContainer.Resolve<ConsoleLister>();
                if (_workspace.ActiveLister is FileLister fileLister)
                {
                    console.WorkingDirectory = _workspace.ActiveLister.Path;
                    console.Selection        = fileLister.SelectedItems;
                }

                console.Command = command;
                _workspace.Open(console);
                console.GotView += l => console.Refresh();
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}