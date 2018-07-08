using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Kexi.UI.Base;
using Kexi.View;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;
using Xceed.Wpf.AvalonDock;

namespace Kexi.UI.View
{
    /// <summary>
    /// Interaction logic for DockManager.xaml
    /// </summary>
    public partial class DockManager : BaseControl<Workspace>
    {
        public DockingManager DockingManager => dockingManager;

        public DockManager()
        {
            InitializeComponent();
            dockingManager.DocumentClosing += DockingManager_DocumentClosing;
        }

        private void DockingManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            e.Document.CanClose = false;
            if (e.Document.Content is DocumentViewModel documentModel)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => RemoveDocumentFromCollection(documentModel) ), DispatcherPriority.Background);
            }     
        }

        private void RemoveDocumentFromCollection(DocumentViewModel documentModel)
        {
            var workspace = DataContext as Workspace;
            workspace?.Docking.Files.Remove(documentModel);
        }
    }
}
