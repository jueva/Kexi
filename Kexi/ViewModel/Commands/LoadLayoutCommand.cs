using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class LoadLayoutCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly DialogPopupViewModel _dialogPopup;

        [ImportingConstructor]
        public LoadLayoutCommand(Workspace workspace, DialogPopupViewModel dialogPopup)
        {
            _workspace = workspace;
            _dialogPopup = dialogPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {            
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            var layouts = Directory.GetFiles(favoriteLocation, "*.ktc").Select(l => new FileInfo(l).Name).ToArray();
            _workspace.PopupViewModel = _dialogPopup;
            _dialogPopup.Open("Select Layout", SelectLayout, layouts);
        }

        private void SelectLayout(string layoutName)
        {
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            var layoutLocation = $@"{favoriteLocation}\{layoutName}";
            _workspace.DockingMananger.DeserializeLayout(layoutLocation);
        }

        public event EventHandler CanExecuteChanged;
    }
}