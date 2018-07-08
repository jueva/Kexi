using System.Linq;
using Kexi.ViewModel.Dock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kexi.UI.Base
{
    public class LayoutUpdater : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            string destinationPane;
            var    treeView   = anchorableToShow.Content as ToolExplorerViewModel;
            var    detailView = anchorableToShow.Content as ToolDetailViewModel;
            if (treeView != null)
                destinationPane = "LeftAnchorPane";
            else if (detailView != null)
                destinationPane = "RightAnchorPane";
            else
                return false;

            var panes     = layout.Descendents().OfType<LayoutAnchorablePane>();
            var toolsPane = panes.FirstOrDefault(d => d.Name == destinationPane);
            if (toolsPane != null)
            {
                anchorableToShow.AutoHideMinWidth = 250;
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }
}