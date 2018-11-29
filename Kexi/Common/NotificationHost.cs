using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using System.Windows.Threading;
using Kexi.Interfaces;

namespace Kexi.Common
{
    [Export(typeof(INotificationHost))]
    [Export(typeof(NotificationHost))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NotificationHost : INotificationHost
    {
        private Workspace Workspace { get; }

        private DispatcherTimer _currentTimer;

        [ImportingConstructor]
        public NotificationHost(Workspace workspace)
        {
            Workspace = workspace;
            _notifications = new ObservableCollection<NotificationItem>();
        }

        public void AddError(Exception ex)
        {
            AddError(ex.Message, ex);
        }

        public void AddError(string message, Exception ex)
        {

            AddError(message, ex.ToString());
        }

        public void AddError(string message, string detailMessage = null)
        {
            Add(message, detailMessage, NotificationType.Error);
        }

        public void AddInfo(string message, string detailMessage = null)
        {
            Add(message, detailMessage, NotificationType.Info);
        }

        public void Add(string message, string detailMessage = null, NotificationType notificationType = NotificationType.Info)
        {
            if (Workspace.ActiveLister == null)
                return;

            var item = new NotificationItem(message, detailMessage, notificationType);
            _notifications.Add(item);

            ClearCurrentMessage();
            var lister = Workspace.ActiveLister;
            lister.Notification = item;
            _currentTimer = new DispatcherTimer();
            _currentTimer.Interval = TimeSpan.FromSeconds(3);
            _currentTimer.Tick += (s, ea) =>
            {
                lister.Notification = null; _currentTimer.Stop();
            };
            _currentTimer.Start();
        }

        public void ClearCurrentMessage()
        {
            Workspace.ActiveLister.Notification = null;
            _currentTimer?.Stop();
        }

        public ObservableCollection<NotificationItem> Notifications
        {
            get
            {
                return _notifications;
            }
        }

        public void ClearNotifications()
        {
            _notifications.Clear();
        }

        private readonly ObservableCollection<NotificationItem> _notifications;

    }
}
