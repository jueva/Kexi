using System;
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
    [Export(typeof(TextLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TextLister : BaseLister<BaseItem>
    {
        [ImportingConstructor]
        public TextLister(Workspace workspace, Options options, CommandRepository commandRepository)
            : base(workspace, options, commandRepository)
        {
        }

        public override ObservableCollection<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("", "DisplayName", ColumnType.Text, ColumnSize.FullWidth)
            };

        public override string ProtocolPrefix => "Edit";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnNotifyPropertyChanged();
            }
        }

        private string _text;

        protected override Task<IEnumerable<BaseItem>> GetItems()
        {
            if (Text == null)
                return null;

            var lines = Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return Task.FromResult(lines.Select(t => new BaseItem(t)));
        }
    }
}