using System;
using System.Collections.ObjectModel;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.Interfaces
{
    public interface INotificationHost
    {
        void AddError(Exception ex);
        void AddError(string message, Exception ex);
        void AddError(string message, string detailMessage = null);
        void AddInfo(string message, string detailMessage = null);
        void Add(string message, string detailMessage = null, NotificationType notificationType = NotificationType.Info);
        void ClearCurrentMessage();
        ObservableCollection<NotificationItem> Notifications { get; }
        void ClearNotifications();
    }
}