using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Common;
using Kexi.Files;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SpecialPastePopupViewModel : PopupViewModel<BaseItem>
    {
        private const    string    NoConfirmation   = "No Confirmations";
        private const    string    RenameOnConflict = "Rename On Conflict";
        private const    string    NormalPaste      = "Normal Paste";
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public SpecialPastePopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            _workspace         = workspace;
            TitleVisible       = true;
            HideInputAtStartup = false;
            Title              = "Paste from Clipboard";
        }

        public override void Open()
        {
            BaseItems = new[]
            {
                new BaseItem(NoConfirmation),
                new BaseItem(RenameOnConflict),
                new BaseItem(NormalPaste)
            };
            SetHeaderIconByKey("appbar_layer_add");
            base.Open();
        }

        public override void ItemSelected(BaseItem selectedItem)
        {
            short flags = ShellNative.FOF_ALLOWUNDO;
            switch (selectedItem.DisplayName)
            {
                case NoConfirmation:
                    flags |= ShellNative.FOF_NOCONFIRMATION;
                    break;
                case RenameOnConflict:
                    flags |= ShellNative.FOF_RENAMEONCOLLISION;
                    break;
                default:
                    break;
            }

            var fileAction       = FilesystemAction.LastFileAction;
            var filesystemAction = new FilesystemAction(_workspace.NotificationHost);
            var items            = Clipboard.GetFileDropList();
            Task.Factory.StartNew(() => { filesystemAction.Paste(_workspace.ActiveLister.Path, items, fileAction, flags); });
            if (fileAction == FileAction.Move)
                Clipboard.Clear();
        }
    }
}