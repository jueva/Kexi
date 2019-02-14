using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Shell;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class DrivePopupViewModel : PopupViewModel<DriveItem>
    {
        [ImportingConstructor]
        public DrivePopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            HideInputAtStartup = true;
            Title              = "Drives";
        }

        public override void Open()
        {
            BaseItems = getDriveItems();
            SetHeaderIconByKey("appbar_box_layered");
            Initialize();
            base.Open();
        }

        private async void Initialize()
        {
            foreach (var d in BaseItems)
                await getDetails(d).ConfigureAwait(false);
        }

        private ObservableCollection<DriveItem> getDriveItems()
        {
            var drives = Directory.GetLogicalDrives();
            return new ObservableCollection<DriveItem>(drives.Select(d => new DriveItem(d)));
        }

        private Task getDetails(DriveItem drive)
        {
            return Task.Factory.StartNew(() =>
            {
                var info = new DriveInfo(drive.DisplayName);
                drive.Path        = info.RootDirectory.FullName;
                drive.IsReady     = info.IsReady;
                drive.DisplayName = drive.FilterString = GetDriveInfoString(info);
                var tmb                                = ShellNative.GetLargeBitmapSource(drive.Path);
                tmb.Freeze();
                drive.Thumbnail = tmb;
            });
        }

        protected override async void ItemSelected(DriveItem selectedItem)
        {
            if (selectedItem == null)
                return;
            if (!selectedItem.IsReady)
            {
                Workspace.NotificationHost.AddError(string.Format("Drive {0} ist not ready", selectedItem.DriveLetter));
                return;
            }

            IsOpen = false;
            if (Workspace.ActiveLister is FileLister)
            {
                Workspace.ActiveLister.Path = selectedItem.Path;
                await Workspace.ActiveLister.Refresh();
            }
            else
            {
                var lister = KexContainer.Resolve<FileLister>();
                Workspace.Open(lister);
                await lister.Refresh();
            }

            base.ItemSelected(selectedItem);
        }

        private static string GetDriveInfoString(DriveInfo di)
        {
            if (!di.IsReady)
                return $"{di.Name} - not ready";

            try
            {
                return $"{di.Name} {di.VolumeLabel} ({di.TotalFreeSpace / 1000000000}/{di.TotalSize / 1000000000} GB free)";
            }
            catch
            {
                return di.Name;
            }
        }
    }
}