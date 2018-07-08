using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(ConnectionLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectionLister : BaseLister<ConnectionItem>
    {
        [ImportingConstructor]
        public ConnectionLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository)
            : base(workspace, notificationHost, options, commandRepository)
        {
            Title = "Active Connections";
            PathName =  "connections://";
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Startpoint", "Startpoint"),
            new Column("Endpoint", "Endpoint"),
            new Column("Description", "Description", ColumnType.Text, ColumnSize.FullWidth)
        };

        protected override Task<IEnumerable<ConnectionItem>> GetItems()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var temp = properties.GetActiveTcpConnections().Select(l => new ConnectionItem(l));
            return Task.FromResult(temp);
        }

        public override void DoAction(ConnectionItem item)
        {
        }
    }
}

