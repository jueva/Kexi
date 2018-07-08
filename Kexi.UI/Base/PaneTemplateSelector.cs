using System.Windows;
using System.Windows.Controls;
using Kexi.ViewModel.Dock;

namespace Kexi.UI.Base
{
    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListerTemplate { get; set; }

        public DataTemplate ToolDetailTemplate { get; set; }

        public DataTemplate ToolExplorerTemplate { get; set; }
        public DataTemplate ToolPreviewTemplate  { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DocumentViewModel)
                return ListerTemplate;

            if (item is ToolDetailViewModel)
                return ToolDetailTemplate;

            if (item is ToolExplorerViewModel)
                return ToolExplorerTemplate;

            if (item is ToolPreviewViewModel)
                return ToolPreviewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}