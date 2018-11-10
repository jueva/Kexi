﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class OpenPathInConsoleCommand : IKexiCommand
    {
        [ImportingConstructor]
        public OpenPathInConsoleCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.CurrentItem is FileItem fileItem)
            {
                var path = fileItem.ItemType == ItemType.Container
                    ? fileItem.Path
                    : _workspace.ActiveLister.Path;
                var psi = new ProcessStartInfo("cmd")
                {
                    WorkingDirectory = path
                };
                var p = new Process {StartInfo = psi};
                p.Start();
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}