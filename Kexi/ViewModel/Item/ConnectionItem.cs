using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class ConnectionItem : BaseItem
    {
        private string _startpoint;
        private string _endpoint;
        private TcpState _state;

        public TcpState State
        {
            get { return _state; }
            set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged();
            }
        }

        private string _description;


        public ConnectionItem(TcpConnectionInformation information)
        {
            Startpoint = information.LocalEndPoint.ToString();
            Endpoint = information.RemoteEndPoint.ToString();
            getDescription(information);
            State = information.State;
            DisplayName = FilterString = string.Concat(Startpoint, " - ", Endpoint);
        }


        private async void getDescription(TcpConnectionInformation information)
        {
            try
            {
                var entry = await Dns.GetHostEntryAsync(information.RemoteEndPoint.Address.ToString());
                Description = entry.HostName;
                DisplayName = FilterString = string.Concat(Startpoint, " - ", Endpoint, "(", Description, ")");
            }
            catch
            {
                Description = "";
            }
        }

        public string Startpoint
        {
            get { return _startpoint; }
            set
            {
                if (value == _startpoint) return;
                _startpoint = value;
                OnPropertyChanged();
            }
        }

        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                if (value == _endpoint) return;
                _endpoint = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }
    }
}
