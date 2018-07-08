using System.Windows;
using System.Windows.Controls;
using Kexi.ViewModel.Dock;

namespace Kexi.UI.Base
{
    internal class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle     { get; set; }
        public Style ToolDetailStyle   { get; set; }
        public Style ToolExplorerStyle { get; set; }
        public Style ToolPreviewStyle  { get; set; }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DocumentViewModel)
                return DocumentStyle;

            if (item is ToolDetailViewModel)
                return ToolDetailStyle;

            if (item is ToolExplorerViewModel)
                return ToolExplorerStyle;

            if (item is ToolPreviewViewModel)
                return ToolPreviewStyle;

            return base.SelectStyle(item, container);
        }
    }
}