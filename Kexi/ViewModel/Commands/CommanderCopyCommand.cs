using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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
    public class CommanderCopyCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CommanderCopyCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                var currentDoc = _workspace.ActiveLayoutContent;
                new CopyHelper(_workspace).CopySelectionToClipboard();
                var target = _workspace.CommanderTargetLayoutDocument;
                var lister = target.Content as FileLister;
                if (lister == null)
                    return;

                var items  = Clipboard.GetFileDropList();
                await Task.Factory.StartNew(() =>
                {
                    new FilesystemAction(_workspace.NotificationHost).Copy(lister.Path, items);
                });
                _workspace.ActiveLayoutContent = target;
                _workspace.FocusItem(_workspace.CurrentItems.FirstOrDefault(i => i.DisplayName == Path.GetFileName(items.Cast<string>().Last())));
                _workspace.ActiveLayoutContent = currentDoc;
            }
            catch (Exception ex)
            {
                _workspace.NotificationHost.AddError("Error Copying", ex);
            }
        }


        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}