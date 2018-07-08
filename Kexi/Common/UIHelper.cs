using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;
using Kexi.ViewModel.Lister;
using Xceed.Wpf.AvalonDock.Layout;
using Application = System.Windows.Application;

namespace Kexi.Common
{
    public class UIHelper
    {
        private readonly Workspace _workspace;

        public UIHelper(Workspace workspace)
        {
            _workspace = workspace;
        }

        public static void CenterToScreen(Screen s)
        {
            if (Application.Current.MainWindow != null)
            {
                var left = s.WorkingArea.X + (s.WorkingArea.Width - Application.Current.MainWindow.Width) / 2;
                Application.Current.MainWindow.Left = left;
            }
        }

        public static Screen GetCurrentScreen()
        {
            if (Application.Current.MainWindow != null)
            {
                var top  = Application.Current.MainWindow.Top;
                var left = Application.Current.MainWindow.Left;
                return Screen.FromPoint(new System.Drawing.Point((int) left, (int) top));
            }

            return Screen.FromPoint(new System.Drawing.Point(0, 0));
        }


        public void FitWidth()
        {
            var gridView = _workspace.ActiveLister.View.ListView.View as GridView;
            var colHeader = gridView?.Columns.First().Header as GridViewColumnHeader;
            if (!(colHeader?.Content is string header))
                return;

            var skip = string.IsNullOrEmpty(header) ? 1 : 0; //HACK: Dont resize Thumbnail Column

            foreach (var column in gridView.Columns.Skip(skip))
            {
                var col = _workspace.ActiveLister.Columns.FirstOrDefault(c => c.Header == column.Header.ToString());
                if (col != null && col.Size == ColumnSize.Auto)
                {
                    column.Width = column.ActualWidth;
                    column.Width = double.NaN;
                }
            }

            var colWidth = 0d;
            foreach (var pane in _workspace.Manager.Layout.Descendents().OfType<LayoutDocumentPane>())
            {
                var listView =
                    (((DocumentViewModel)pane.SelectedContent.Content).Content).View.ListView;
                var gridWidth = 0d;
                if (listView.View is GridView grid)
                {
                    foreach (var c in grid.Columns)
                    {
                        colWidth += c.ActualWidth;
                        gridWidth += c.ActualWidth;
                    }
                }
                pane.DockMinWidth = gridWidth;
                pane.DockMinWidth = 0;
            }

            var manager = _workspace.Manager;
            var panes = manager.Layout.Descendents().OfType<LayoutAnchorablePane>().ToList();
            colWidth += panes.Where(p => p.IsVisible).Sum(p => p.DockWidth.Value);
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Width = colWidth + 80;
        }
    }
}
