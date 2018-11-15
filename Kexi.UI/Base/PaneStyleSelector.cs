using System.Windows;
using System.Windows.Controls;
using Kexi.ViewModel.Dock;

namespace Kexi.UI.Base
{
    internal class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle     { private get; set; }
        public Style ToolDetailStyle   { private get; set; }
        public Style ToolExplorerStyle { private get; set; }
        public Style ToolPreviewStyle  { private get; set; }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case DocumentViewModel _:
                    return DocumentStyle;
                case ToolDetailViewModel _:
                    return ToolDetailStyle;
                case ToolExplorerViewModel _:
                    return ToolExplorerStyle;
                case ToolPreviewViewModel _:
                    return ToolPreviewStyle;
                default:
                    return base.SelectStyle(item, container);
            }
        }
    }
}