using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;
using Kexi.ViewModel.Lister;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Kexi
{
    public class DockingSerializer : IDockingManager
    {
        public DockingSerializer(Workspace workspace, DockingManager dockingManager)
        {
            _workspace      = workspace;
            _dockingManager = dockingManager;
        }

        public void SerializeLayout(string file)
        {
            var documents = _dockingManager.Layout.RootPanel.Descendents().OfType<LayoutDocument>();
            foreach (var d in documents)
            {
                if (d.Content is DocumentViewModel viewModel)
                    d.ContentId = viewModel.Content.Path;
                if (d.Content is ILister lister)
                    d.ContentId = lister.Path;
                if (d.Content is LayoutDocument layoutDocument)
                    if (layoutDocument.Content is ILister subLister)
                        d.ContentId = subLister.Path;
            }

            var serializer = new XmlLayoutSerializer(_dockingManager);
            serializer.Serialize(file);
        }

        public void DeserializeLayout(string file)
        {
            _workspace.Docking.Files.Clear();
            var serializer = new XmlLayoutSerializer(_dockingManager);
            serializer.LayoutSerializationCallback += Serializer_LayoutSerializationCallback;
            serializer.Deserialize(file);
            _workspace.HasMultipleTabs = _dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().Count() > 1;
            _workspace.SetTabsVisibility();
            if (_workspace.PopupViewModel?.IsOpen ?? false)
                _workspace.PopupViewModel.IsOpen = false;
        }

        private readonly DockingManager _dockingManager;
        private readonly Workspace      _workspace;

        private async void Serializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            switch (e.Model)
            {
                case LayoutAnchorable anchorable:
                    switch (anchorable.ContentId)
                    {
                        case ToolExplorerViewModel.Identifier:
                            _workspace.Docking.ExplorerViewModel.IsVisible = anchorable.IsVisible;
                            e.Content = _workspace.Docking.ExplorerViewModel;
                            break;
                        case ToolDetailViewModel.Identifier:
                            _workspace.Docking.DetailViewModel.IsVisible = anchorable.IsVisible;
                            e.Content = _workspace.Docking.DetailViewModel;
                            break;
                        case ToolPreviewViewModel.Identifier:
                            _workspace.Docking.PreviewViewModel.IsVisible = anchorable.IsVisible;
                            e.Content = _workspace.Docking.PreviewViewModel;
                            break;
                    }
                    break;
                case LayoutDocument document:
                {
                    var lister = KexContainer.Resolve<FileLister>();


                    e.Content = new DocumentViewModel
                    {
                        Content   = lister,
                        ContentId = Guid.NewGuid().ToString()
                    };
                    await LoadIt(lister, document);
                    break;
                }
            }
        }

        private async Task LoadIt(ILister lister, LayoutDocument doc)
        {
            lister.Path = doc.ContentId;
            if (doc.IsSelected && doc.IsActive)
            {
                lister.GotItems         += Lister_GotItems;
                _workspace.ActiveLister =  lister;
            }
            await lister.Refresh(); //ensure this has a Task
        }
        private void Lister_GotItems(object sender, EventArgs e)
        {
            if (sender is ILister lister)
            {
                lister.GotItems -= Lister_GotItems;
                lister.View?.ListView?.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action) (() => lister.View?.FocusCurrentOrFirst()));
            }
        }
    }
}