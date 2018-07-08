using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveCursorTopCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public MoveCursorTopCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.ActiveLister.View.ListView.SelectionMode = SelectionMode.Single;
            var items                                           = _workspace.ActiveLister.ItemsView.Cast<IItem>();
            _workspace.ActiveLister.View.ListView.SelectedIndex = 0;
            _workspace.FocusItem(items.FirstOrDefault());
            _workspace.ActiveLister.View.ListView.SelectionMode = SelectionMode.Extended;
        }

        public event EventHandler CanExecuteChanged;
    }
}