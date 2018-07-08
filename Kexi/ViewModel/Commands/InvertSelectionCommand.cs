using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Common.MultiSelection;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class InvertSelectionCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public InvertSelectionCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var currentSelection = _workspace.ActiveLister.SelectedItems.ToArray();
            _workspace.ActiveLister.View.ListView.SelectAll();
            foreach (var item in currentSelection)
                _workspace.ActiveLister.SetSelection(item, false);
        }

        public event EventHandler CanExecuteChanged;
    }
}