using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Kexi.Interfaces;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class OpenDirectorySplittedCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public OpenDirectorySplittedCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var documentPaneGroup1 = _workspace.ActiveLayoutDocument?.FindParent<LayoutDocumentPaneGroup>();
            if (documentPaneGroup1?.ChildrenCount < 4)
            {
                new OpenDirectoryCommand(_workspace).Execute(parameter);
                new WindowSplitVerticalCommand(_workspace).Execute(parameter);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}