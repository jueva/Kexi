using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel
{
    [Export(typeof(ILister))]
    [Export(typeof(ServiceLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ServiceLister : BaseLister<ServiceItem>
    {
        private static readonly Lazy<BitmapImage> Thumb = new Lazy<BitmapImage>(() => Utils.GetImageFromRessource("service.png"));

        [ImportingConstructor]
        public ServiceLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandrepository)
            : base(workspace, notificationHost, options, commandrepository)
        {
            Title     =  PathName = Path = "Services";
            Thumbnail =  Thumb.Value;
            GotItems  += ServiceLister_GotItems;
        }

        [ExportContextMenuCommand(typeof(ServiceLister), "Stop Service")]
        public RelayCommand StopCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var items = Workspace.GetSelection<ServiceItem>();
                    foreach (var item in items)
                        stopService(item);
                }, _ => Workspace.GetSelection<ServiceItem>().Any(i => i.Status == ServiceControllerStatus.Running));
            }
        }

        [ExportContextMenuCommand(typeof(ServiceLister), "Start Service")]
        public RelayCommand StartCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var items = Workspace.GetSelection<ServiceItem>();
                    foreach (var item in items)
                        startService(item);
                }, _ => Workspace.GetSelection<ServiceItem>().Any(i => i.Status == ServiceControllerStatus.Stopped));
            }
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "Thumbnail", ColumnType.Image),
            new Column("Name", "DisplayName") {Width        = 300},
            new Column("Description", "Description") {Width = 300},
            new Column("Status", "Status") {Width           = 300}
        };

        public override string ProtocolPrefix => "services";

        private void ServiceLister_GotItems(object sender, EventArgs e)
        {
            ItemsView.CurrentChanged += ItemsView_CurrentChanged;
        }


        private async void startService(ServiceItem item)
        {
            await Task.Run(() =>
            {
                if (item.Status == ServiceControllerStatus.Stopped)
                {
                    item.ServiceController.Start();
                    item.Status = ServiceControllerStatus.StartPending;
                    item.ServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    item.Status = item.ServiceController.Status;
                }
            });
        }

        private async void stopService(ServiceItem item)
        {
            LoadingStatus = LoadingStatus.Loading;
            await Task.Run(() =>
            {
                if (item.ServiceController.CanStop && item.Status != ServiceControllerStatus.Stopped)
                {
                    item.ServiceController.Stop();
                    item.Status = ServiceControllerStatus.StopPending;
                    item.ServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    item.Status = item.ServiceController.Status;
                }
            });
            LoadingStatus = LoadingStatus.Loaded;
        }

        protected override async Task<IEnumerable<ServiceItem>> GetItems()
        {
            var services = await GetServices();
            return services;
        }

        private void ItemsView_CurrentChanged(object sender, EventArgs e)
        {
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        public override void DoAction(ServiceItem item)
        {
        }

        private async Task<IEnumerable<ServiceItem>> GetServices()
        {
            var services = await Task.Run(() => ServiceController.GetServices());
            return services.Select(s => new ServiceItem(s));
        }
    }
}