using System.Windows.Data;

namespace Kexi
{
    public class Column
    {
        public Column(string header, string bindingExpression, int width)
            : this(header, bindingExpression)
        {
            Width = width;
        }

        public Column(string header, string bindingExpression, ColumnType type = ColumnType.Text, ColumnSize size = ColumnSize.Undefined, int? width = null, int? maxWidth = null)
        {
            Header = header;
            BindingExpression = bindingExpression;
            Type = type;
            Size = size;
            Width = width;
            MaxWidth = maxWidth;
        }

        public string Header { get; set; }
        public string BindingExpression { get; set; }
        public IValueConverter Converter { get; set; }
        public ColumnType Type { get; set; }
        public ColumnSize Size { get; set; }
        public int? Width { get; set; }
        public int? MaxWidth { get; set; }
        public bool OneTimeBinding { get; set; }
    }

    public enum ColumnType
    {
        Text,
        Number,
        Image,
        Bool,
        RightAligned,
        SyntaxHighlighted,
        Highlightable
    }

    public enum ColumnSize
    {
        Undefined,
        Auto,
        FullWidth
    }
}