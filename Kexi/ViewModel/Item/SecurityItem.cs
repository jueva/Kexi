using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class SecurityItem : BaseItem
    {
        private string _identity;
        private string _accessControlType;
        private string _permissions;

        public SecurityItem(string identity, string acl, string permissions)
        {
            Identity = identity;
            AccessControlType = acl;
            Permissions = permissions;
            DisplayName = FilterString = string.Concat(identity, ", ", acl, ", ", permissions);
        }

        public string Identity
        {
            get { return _identity; }
            set
            {
                if (_identity == value)
                    return;
                _identity = value;
                OnPropertyChanged();
            }
        }

        public string AccessControlType
        {
            get { return _accessControlType; }
            set
            {
                if (_accessControlType == value)
                    return;
                _accessControlType = value;
                OnPropertyChanged();
            }
        }

        public string Permissions
        {
            get { return _permissions; }
            set
            {
                if (_permissions == value)
                    return;
                _permissions = value; 
                OnPropertyChanged();
            }
        }
    }
}
