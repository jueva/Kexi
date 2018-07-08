using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveCursorDownCommand : IKexiCommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //Key bound to this Command is catched and replaced with CursorDown in Keyhandler
            //var uie = Keyboard.FocusedElement as UIElement;
            //uie?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        }

        public event EventHandler CanExecuteChanged;
    }
}