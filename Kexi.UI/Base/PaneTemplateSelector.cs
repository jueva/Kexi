using System.Windows;
using System.Windows.Controls;
using Kexi.ViewModel.Dock;

namespace Kexi.UI.Base
{
    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListerTemplate { private get; set; }

        public DataTemplate ToolDetailTemplate { private get; set; }

        public DataTemplate ToolExplorerTemplate { private get; set; }

        public DataTemplate ToolPreviewTemplate  { private get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case DocumentViewModel _:
                    return ListerTemplate;
                case ToolDetailViewModel _:
                    return ToolDetailTemplate;
                case ToolExplorerViewModel _:
                    return ToolExplorerTemplate;
                case ToolPreviewViewModel _:
                    return ToolPreviewTemplate;
                default:
                    return base.SelectTemplate(item, container);
            }
        }
    }
}