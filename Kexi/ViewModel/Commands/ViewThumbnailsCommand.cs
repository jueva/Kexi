using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.View;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ViewThumbnailsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ViewThumbnailsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            new ViewHandler(_workspace.ActiveLister).SetThumbView();
            _workspace.FocusCurrentOrFirst();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}