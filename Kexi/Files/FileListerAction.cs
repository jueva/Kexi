using System.Diagnostics;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.Files
{
    public class FileListerAction
    {
        public FileListerAction(Workspace workspace, FileItem item)
        {
            _workspace = workspace;
            _fileItem  = item;
        }

        private readonly FileItem  _fileItem;
        private readonly Workspace _workspace;

        public string DoAction()
        {
            if (_fileItem.Extension == ".ktc")
            {
                _workspace.DockingMananger.DeserializeLayout(_fileItem.Path);
                return null;
            }

            var target = new FileItemTargetResolver(_fileItem);
            if (target.TargetType == ItemType.Container)
            {
                return target.TargetPath;
            }

            var startInfo = new ProcessStartInfo(target.TargetPath)
            {
                WorkingDirectory = target.WorkingDirectory
            };
            Process.Start(startInfo);
            return null;
        }
    }
}