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
        [Import]
        private Workspace Workspace { get; set; }

        private DispatcherTimer currentTimer;

        public NotificationHost()
        {
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
            currentTimer = new DispatcherTimer();
            currentTimer.Interval = TimeSpan.FromSeconds(3);
            currentTimer.Tick += (s, ea) =>
            {
                lister.Notification = null; currentTimer.Stop();
            };
            currentTimer.Start();
        }

        public void ClearCurrentMessage()
        {
            Workspace.ActiveLister.Notification = null;
            currentTimer?.Stop();
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
