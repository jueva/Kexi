using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CopyCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CopyCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.ActiveLister is ICopyPasteHandler handler)
            {
                handler.Copy();
            }
            else
            {
                var items = _workspace.ActiveLister.SelectedItems.Select(i => i.DisplayName);
                var text  = string.Join(",", items);
                Clipboard.SetText(text);
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}