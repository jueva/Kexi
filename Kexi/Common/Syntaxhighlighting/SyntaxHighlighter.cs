using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Kexi.Common.Syntaxhighlighting
{
    public class SyntaxHighlighter : IDisposable
    {
        public SyntaxHighlighter(Encoding encoding)
        {
            _encoding = encoding;
        }

        public           string   Text { get; set; }
        private readonly Encoding _encoding;

        private List<byte[]> _binary;
        private DocumentHighlighter     _highlighter;
        private IHighlightingDefinition _highlightingDefinition;
        private TextDocument            _textDocument;

        public void Init(string path, string extension)
        {
            if (_encoding != null)
            {
                Text                    = File.ReadAllText(path, _encoding);
                _textDocument           = new TextDocument(Text);
                _highlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(extension);
                if (_highlightingDefinition != null)
                    _highlighter = new DocumentHighlighter(_textDocument, _highlightingDefinition);
            }
            else
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer        = new byte[stream.Length];
                    var contentLength = buffer.Length;
                    var lineLength    = 24;
                    stream.Read(buffer, 0, contentLength);
                    _binary = new List<byte[]>();
                    for (var i = 0; i < contentLength; i += lineLength)
                    {
                        if (i + lineLength > contentLength)
                            lineLength = contentLength - i;
                        var part = new byte[lineLength];
                        Array.Copy(buffer,i,part,0, lineLength);
                        _binary.Add(part);
                    }

                    Text = string.Join(Environment.NewLine, _binary.Select(b => string.Join("", b.Select(c => (char) c))));
                }
            }
        }

        public Run[] GetInlines(int lineIndex)
        {
            if (_encoding == null)
            {
                var line = _binary[lineIndex - 1];
                return GetHex(line).ToArray();
            }

            if (_highlighter != null)
            {
                var line = _highlighter.HighlightLine(lineIndex);
                return line.ToRichText().CreateRuns();
            }

            var docLine = _textDocument.GetLineByNumber(lineIndex);
            return new[] {new Run(_textDocument.GetText(docLine.Offset, docLine.Length))};
        }

        private static IEnumerable<Run> GetHex(byte[] bytes)
        {
            const string fontFamily = "Consolas";
            foreach (var b in bytes)
                if (b > 'A' && b < 'z')
                {
                    var run = new Run(Encoding.ASCII.GetString(new[] {b}) + "  ")
                    {
                        FontFamily = new FontFamily(fontFamily),
                        Background = Brushes.DimGray
                    };
                    yield return run;
                }
                else
                {
                    yield return new Run(b.ToString("x2").ToUpper() + " ")
                    {
                        FontFamily = new FontFamily(fontFamily)
                    };
                }
        }

        public void Dispose()
        {
            _highlighter?.Dispose();
        }
    }
}