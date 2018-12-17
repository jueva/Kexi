using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kexi.Annotations;
using Kexi.Common.Syntaxhighlighting;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel
{
    public class PreviewContentView : INotifyPropertyChanged
    {
        private ViewFileLister _viewFileLister;

        public PreviewContentView(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task SetItem(IItem item)
        {
            if (item is FileItem fileItem)
            {
                var target = new FileItemTargetResolver(fileItem);
                if (target.TargetType == ItemType.Container)
                {
                    await ShowDirectoryContent(target);
                }
                else
                {
                    await ShowFileContent(fileItem);
                }
            }
            else
            {
                Items = Enumerable.Empty<RtfItem>();
            }
        }

        private async Task ShowFileContent(FileItem fileItem)
        {
            if (_viewFileLister == null) //TODO: Inject
                _viewFileLister = KexContainer.Resolve<ViewFileLister>();

            var targetResolver = new FileItemTargetResolver(fileItem);
            _viewFileLister.Path = targetResolver.TargetPath;

            await _viewFileLister.Refresh().ConfigureAwait(false);
            Items = _viewFileLister.Items;
        }

        private async Task ShowDirectoryContent(FileItemTargetResolver target)
        {
            var provider = new FileItemProvider(Workspace);
            var items = (await provider.GetItems(target.TargetPath)).ToList();
            var highLighter = new SyntaxHighlighter(Encoding.UTF8);
            highLighter.Init(items.Select(i => i.Name));
            var line = 1;
            Items = items.Select(i => new RtfItem(highLighter, i.Name, line++));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<RtfItem> _items;

        public IEnumerable<RtfItem> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }
    }
}