using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Property;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(PropertyLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropertyLister : BaseLister<PropertyItem>
    {
        private readonly FilePropertyProvider _contentProvider;
        private readonly ExtendedExtendedPropertyProvider _extendedPropertyProvider;
        private FileItem _fileItem;

        [ImportingConstructor]
        public PropertyLister(Workspace workspace, Options options, CommandRepository commandRepository,
            ExtendedExtendedPropertyProvider extendedPropertyProvider)
            : base(workspace, options, commandRepository)
        {
            _extendedPropertyProvider = extendedPropertyProvider;
            _contentProvider = new FilePropertyProvider(workspace);
        }

        public override ObservableCollection<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("Key", "Key", ColumnType.Highlightable),
            new Column("Value", "Value", ColumnType.Highlightable),
            new Column("Group", "Group") {Size = ColumnSize.FullWidth}
        };

        public override string ProtocolPrefix => "Properties";

        public FileItem FileItem
        {
            private get => _fileItem;
            set
            {
                if (_fileItem == value)
                    return;
                _fileItem = value;
                OnNotifyPropertyChanged();
            }
        }

        public override bool ShowInMenu => false;

        [ExportContextMenuCommand(typeof(PropertyLister), "Copy value(s)")]
        public ICommand CopyValues
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var selection = Workspace.GetSelection<PropertyItem>().Select(p => p.Value);
                    var text = string.Join(Environment.NewLine, selection);
                    Clipboard.SetText(text);
                });
            }
        }

        [ExportContextMenuCommand(typeof(PropertyLister), "Copy key(s)")]
        public ICommand CopyKeys
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var selection = Workspace.GetSelection<PropertyItem>().Select(p => p.Key);
                    var text = string.Join(Environment.NewLine, selection);
                    Clipboard.SetText(text);
                });
            }
        }

        protected override Task<IEnumerable<PropertyItem>> GetItems()
        {
            if (FileItem == null)
                return null;

            Path = _fileItem.Path;
            PathName = Title = ProtocolPrefix + "/" + $"{_fileItem.DisplayName}";
            return Task.Run(Parse);
        }

        private async Task<IEnumerable<PropertyItem>> Parse()
        {
            var it = new RangeObservableCollection<PropertyItem>();
            it.AddRange(await _extendedPropertyProvider.GetItems(_fileItem).ConfigureAwait(false));

            await _contentProvider.SetItem(new FileItem(_fileItem.Path));
            it.AddRange(_contentProvider.PropertiesBottom);
            return it;
        }

        public override void Copy()
        {
            var selection = ItemsView.SelectedItems.Select(s => $"{s.Key} - {s.Value}");
            var text = string.Join(Environment.NewLine, selection);
            Clipboard.SetText(text);
        }
    }
}