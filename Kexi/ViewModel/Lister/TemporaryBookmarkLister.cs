using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TemporaryBookmarkLister : BaseLister<BaseItem>
    {
        [ImportingConstructor]
        public TemporaryBookmarkLister(Workspace workspace, Options options, CommandRepository commandRepository)
            : base(workspace, options, commandRepository)
        {
            Title = PathName = "Temporary Bookmarks";
        }

        public override ObservableCollection<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("DisplayName", "DisplayName", ColumnType.Highlightable),
                new Column("Path", "Path", ColumnType.Highlightable)
            };

        public override string ProtocolPrefix => "Temboo";

        protected override Task<IEnumerable<BaseItem>> GetItems()
        {
            return Task.FromResult(Workspace.TemporaryFavorites.Favorites.Cast<BaseItem>());
        }
    }
}