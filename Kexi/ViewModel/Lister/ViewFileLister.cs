using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Common.Syntaxhighlighting;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;
using Ude;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(ViewFileLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ViewFileLister : BaseLister<LineItem>
    {
        private readonly Options _options;
        private SyntaxHighlighter _syntaxHighlighter;

        [ImportingConstructor]
        public ViewFileLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository)
            : base(workspace, notificationHost, options, commandRepository)
        {
            _options = options;
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "LineNumber", ColumnType.Number, ColumnSize.Undefined, 40),
            new Column("", "RtfRuns", ColumnType.SyntaxHighlighted, ColumnSize.FullWidth)
        };

        public override string StatusString => null;

        protected override async Task<IEnumerable<LineItem>> GetItems()
        {
            Title            = Path;
            Thumbnail        = ShellNative.GetLargeBitmapSource(Path);
            var i            = 1;
            PathName         = System.IO.Path.GetFileName(Path);
            var    extension = System.IO.Path.GetExtension(Path);
            var encoding = GetEncoding();
            _syntaxHighlighter = new SyntaxHighlighter(encoding);
            if (encoding == null)
                await Task.Run(() => _syntaxHighlighter.Init(Path, extension));
            else
                _syntaxHighlighter.Init(Path, extension); //Avalonedit Textdocument must be on same thread

            var lines = _syntaxHighlighter.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return lines.Select(l => new LineItem(_syntaxHighlighter, l, i++));
        }

        public override void DoAction(LineItem lineItem)
        {
            if (lineItem == null)
                return;

            var psi       = new ProcessStartInfo(_options.PreferredEditorLocation);
            psi.Arguments = $"+{lineItem.LineNumber} {Path}";
            Process.Start(psi);
        }

        public override string GetParentContainer()
        {
            return null;
        }

        private Encoding GetEncoding()
        {
            try
            {
                var detector = new CharsetDetector();
                detector.Feed(File.OpenRead(Path));
                detector.DataEnd();
                return detector.Charset == null ? null : Encoding.GetEncoding(detector.Charset);
            }
            catch (Exception ex)
            {
                NotificationHost.AddError("Fehler beim Ermitteln des Encodings", ex);
                return Encoding.Default;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _syntaxHighlighter?.Dispose();
            base.Dispose(disposing);
        }
    }
}