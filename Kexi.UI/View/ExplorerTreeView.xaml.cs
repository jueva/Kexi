using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Shell;
using Kexi.ViewModel.Dock;
using TreeViewItem = Kexi.ViewModel.TreeView.TreeViewItem;

namespace Kexi.UI.View
{
    public partial class ExplorerTreeView
    {
        public ExplorerTreeView()
        {
            InitializeComponent();
            DataContextChanged += ExplorerTreeView_DataContextChanged;
        }

        private void ExplorerTreeView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToolExplorerViewModel detail)
            {
                //detail.ScrollDownAction = () => scrollViewer.PageDown();
                //detail.ScrollUpAction   = () => scrollViewer.PageUp();
            }
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button b)
            {
                var item  = (TreeViewItem) b.CommandParameter;
                var dinfo = new[] {new DirectoryInfo(item.Path)};
                var scm   = new ShellContextMenu();
                scm.ShowContextMenu(dinfo, System.Windows.Forms.Cursor.Position); //TODO: System.Windows.Forms && System.Drawing Reference just for this line...
                e.Handled = true;
            }
        }


    }
}