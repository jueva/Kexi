using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.View;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class ViewPopupViewModel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public ViewPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title       = "Views";
            _viewHandler = new ViewHandler(Workspace.ActiveLister);
        }

        public override void Open()
        {
            BaseItems   = ViewHandler.Views.Select(i => new BaseItem(i));
            base.Open();
        }

        private readonly ViewHandler _viewHandler;

        protected override void ItemSelected(IItem selectedItem)
        {
            base.ItemSelected(selectedItem);
            SetView(selectedItem.DisplayName);
            IsOpen = false;
        }

        private void SetView(string displayName)
        {
            switch (displayName)
            {
                case "Thumbs":
                    _viewHandler.SetThumbView();
                    break;
                case "Icons":
                    _viewHandler.SetIconView();
                    break;
                default:
                    _viewHandler.SetDetailsView();
                    break;
            }
        }
    }
}