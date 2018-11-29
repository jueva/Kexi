using System;
using System.Windows;
using System.Windows.Threading;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;
using Xceed.Wpf.AvalonDock;

namespace Kexi.UI.View
{
    public partial class DockManager
    {
        public DockManager()
        {
            InitializeComponent();
            dockingManager.DocumentClosing += DockingManager_DocumentClosing;
            DataContextChanged             += DockManager_DataContextChanged;
        }

        public DockingManager DockingManager => dockingManager;

        private DockViewModel Model => DataContext as DockViewModel;
        private DockViewModel _oldModel;

        private void DockManager_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_oldModel != null)
                _oldModel.PropertyChanged -= ViewModel_PropertyChanged;
            _oldModel                  =  Model;
            Model.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DockViewModel.DockWidth))
            {
                LeftAnchorPane.DockWidth = RightAnchorPane.DockWidth = Model.DockWidth;
            }
        }

        private void DockingManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            e.Document.CanClose = false;
            if (e.Document.Content is DocumentViewModel documentModel)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => RemoveDocumentFromCollection(documentModel)), DispatcherPriority.Background);
            }
        }

        private void RemoveDocumentFromCollection(DocumentViewModel documentModel)
        {
            var workspace = DataContext as Workspace;
            workspace?.Docking.Files.Remove(documentModel);
        }
    }
}