using System.Linq;
using Kexi.ViewModel.Dock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kexi.UI.Base
{
    public class LayoutUpdater : ILayoutUpdateStrategy
    {
        private const string LeftPane  = "LeftAnchroPane";
        private const string RightPane = "RighrAnchroPane";
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {

            string destinationPane;
            var    treeView    = anchorableToShow.Content as ToolExplorerViewModel;
            var    detailView  = anchorableToShow.Content as ToolDetailViewModel;
            var    previewView = anchorableToShow.Content as ToolPreviewViewModel;
            if (treeView != null)
                destinationPane = "RightAnchorPane";
            else if (detailView != null)
                destinationPane = "RightAnchorPane";
            else if (previewView != null)
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