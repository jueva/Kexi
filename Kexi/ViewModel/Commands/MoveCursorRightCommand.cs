using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveCursorRightCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public MoveCursorRightCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var uie = Keyboard.FocusedElement as UIElement;
            uie?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
        }

        public event EventHandler CanExecuteChanged;
    }
}