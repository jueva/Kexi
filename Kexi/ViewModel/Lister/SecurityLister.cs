using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(SecurityLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SecurityLister : BaseLister<SecurityItem>
    {
        [ImportingConstructor]
        public SecurityLister(Workspace workspace,  Options options, CommandRepository commandRepository)
            : base(workspace,  options, commandRepository)
        {
            Title           =  PathName = "Security";
            PropertyChanged += SecurityLister_PropertyChanged;
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Identity", "Identity"),
            new Column("Permissions", "Permissions"),
            new Column("AccessControlType", "AccessControlType")
        };

        public override string ProtocolPrefix => "Security";

        private void SecurityLister_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Path")
            {
                Title = Path;
                var fi = new FileItem(Path);
                PathName = "Security - " + fi.Name;
            }
        }

        protected override Task<IEnumerable<SecurityItem>> GetItems()
        {
            if (Path == null)
            {
                Workspace.NotificationHost.AddError("Pfad nicht gesetzt");
                return Task.FromResult(Enumerable.Empty<SecurityItem>());
            }
            var f   = File.GetAccessControl(Path);
            var acl = f.GetAccessRules(true, true, typeof(NTAccount));

            var items = acl.Cast<FileSystemAccessRule>().Select(r => new SecurityItem
            (
                GetIdentity(r.IdentityReference),
                r.AccessControlType.ToString(),
                r.FileSystemRights.ToString()
            ));
            return Task.FromResult(items);
        }

        private string GetIdentity(IdentityReference iRef)
        {
            var identity = iRef.ToString();
            var index    = identity.IndexOf('\\');
            if (index > -1) return string.Format("{0} ({1})", identity.Substring(index + 1), identity.Substring(0, index));
            return identity;
        }

        public override void DoAction(SecurityItem item)
        {
        }
    }
}