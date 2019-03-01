using System;
using System.ComponentModel.Composition;
using System.IO;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CreateDirectoryFromFullPathCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CreateDirectoryFromFullPathCommand(Workspace workspace, TextInputPopupViewmodel textInputPopup)
        {
            _workspace = workspace;
            _textInputPopup = textInputPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _textInputPopup;
            _textInputPopup.Open("Enter Path", PathSelected);
        }

        private void PathSelected(string name)
        {
            try
            {
                Directory.CreateDirectory(name);
            }
            catch (Exception ex)
            {
                _workspace.NotificationHost.AddError(ex);
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
        private readonly TextInputPopupViewmodel _textInputPopup;
    }
}