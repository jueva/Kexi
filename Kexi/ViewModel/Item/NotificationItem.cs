using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kexi.Common;

namespace Kexi.ViewModel.Item
{
    public class NotificationItem : BaseItem
    {
        public NotificationItem(string message, string detailMessage = null, NotificationType notificationType = NotificationType.Info) : base(message)
        {
            ShortMessage = message;
            DetailMessage = detailMessage;
            Message = ShortMessage;
            NotificationType = notificationType;
            DisplayDetails = false;
            _created = DateTime.Now;
        }

        private readonly DateTime _created;
        private DateTime _visited;
        private string _message;
        private string _detailMessage;
        private NotificationType _notificationType;


        public bool IsError
        {
            get { return NotificationType == NotificationType.Error; }
        }

        public bool IsInfo
        {
            get { return NotificationType == NotificationType.Info; }
        }

        public DateTime Created
        {
            get { return _created; }
        }

        public DateTime Visited
        {
            get { return _visited; }
            set
            {
                if (value == _visited)
                    return;

                _visited = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message)
                    return;

                _message = value;
                DisplayName = Message;
                OnPropertyChanged();
            }
        }

        public string ShortMessage
        {
            get { return _shortMessage; }
            set
            {
                if (value == _shortMessage)
                    return;

                _shortMessage = value;
                OnPropertyChanged();
            }
        }

        public string DetailMessage
        {
            get { return _detailMessage; }
            set
            {
                if (value == _detailMessage)
                    return;

                _detailMessage = value;
                HasDetails = !string.IsNullOrEmpty(_detailMessage);
                OnPropertyChanged();
            }
        }

        public NotificationType NotificationType
        {
            get { return _notificationType; }
            set
            {
                if (value == _notificationType)
                    return; 

                _notificationType = value;
                OnPropertyChanged();
            }
        }

        private bool _displayDetails;
        private string _shortMessage;
        private bool _hasDetails;

        public bool HasDetails
        {
            get { return _hasDetails; }
            set
            {
                if (value == _hasDetails)
                    return;
                _hasDetails = value;
                OnPropertyChanged();
            }
        }

        public bool DisplayDetails
        {
            get { return _displayDetails; }
            set
            {
                if (value == _displayDetails)
                    return;

                _displayDetails = value;
                Message = FilterString = _displayDetails ? DetailMessage : ShortMessage;
                OnPropertyChanged();
            }
        }

    }
}
