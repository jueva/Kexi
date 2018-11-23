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
            this.DataContextChanged += DockManager_DataContextChanged;
        }

        private void DockManager_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (oldModel != null)
                oldModel.PropertyChanged -= ViewModel_PropertyChanged;
            oldModel = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DockViewModel.DockWidth))
            {
                LeftAnchorPane.DockWidth = RightAnchorPane.DockWidth = viewModel.DockWidth;
            }
        }

        private DockViewModel viewModel => DataContext as DockViewModel;
        private DockViewModel oldModel;

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
