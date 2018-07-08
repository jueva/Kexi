using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Kexi.ViewModel;

namespace Kexi.Common
{
    public class CopyHelper
    {
        public CopyHelper(Workspace workspace)
        {
            _workspace = workspace;
        }

        private readonly Workspace _workspace;

        public void CopySelectionToClipboard()
        {
            var selection = _workspace.ActiveLister.SelectedItems.Select(s => s.Path);
            var files     = new StringCollection();
            files.AddRange(selection.ToArray());
            Clipboard.SetFileDropList(files);
        }
    }
}