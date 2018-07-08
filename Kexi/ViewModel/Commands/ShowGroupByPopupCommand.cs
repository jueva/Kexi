using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowGroupByPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly GroupByPopupViewModel _groupByPopup;

        [ImportingConstructor]
        public ShowGroupByPopupCommand(Workspace workspace, GroupByPopupViewModel groupByPopup)
        {
            _workspace = workspace;
            _groupByPopup = groupByPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
           _workspace.PopupViewModel = _groupByPopup;
            _groupByPopup.Open();
        }

        public event EventHandler CanExecuteChanged;
    }
}