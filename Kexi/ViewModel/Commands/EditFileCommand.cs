using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class EditFileCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public EditFileCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var arguments = _workspace.ActiveLister.SelectedItems.Select(i => i.Path);
            var argumentString = string.Join(" ", arguments);
            Process.Start(_workspace.Options.PreferredEditorLocation, argumentString);
 
        }

        public event EventHandler CanExecuteChanged;
    }
}