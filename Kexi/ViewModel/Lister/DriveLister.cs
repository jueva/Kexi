using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(DriveLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class DriveLister : BaseLister<DriveItem>
    {
        [ImportingConstructor]
        public DriveLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository) : base(workspace, notificationHost, options, commandRepository)
        {
            Title = PathName = Path = "Drives";
        }

        public override IEnumerable<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("", "Thumbnail", ColumnType.Image),
                new Column("Name", "DisplayName", ColumnType.Highlightable) {Width = 300}
            };


        [ExportContextMenuCommand(typeof(DriveLister), "Properties")]
        public ICommand PropertiesCommand => new RelayCommand(c => { ShellNative.ShowFileProperties(Workspace.CurrentItem.Path); });

        protected override Task<IEnumerable<DriveItem>> GetItems()
        {
            GotItems += DriveLister_GotItems;
            var it = GetDriveItems();
            return Task.FromResult(it.AsEnumerable());
        }
        private async void DriveLister_GotItems(object sender, System.EventArgs e)
        {
            await Initialize(Items);
            Thumbnail =  Items?.FirstOrDefault()?.Thumbnail;
            GotItems  -= DriveLister_GotItems;
        }

        private async Task Initialize(IEnumerable<DriveItem> it)
        {
            foreach (var d in it)
                await GetDetails(d);
        }

        private static List<DriveItem> GetDriveItems()
        {
            var drives = Directory.GetLogicalDrives();
            return new List<DriveItem>(drives.Select(d => new DriveItem(d)));
        }

        private async Task GetDetails(DriveItem drive)
        {
            await Task.Factory.StartNew(() =>
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

        public override void DoAction(DriveItem item)
        {
            var fi = KexContainer.Resolve<FileLister>();
            fi.Path = item?.Path;
            Workspace.ReplaceCurrentLister(fi);
            fi.Refresh();
        }

        public override string ProtocolPrefix => "Drives";
    }
}