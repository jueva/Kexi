﻿using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class FocusPreviousConsoleCommand : IKexiCommand
    {
        [ImportingConstructor]
        public FocusPreviousConsoleCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.ActiveLister is ConsoleLister console)
                if (console.FocusPreviousConsoleCommand.CanExecute(parameter))
                    console.FocusPreviousConsoleCommand.Execute(parameter);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}