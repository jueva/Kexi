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
        private readonly Workspace _workspace;
        private readonly SpecialPastePopupViewModel _specialPastePopup;

        [ImportingConstructor]
        public ShowSpecialPastePopupCommand(Workspace workspace, SpecialPastePopupViewModel specialPastePopup)
        {
            _workspace = workspace;
            _specialPastePopup = specialPastePopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel          = _specialPastePopup;
            _specialPastePopup.Open();
        }

        public event EventHandler CanExecuteChanged;
    }
}