using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kexi.Annotations;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
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
        public Options   Options   => Workspace.Options;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task SetItem(IItem item)
        {
            if (!(item is FileItem fileItem) || fileItem.IsContainer)
            {
                Items = Enumerable.Empty<LineItem>();
                return;
            }

            if (_viewFileLister == null) //TODO: Inject
                _viewFileLister = KexContainer.Resolve<ViewFileLister>();

            var targetResolver = new FileItemTargetResolver(fileItem);
            targetResolver.Parse();

            _viewFileLister.Path = targetResolver.TargetPath;
            if (_viewFileLister.GetEncoding() == null)
            {
                Items = Enumerable.Empty<LineItem>();
                return;
            }

            await _viewFileLister.Refresh().ConfigureAwait(false);
            Items = _viewFileLister.Items;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<LineItem> _items;

        public IEnumerable<LineItem> Items
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