using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class AdjustColumnWidth : IKexiCommand
    {
        [ImportingConstructor]
        public AdjustColumnWidth(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!(_workspace.ActiveLister.View.ListView.View is GridView gridView))
                return;

            foreach (var column in gridView.Columns)
            {
                var header = ((ContentControl) column.Header).Content as string;
                var col    = _workspace.ActiveLister.Columns.FirstOrDefault(co => co.Header == header);
                //TODO: check actualsize vs desiredsize
                if (string.IsNullOrEmpty(col?.Header)) continue;
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}