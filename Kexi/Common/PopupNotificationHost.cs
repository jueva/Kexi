using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.Common
{
   // [Export(typeof(INotificationHost))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PopupNotificationHost : INotificationHost
    {
        private readonly Workspace _workspace;
        private readonly Options _options;
        private DispatcherTimer currentTimer;

        [ImportingConstructor]
        public PopupNotificationHost(Workspace workspace, Options options)
        {
            _workspace = workspace;
            _options = options;
            Notifications = new ObservableCollection<NotificationItem>();
        }

        public void AddError(Exception ex)
        {
            AddError(ex.Message, ex);
        }

        public void AddError(string message, Exception ex)
        {
            AddError(message, ex.StackTrace);
        }

        public void AddError(string message, string detailMessage = null)
        {
            var item = new NotificationItem(message, detailMessage, NotificationType.Error);
            Notifications.Add(item);
        }

        public void AddInfo(string message, string detailMessage = null)
        {
            Add(message);
        }


        private Popup _popup;
        public void Add(string message, string detailMessage = null, NotificationType notificationType = NotificationType.Info)
        {
            var item = new NotificationItem(message, detailMessage, notificationType);
            Notifications.Add(item);
            var backGround = (notificationType == NotificationType.Error)
                ? _workspace.ActiveLister.View.ListView.FindResource("ItemErrorColor") as Brush
                : _workspace.ActiveLister.View.ListView.FindResource("ListerStatusbarBackground") as Brush;

            var tb = new TextBlock
            {
                Text = message,
                Width = _workspace.ActiveLister.View.ListView.ActualWidth,
                Padding = new Thickness(5,2,5,2),
                Background = backGround
            };
            _popup = new Popup
            {
                StaysOpen = true,
                Child = tb,
                IsOpen = true,
                
                PlacementTarget =  _workspace.ActiveLister.View.ListView,
                Placement = PlacementMode.Bottom,
                VerticalOffset = -tb.ActualHeight - 3,
            };

            
            currentTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(_options.NotificationDuration)
            };
            currentTimer.Tick += (s, ea) => ClearCurrentMessage();
            currentTimer.Start();
        }

        public void ClearCurrentMessage()
        {
            if (_popup == null)
                return;

            _popup.IsOpen = false;
            currentTimer?.Stop();
        }

        public ObservableCollection<NotificationItem> Notifications { get; }
        public void ClearNotifications()
        {
            Notifications.Clear();
        }
    }
}
