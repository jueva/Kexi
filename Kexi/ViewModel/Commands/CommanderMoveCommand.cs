using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CommanderMoveCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CommanderMoveCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                new CopyHelper(_workspace).CopySelectionToClipboard();
                var target = _workspace.CommanderTargetLayoutDocument;
                if (!(target.Content is FileLister lister) || !Clipboard.ContainsFileDropList()) return;

                var items = Clipboard.GetFileDropList();
                Task.Factory.StartNew(() => { new FilesystemAction(_workspace.NotificationHost).Move(lister.Path, items); });
                Clipboard.Clear();
                _workspace.FocusCurrentOrFirst();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Moving", ex);
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}