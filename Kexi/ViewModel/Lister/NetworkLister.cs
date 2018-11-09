using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{ 
    [Export(typeof(ILister))]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NetworkLister : BaseLister<NetworkItem>
    {
        [ImportingConstructor]
        public NetworkLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository) : base(workspace, notificationHost, options, commandRepository)
        {
            Title = PathName = "Network";
        }

        public override IEnumerable<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("Name", "DisplayName") {Width = 300}
            };
        protected override async Task<IEnumerable<NetworkItem>> GetItems()
        {
            var browser = new NetworkBrowser();
            var computers = await Task.Run(()=>browser.GetNetworkComputers());
            return computers.Select(i => new NetworkItem(i));
        }

        public override string ProtocolPrefix => "Network";

        public override async void DoAction(NetworkItem item)
        {
            var lister = KexContainer.Resolve<FileLister>();
            lister.Path = @"\\" + item.DisplayName;
            Workspace.ReplaceCurrentLister(lister);
            await lister.Refresh();
        }
    }

    public class NetworkItem : BaseItem
    {
        public NetworkItem(string name) : base(name)
        {
        }
    }
}
