using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Kexi.Interfaces;
using Shell32;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowFilePropertiesCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowFilePropertiesCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var shell       = new Shell32.Shell();
            var path = _workspace.CurrentItem.Path;
            var dir         = Path.GetDirectoryName(path);
            var file        = Path.GetFileName(path);
            var shellFolder = shell.NameSpace(dir) as Folder3;
            var verbs      = shellFolder?.ParseName(file)?.Verbs().Cast<FolderItemVerb>();
            var properties = verbs?.FirstOrDefault(fi => fi.Name == "P&roperties" || fi.Name == "E&igenschaften"); //TODO: avoid this hack
            properties?.DoIt();
        }

        public event EventHandler CanExecuteChanged;
    }
}