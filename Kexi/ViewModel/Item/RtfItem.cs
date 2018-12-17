using System.Windows.Documents;
using Kexi.Common.Syntaxhighlighting;

namespace Kexi.ViewModel.Item
{
    public class RtfItem : BaseItem
    {
        public RtfItem(SyntaxHighlighter highlighter, string line, int lineNumber) : base(line)
        {
            LineNumber   = lineNumber;
            _line        = line;
            _highlighter = highlighter;
        }

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

        public int LineNumber 
            { 
            get => _lineNumber; 
            set
            {
                if (_lineNumber == value)
                    return;

                _lineNumber = value;
                OnPropertyChanged();
            }
        }


        private readonly SyntaxHighlighter _highlighter;
        private          Run[]             _rtfRuns;
        private string _line;
        private int _lineNumber;

        private void SetRtf()
        {
            RtfRuns = _highlighter.GetInlines(LineNumber);
        }
    }
}