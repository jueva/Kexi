﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class WindowMaximizeCommand : IKexiCommand
    {
        [ImportingConstructor]
        public WindowMaximizeCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}