using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowLatestItemsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowLatestItemsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!(_workspace.CurrentItem is FileItem item))
                return;

            var    searchLister = KexContainer.Resolve<SearchLister>();
            string target;
            if (item.IsLink())
                target = new FileItemTargetResolver(item).TargetPath;
            else if (item.ItemType == ItemType.Item)
                target = _workspace.ActiveLister.Path;
            else
                target = item.Path;

            searchLister.Path          = target;
            searchLister.SearchPattern = "";
            searchLister.MaxItems      = 20;
            _workspace.Open(searchLister);
            searchLister.GotView += SearchLister_GotView;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;

        private static void SearchLister_GotView(ILister lister)
        {
            new SortHandler(lister).HandleSorting(new BaseItem("LastModified"), ListSortDirection.Descending);
            ((ICollectionViewLiveShaping) lister.ItemsView).IsLiveSorting = true;
            lister.Refresh();
        }
    }
}