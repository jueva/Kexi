using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class CreateElementPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public CreateElementPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            TitleVisible = true;
            Title        = "New";
        }

        public override void Open()
        {
            BaseItems = new[]
            {
                new BaseItem("Directory"),
                new BaseItem("File"),
                new BaseItem(".txt"),
                new BaseItem(".docx")
            };
            HideInputAtStartup = false;
            SetHeaderIconByKey("appbar_add");
            base.Open();
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            //Dont Filter List Items
        }

        protected override void ItemSelected(BaseItem selectedItem)
        {
            var itemName = Text;
            try
            {
                if (Workspace.ActiveLister is FileLister fileLister) fileLister.Items.CollectionChanged += Items_CollectionChanged;

                var path = Path.Combine(Workspace.ActiveLister.Path, itemName);
                if (selectedItem.Path.StartsWith("."))
                {
                    if (!path.EndsWith(selectedItem.Path))
                        path += selectedItem.Path;
                    if (File.Exists(path))
                        Workspace.NotificationHost.AddError("File already exists");
                    else
                        using (File.Create(path, 100, FileOptions.Asynchronous))
                        {
                        }
                }
                else
                {
                    switch (selectedItem.Path)
                    {
                        case "Directory":
                            Directory.CreateDirectory(path);
                            break;
                        case "File":
                            if (File.Exists(path))
                                Workspace.NotificationHost.AddError("File already exists");
                            else
                                using (File.Create(path, 100, FileOptions.Asynchronous))
                                {
                                }

                            break;
                    }
                }

                IsOpen = false;
            }
            catch (Exception ex)
            {
                Workspace.NotificationHost.AddError("Error creating Item", ex);
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ObservableCollection<FileItem> items))
                return;

            var newItem = e.NewItems[0] as FileItem;
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0) Workspace.FocusItem(newItem);
            items.CollectionChanged -= Items_CollectionChanged;
        }
    }
}