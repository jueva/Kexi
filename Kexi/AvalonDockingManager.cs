using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;
using Kexi.ViewModel.Lister;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Kexi
{
    public class AvalonDockingManager : IDockingManager
    {
        public AvalonDockingManager(Workspace workspace, DockingManager dockingManager)
        {
            _workspace      = workspace;
            _dockingManager = dockingManager;
            //_dockingManager.DocumentClosed += _dockingManager_DocumentClosed;
        }

        private void _dockingManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            var parent = e.Document?.Parent;
            if (parent != null)
            {
                var group = parent as LayoutDocumentPaneGroup;
                group?.RemoveChild(e.Document);
            }

        }

        public void SerializeLayout(string file)
        {
            var documents = _dockingManager.Layout.RootPanel.Descendents().OfType<LayoutDocument>();
            foreach (var d in documents)
            {
                if (d.Content is ILister lister)
                    d.ContentId = lister.Path;
                if (d.Content is LayoutDocument layoutDocument)
                    if (layoutDocument.Content is ILister subLister)
                        d.ContentId = subLister.Path;
            }

            ;
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
            if (!(e.Model is LayoutDocument document))
                return;
            var lister = KexContainer.Resolve<FileLister>();
            e.Content = new DocumentViewModel
            {
                Content   = lister,
                ContentId = Guid.NewGuid().ToString()
            };
            await LoadIt(lister, document);
        }

        private async Task LoadIt(ILister lister, LayoutDocument doc)
        {
            lister.Path = doc.ContentId;
            await lister.Refresh(); //ensure this has a Task
        }

    }
}