﻿using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class RepeatLastCommandCommand : IKexiCommand
    {
        [ImportingConstructor]
        public RepeatLastCommandCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CommandRepository.LastCommand != null;
        }

        public void Execute(object parameter)
        {
            _workspace.CommandRepository.Execute(_workspace.CommandRepository.LastCommand);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}