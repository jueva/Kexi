using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Shell;
using TreeViewItem = Kexi.ViewModel.TreeView.TreeViewItem;

namespace Kexi.UI.View
{
    /// <summary>
    ///     Interaction logic for ExplorerTreeView.xaml
    /// </summary>
    public partial class ExplorerTreeView : UserControl
    {
        public ExplorerTreeView()
        {
            InitializeComponent();
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