using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class GainAccessToDirectoryCommand : IKexiCommand
    {
        private readonly DialogPopupViewModel _dialogPopup;
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public GainAccessToDirectoryCommand(Workspace workspace, DialogPopupViewModel dialogPopup)
        {
            _workspace = workspace;
            _dialogPopup = dialogPopup;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is FileLister;
        }

        public void Execute(object folderPath)
        {
            if (folderPath is string path)
            {
                _workspace.PopupViewModel = _dialogPopup;
                _dialogPopup.Open("Access denied. Try to gain access?", ProcessSelection, "Yes", "No");

                async void ProcessSelection(string selection)
                {
                    if (selection == "Yes")
                        try
                        {
                            SetFolderPermission(path);
                            await _workspace.ActiveLister.Refresh();
                        }
                        catch
                        {
                            _workspace.NotificationHost.AddError("Access denied");
                        }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(folderPath));
            }
        }

        public event EventHandler CanExecuteChanged;

        private static void SetFolderPermission(string folderPath)
        {
            var rule = new FileSystemAccessRule(
                WindowsIdentity.GetCurrent().Name,
                FileSystemRights.Read,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow);

            var dir = new DirectoryInfo(folderPath);
            var permissions = dir.GetAccessControl();
            permissions.AddAccessRule(rule);
            dir.SetAccessControl(permissions);
        }
    }
}