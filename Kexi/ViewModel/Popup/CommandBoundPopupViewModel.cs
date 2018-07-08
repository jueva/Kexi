using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommandBoundPopupViewModel : PopupViewModel<CommandBoundItem>
    {
        [ImportingConstructor]
        public CommandBoundPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Actions";
        }

        public override void Open()
        {
            BaseItems = Workspace.ActiveLister.ContextMenuItems;
            SetHeaderIconByKey("appbar_console");
            foreach (var command in BaseItems) command.Enabled = command.Command.CanExecute(null);
            base.Open();
        }

        public override void ItemSelected(CommandBoundItem item)
        {
            IsOpen = false;
            if (item != null)
            {
                if (!item.Enabled)
                    return;

                if (item.Command.CanExecute(null))
                    item.Command.Execute(Workspace.ActiveLister);
            }

            base.ItemSelected(item);
        }
    }
}