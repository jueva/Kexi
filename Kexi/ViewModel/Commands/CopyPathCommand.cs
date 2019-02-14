using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Interfaces;


namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CopyPathCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CopyPathCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var items = _workspace.ActiveLister.SelectedItems.Select(i => i.Path);
            var text  = string.Join(Environment.NewLine, items);
            Clipboard.SetText(text);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}
