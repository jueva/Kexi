﻿using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowContextMenuPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowContextMenuPopupCommand(Workspace workspace, ContextMenuPopupViewModel contextMenuPopup)
        {
            _workspace        = workspace;
            _contextMenuPopup = contextMenuPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _contextMenuPopup;
            _contextMenuPopup.Open();
        }

        public event EventHandler                  CanExecuteChanged;
        private readonly ContextMenuPopupViewModel _contextMenuPopup;
        private readonly Workspace                 _workspace;
    }
}