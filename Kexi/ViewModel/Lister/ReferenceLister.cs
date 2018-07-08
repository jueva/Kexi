using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Kexi.Common;
using Kexi.Common.MultiSelection;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.Property;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof (ReferenceLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class ReferenceLister : BaseLister<ReferenceItem>, IHistorisationProvider
    {
        private SearchItemProvider _searchItemProvider;

        [ImportingConstructor]
        public ReferenceLister(Workspace workspace, INotificationHost notificationHost, Options options,
            CommandRepository commandRepository)
            : base(workspace, notificationHost, options, commandRepository)
        {
            Items = new ObservableCollection<ReferenceItem>();
            Title = PathName = ProtocolPrefix;
            PropertyChanged += ReferenceLister_PropertyChanged;
  
            History = new BrowsingHistory();
        }

        private void ReferenceLister_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Path")
            {
                Item = new FileItem(Path);
                History.Push(Path, Filter, GroupBy, SortHandler.CurrentSortDescription);
                PathName = Item.Name;
            }
        }

        private FileItem Item { get; set; }

        public override IEnumerable<Column> Columns => new[]
        {
            new Column("", "Thumbnail", ColumnType.Image),
            new Column("Name", "DisplayName", ColumnType.Highlightable),
            new Column("Version", "Version"),
            new Column("Referenced by", "RelativePath"),
        };

        public override async void Refresh()
        {
            Items.Clear();
            if (Item.IsContainer)
            {
                _searchItemProvider = new SearchItemProvider();
                _searchItemProvider.ItemAdded += SearchItemProvider_ItemAdded;
                await _searchItemProvider.GetItems(Path, ".dll");
            }
            else
            {
                foreach (var refItem in GetReferenceItems(Item.GetPathResolved()))
                    Items.Add(refItem);
            }
            ItemsView = new MultiSelectCollectionView<ReferenceItem>(Items);
            Title = PathName = ProtocolPrefix+"/"+Item.Name;
        }

        protected override Task<IEnumerable<ReferenceItem>> GetItems() => null; //Refresh overide. Yup, ugly

        private void SearchItemProvider_ItemAdded(FileItem obj)
        {
            if (!Items.Any())
            {
                Workspace.FocusCurrentOrFirst();
            }
            foreach (var i in GetReferenceItems(obj.GetPathResolved()))
                Items.Add(i);
        }

        private IEnumerable<ReferenceItem> GetReferenceItems(string path)
        {
            var rootPath = Item.IsContainer
                ? Item.GetPathResolved()
                : Item.Directory;

            var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(path);
            var baseName = assembly.Name.FullName;
            var main = assembly.MainModule;
            var refs = main.AssemblyReferences.Select(r => new ReferenceItem(path, rootPath, baseName, r));
            return refs;
        }

        public override void DoAction(ReferenceItem item)
        {
            if (!string.IsNullOrEmpty(item?.TargetPath))
            {
                History.Push(Path,Filter, GroupBy, SortHandler.CurrentSortDescription);
                Path = item.TargetPath;
                Refresh();
            }
        }

       public override void Copy()
        {
            var selection = ItemsView.SelectedItems.Select(s => string.Join(",", s.DisplayName, s.Version, s.Assembly));
            var text = string.Join(Environment.NewLine, selection);
            Clipboard.SetText(text);
        }

        public override string ProtocolPrefix => "References";

        public BrowsingHistory History { get; }
    }
}
