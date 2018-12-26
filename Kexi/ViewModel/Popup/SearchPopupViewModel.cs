using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SearchPopupViewModel : PopupViewModel<BaseItem>
    {
        private readonly DoSearchCommand _searchCommand;

        [ImportingConstructor]
        public SearchPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler, DoSearchCommand searchCommand) : base(workspace, options, mouseHandler)
        {
            _searchCommand = searchCommand;
            TitleVisible       = true;
            Title              = "Search";
            HideInputAtStartup = false;
            SetHeaderIconByKey("appbar_magnify");
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            //Dont Filter List Items
        }

        protected override void ItemSelected(BaseItem selectedItem)
        {
            try
            {
                var searchText = Text;
                IsOpen = false;
                Workspace.FocusCurrentOrFirst();
                _searchCommand.Execute(searchText);
            }
            catch (Exception ex)
            {
                Workspace.NotificationHost.AddError("Error creating Item", ex);
            }
        }
    }
}