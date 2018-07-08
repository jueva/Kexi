using System.Windows.Documents;
using Kexi.Common.Syntaxhighlighting;

namespace Kexi.ViewModel.Item
{
    public class LineItem : BaseItem
    {
        public LineItem(SyntaxHighlighter highlighter, string line, int lineNumber) : base(line)
        {
            LineNumber   = lineNumber;
            _line        = line;
            _highlighter = highlighter;
        }

        public int LineNumber { get; }

        public Run[] RtfRuns
        {
            get
            {
                if (_rtfRuns == null)
                    SetRtf();
                return _rtfRuns;
            }
            set
            {
                _rtfRuns = value;
                OnPropertyChanged();
            }
        }

        private readonly SyntaxHighlighter _highlighter;
        private readonly string            _line;
        private          Run[]             _rtfRuns;

        private void SetRtf()
        {
            RtfRuns = _highlighter.GetInlines(LineNumber);
        }
    }
}