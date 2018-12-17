using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
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
    public class ViewFileLister : BaseLister<RtfItem>
    {
        private readonly Options _options;
        private SyntaxHighlighter _syntaxHighlighter;

        [ImportingConstructor]
        public ViewFileLister(Workspace workspace, Options options, CommandRepository commandRepository)
            : base(workspace,  options, commandRepository)
        {
            _options = options;
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "LineNumber", ColumnType.Number, ColumnSize.Undefined, 40),
            new Column("", "RtfRuns", ColumnType.SyntaxHighlighted, ColumnSize.FullWidth)
        };

        protected override async Task<IEnumerable<RtfItem>> GetItems()
        {
            Title            = Path;
            Thumbnail        = ShellNative.GetLargeBitmapSource(Path);
            PathName         = System.IO.Path.GetFileName(Path);
            var    extension = System.IO.Path.GetExtension(Path);
            var encoding = GetEncoding();
            _syntaxHighlighter = new SyntaxHighlighter(encoding);
            if (encoding == null)
            {
                return await Task.Run(() => _syntaxHighlighter.InitBinary(Path));
            }
            return await _syntaxHighlighter.Init(Path, extension);
        }

        public override void DoAction(RtfItem lineItem)
        {
            if (lineItem == null)
                return;

            var psi = new ProcessStartInfo(_options.PreferredEditorLocation)
            {
                Arguments = $"+{lineItem.LineNumber} {Path}"
            };
            Process.Start(psi);
        }

        private Encoding GetEncoding()
        {
            try
            {
                return GetEncoding(Path);
            }
            catch (Exception ex)
            {
                NotificationHost.AddError("Fehler beim Ermitteln des Encodings", ex);
                return Encoding.Default;
            }
        }

        private static Encoding GetEncoding(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var detector = new CharsetDetector();
                detector.Feed(stream);
                detector.DataEnd();
                return detector.Charset == null ? null : Encoding.GetEncoding(detector.Charset);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _syntaxHighlighter?.Dispose();
            base.Dispose(disposing);
        }
    }
}