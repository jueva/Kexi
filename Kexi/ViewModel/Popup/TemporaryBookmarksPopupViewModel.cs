using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class TemporaryFavoritesPopupViewModel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public TemporaryFavoritesPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Temporary Favorites";
            SetHeaderIconByKey("appbar_app_favorite");
        }

        public override void Open()
        {
            BaseItems = Workspace.TemporaryFavorites.Favorites;
            base.Open();
        }

        protected override void ItemSelected(IItem selectedItem)
        {
            if (selectedItem is FileItem fileItem)
            {
                if (fileItem.ItemType == ItemType.Item)
                    new FileListerAction(Workspace, fileItem).DoAction();
                else
                {
                    Workspace.ActiveLister.Path = fileItem.Path;
                    Workspace.ActiveLister.Refresh();
                }
                base.ItemSelected(selectedItem);
                Text   = "";
                IsOpen = false;
            }
        }
    }
}