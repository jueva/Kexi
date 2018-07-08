using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class SelectAllOfSameType : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public SelectAllOfSameType(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CurrentItem is FileItem;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            if (_workspace.CurrentItem is FileItem currentItem)
            {
                _workspace.ActiveLister.View.ListView.SelectedIndex = -1;
                var sameType = _workspace.CurrentItemsOfType<FileItem>().Where(f => f.Extension == currentItem.Extension);
                foreach (var item in sameType)
                    _workspace.ActiveLister.SetSelection(item, true);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}