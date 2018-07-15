using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SynchronizeFilenamesPopupViewModel : PopupViewModel<FileItem>
    {
        [ImportingConstructor]
        public SynchronizeFilenamesPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title                 = "Sync Filenames";
            SetHeaderIconByKey("appbar_arrow_left_right");
        }

        public override void Open()
        {
            BaseItems = Workspace.ActiveLister.SelectedItems.OfType<FileItem>().Where(i => i.ItemType == ItemType.Item);
            base.Open();
        }


        protected override void ItemSelected(FileItem selected)
        {
            try
            {
                var baseName = Path.GetFileNameWithoutExtension(selected.Path);
                foreach (var item in BaseItems)
                {
                    var di     = new FileInfo(item.Path);
                    var parent = di.Directory;
                    var dest   = Path.Combine(parent.FullName, baseName + item.Extension);
                    di.MoveTo(dest);
                }

                IsOpen = false;
                base.ItemSelected(selected);
            }
            catch (Exception ex)
            {
                Workspace.NotificationHost.AddError(ex);
            }
        }
    }
}