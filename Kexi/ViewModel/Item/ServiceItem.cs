using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Kexi.Common;

namespace Kexi.ViewModel.Item
{
    public class ServiceItem: BaseItem, INotifyPropertyChanged
    {
        public ServiceItem(ServiceController controller) : base(controller.ServiceName)
        {
            ServiceController = controller;
            Status = controller.Status;
            Description = controller.DisplayName;
            ServiceType = controller.ServiceType.ToString();
            try
            {
                Dependent = string.Join(",", controller.DependentServices.Select(s => s.ServiceName));
            } catch  { } //AccessDenied
            FilterString = string.Concat(DisplayName,"_",Description,"_",Status);
            Thumbnail = thumb.Value;
        }

        public string Dependent { get; set; }

        public string ServiceType { get; set; }

        private ServiceControllerStatus _status;
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description)
                    return;

                _description = value;
                OnPropertyChanged();
            }
        }

        public ServiceControllerStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ServiceController ServiceController { get; }

        private readonly Lazy<BitmapImage> thumb = new Lazy<BitmapImage>(() => Utils.GetImageFromRessource("service.png")); 
    }
}
