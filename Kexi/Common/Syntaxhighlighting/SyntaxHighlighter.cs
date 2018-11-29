using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Kexi.ViewModel.Item;

namespace Kexi.Common.Syntaxhighlighting
{
    public class SyntaxHighlighter : IDisposable
    {
        public SyntaxHighlighter(Encoding encoding)
        {
            _encoding = encoding;
            _currentForeground = Application.Current.FindResource("ListerForeground") as SolidColorBrush;
            _darkForeground = IsDarkForeground();
        }

        private bool IsDarkForeground()
        {
            var c = _currentForeground.Color;
            var y = 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
            return y < 128;
        }

        public           string   Text { get; set; }
        private readonly Encoding _encoding;

        public byte[] Binary { get; private set; }
        private DocumentHighlighter     _highlighter;
        private IHighlightingDefinition _highlightingDefinition;
        private TextDocument            _textDocument;
        private readonly SolidColorBrush _currentForeground;
        private readonly bool _darkForeground;

        public void Init(IEnumerable<string> lines)
        {
            _textDocument = new TextDocument(string.Join(Environment.NewLine, lines));
        }

        public async Task<IEnumerable<LineItem>> InitBinary(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var contentLength = (int) stream.Length;
                Binary        = new byte[contentLength];
                await stream.ReadAsync(Binary, 0, contentLength);
                return Enumerable.Range(1,contentLength/24).Select((b, index) => new LineItem(this, "", index + 1)).ToList();
            }
        }

        public async Task<IEnumerable<LineItem>> Init(string path, string extension)
        {
            using (var file = new StreamReader(path, _encoding))
            {
                Text = await file.ReadToEndAsync();
            }
            _textDocument           = new TextDocument(Text);
            _highlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(extension);

            if (_highlightingDefinition != null)
            {
                _highlighter = new DocumentHighlighter(_textDocument, _highlightingDefinition);
                SetHighlightColors(_highlightingDefinition);
            }
            var lines = Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return lines.Select((l, index) => new LineItem(this, l, index+1));
        }

        private void SetHighlightColors(IHighlightingDefinition highlighter)
        {
            if (_darkForeground)
                return;
            foreach (var brush in highlighter.NamedHighlightingColors)
            {
                brush.Foreground = new SimpleHighlightingBrush(_currentForeground.Color);
            }
        }

        public Run[] GetInlines(int lineIndex)
        {
            if (_encoding == null)
            {
                return new Run[0];
                return GetHex(Binary, lineIndex ).ToArray();
            }

            var docLine = _textDocument.GetLineByNumber(lineIndex);
            var text = _textDocument.GetText(docLine.Offset, docLine.Length);
            if (_highlighter != null)
            {
                var hLine = _highlighter.HighlightLine(lineIndex);
                return hLine.ToRichText().CreateRuns();
            }

            var run = new Run(text) {Foreground = _currentForeground};
            return new[] {run};
        }

        private IEnumerable<Run> GetHex(byte[] bytes, int lineIndex)
        {
            const string fontFamily = "Consolas";
            var offset = (lineIndex - 1) * 24;
            var length = Math.Min(24, bytes.Length - offset);
            for (var i = offset; i < offset+length; i++)
            {
                var b = bytes[i];
                if (b > 'A' && b < 'z')
                {
                    var run = new Run(Encoding.ASCII.GetString(new[] {b}) + "  ")
                    {
                        FontFamily = new FontFamily(fontFamily),
                        Background = Brushes.DimGray,
                        Foreground = _currentForeground
                    };
                    yield return run;
                }
                else
                {
                    yield return new Run(b.ToString("x2").ToUpper() + " ")
                    {
                        FontFamily = new FontFamily(fontFamily),
                        Foreground = _currentForeground
                    };
                }
            }
        }

        public void Dispose()
        {
            _highlighter?.Dispose();
        }
    }
}