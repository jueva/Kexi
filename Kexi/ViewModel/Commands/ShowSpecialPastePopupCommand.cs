using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSpecialPastePopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowSpecialPastePopupCommand(Workspace workspace, SpecialPastePopupViewModel specialPastePopup)
        {
            _workspace         = workspace;
            _specialPastePopup = specialPastePopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _specialPastePopup;
            _specialPastePopup.Open();
        }

        public event EventHandler                   CanExecuteChanged;
        private readonly SpecialPastePopupViewModel _specialPastePopup;
        private readonly Workspace                  _workspace;
    }
}