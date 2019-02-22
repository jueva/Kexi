using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(NotificationLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NotificationLister : BaseLister<NotificationItem>
    {
        [ImportingConstructor]
        public NotificationLister(Workspace workspace, Options options, CommandRepository commandRepository)
            : base(workspace,  options, commandRepository)
        {
            Title                                            =  PathName = "Notifications";
        }

        public override ObservableCollection<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("DisplayName", "DisplayName", ColumnType.Highlightable) {Width = 600},
                new Column("Has Details", "HasDetails", ColumnType.Bool) {Width           = 65},
                new Column("Created", "Created") {Width                                   = 133}
            };


        [ExportContextMenuCommand(typeof(NotificationLister), "Expand All")]
        public ICommand ExpandAllCommand
        {
            get { return _expandAllCommand ?? (_expandAllCommand = new RelayCommand(c => NotificationHost.Notifications.Foreach(n => n.DisplayDetails = true))); }
        }

        [ExportContextMenuCommand(typeof(NotificationLister), "Collapse All")]
        public ICommand CollapseAllCommand
        {
            get { return _collapseAllCommand ?? (_collapseAllCommand = new RelayCommand(c => NotificationHost.Notifications.Foreach(n => n.DisplayDetails = false))); }
        }

        [ExportContextMenuCommand(typeof(NotificationLister), "Clear Notifications")]
        public ICommand ClearAllNotificationsCommand
        {
            get
            {
                return _clearAllNotificationsCommand ?? (_clearAllNotificationsCommand = new RelayCommand(async c =>
                {
                    NotificationHost.Notifications.Clear();
                    await Refresh().ConfigureAwait(false);
                }));
            }
        }

        public override string   ProtocolPrefix => "Notifications";
        private         ICommand _clearAllNotificationsCommand;
        private         ICommand _collapseAllCommand;
        private         ICommand _expandAllCommand;

        protected override Task<IEnumerable<NotificationItem>> GetItems()
        {
            var items = NotificationHost.Notifications.Any(n => n.IsError)
                ? new ObservableCollection<NotificationItem>(NotificationHost.Notifications.Where(n => n.IsError).OrderByDescending(n => n.Created))
                : new ObservableCollection<NotificationItem>(new[] {new NotificationItem("No Notifications")});
            return Task.FromResult(items.AsEnumerable());
        }

        public override void DoAction(NotificationItem selection)
        {
            if (selection == null)
                return;

            selection.DisplayDetails = !selection.DisplayDetails;
        }
    }
}