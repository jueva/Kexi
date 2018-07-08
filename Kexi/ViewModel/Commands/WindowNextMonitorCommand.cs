using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class WindowNextMonitorCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public WindowNextMonitorCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var all = Screen.AllScreens;
            var s = UIHelper.GetCurrentScreen();
            int i;
            for (i = 0; i < all.Length; i++)
                if (all[i].DeviceName == s.DeviceName)
                {
                    i++;
                    break;
                }

            if (i > all.Length - 1) i = 0;
            UIHelper.CenterToScreen(all[i]);
        }

        public event EventHandler CanExecuteChanged;
    }
}