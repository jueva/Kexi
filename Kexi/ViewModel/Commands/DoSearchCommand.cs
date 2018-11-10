using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DoSearchCommand : IKexiCommand
    {
        [ImportingConstructor]
        public DoSearchCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var searchPattern = parameter as string;
            if (string.IsNullOrEmpty(searchPattern))
            {
                _workspace.NotificationHost.AddError("Please enter Searchterm");
                return;
            }

            if (_workspace.ActiveLister is SearchLister searchLister)
            {
                searchLister.SearchPattern = (string) parameter;
                if (searchLister.View != null)
                    await searchLister.Refresh();
                else
                    searchLister.GotView += searchLister_GotView;
            }
            else
            {
                searchLister               = KexContainer.Resolve<SearchLister>();
                searchLister.Path          = _workspace.ActiveLister.Path;
                searchLister.SearchPattern = searchPattern;
                _workspace.Open(searchLister);
                searchLister.GotView += searchLister_GotView;
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;

        private void searchLister_GotView(ILister lister)
        {
            new SortHandler(lister).HandleSorting(new BaseItem("LastModified"), ListSortDirection.Descending);
            ((ICollectionViewLiveShaping) lister.ItemsView).IsLiveSorting = true;
            lister.Refresh();
        }
    }
}