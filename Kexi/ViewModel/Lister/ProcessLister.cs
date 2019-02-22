using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Converter;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ProcessLister))]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProcessLister : BaseLister<ProcessItem>, IBackgroundLoader<ProcessItem>
    {
        [ImportingConstructor]
        public ProcessLister(Workspace workspace,  Options options, CommandRepository commandRepository)
            : base(workspace,  options, commandRepository)
        {
            Title     = PathName = Path = "Processes";
            Thumbnail = _thumb.Value;
        }

        public override bool SupportsMultiview => true;

        public override ObservableCollection<Column> Columns { get; }
            = new ObservableCollection<Column>
            {
                new Column("", "Details.Thumbnail", ColumnType.Image),
                new Column("Name", "DisplayName", ColumnType.Highlightable) {Width                = 300},
                new Column("Description", "Details.Description", ColumnType.Highlightable) {Width = 300},
                new Column("Memory", "Details.Memory", ColumnType.RightAligned) { Width     = 80, Converter = new LengthConverter() },
                new Column("Cpu", "Details.Cpu", ColumnType.RightAligned),
                new Column("Pid", "Details.Pid")
            };

        public override string ProtocolPrefix => "processes";

        [ExportContextMenuCommand(typeof(ProcessLister), "Kill Process")]
        public ICommand KillProcess
        {
            get
            {
                return new RelayCommand(c =>
                    {
                        var items = Workspace.GetSelection<ProcessItem>();
                        foreach (var item in items) item.Process.Kill();
                    }
                );
            }
        }

        [ExportContextMenuCommand(typeof(ProcessLister), "Open File Location")]
        public ICommand OpenFileLocation
        {
            get
            {
                return new RelayCommand(c =>
                    {
                        var item       = Workspace.GetSelection<ProcessItem>().First();
                        OpenFileLister(item);
                    }
                );
            }
        }

        private async void OpenFileLister(ProcessItem item)
        {
            var fileLister = KexContainer.Resolve<FileLister>();
            fileLister.Path = System.IO.Path.GetDirectoryName(item.FileName);
            await fileLister.Refresh();
            _processItem       =  item;
            fileLister.GotView += FileLister_GotView;
            Workspace.Open(fileLister);
        }

        private readonly Lazy<BitmapImage> _thumb = new Lazy<BitmapImage>(() => Utils.GetImageFromRessource("process.png"));

        private ProcessItem _processItem;

        private void FileLister_GotView(ILister obj)
        {
            if (obj is FileLister fileLister)
            {
                fileLister.GotView -= FileLister_GotView;
                fileLister.View.FocusItem(fileLister.Items.FirstOrDefault(i => i.Path == _processItem?.FileName));
            }
        }

        protected override Task<IEnumerable<ProcessItem>> GetItems()
        {
            return Task.Run(() => Process.GetProcesses().Select(p => new ProcessItem(p)));
        }

        public void LoadBackgroundData(IEnumerable<ProcessItem> items, CancellationToken cancellationToken)
        {
            foreach (var i in items)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                i.Details = i.GetDetail();
            }
        }

        public void LoadBackgroundData(IEnumerable items, CancellationToken cancellationToken)
        {
            LoadBackgroundData(items.Cast<ProcessItem>(), cancellationToken);
        }
    }
}