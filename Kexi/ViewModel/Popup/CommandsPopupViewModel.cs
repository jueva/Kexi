using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class CommandsPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public CommandsPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title     = "Drives";
        }

        public override void Open()
        {
            BaseItems = CommandRepository.CommandCache.Value.Select(c => new BaseItem(c.Key));
            SetHeaderIconByKey("appbar_control_guide");
            base.Open();
        }

        public override void ItemSelected(BaseItem selectedItem)
        {
            var command = CommandRepository.GetCommandByName(selectedItem.DisplayName);
            IsOpen = false;
            if (command.CanExecute(null))
                command.Execute(null);
        }
    }
}