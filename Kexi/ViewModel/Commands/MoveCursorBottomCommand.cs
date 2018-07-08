using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveCursorBottomCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public MoveCursorBottomCommand(Workspace workspace)
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
            var items = _workspace.ActiveLister.ItemsView.Cast<IItem>().ToArray();
            _workspace.ActiveLister.View.ListView.SelectedIndex = items.Length - 1;
            _workspace.FocusItem(items.LastOrDefault());
            _workspace.ActiveLister.View.ListView.SelectionMode = SelectionMode.Extended;
        }

        public event EventHandler CanExecuteChanged;
    }
}