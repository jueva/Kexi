using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Shell32;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class ContextMenuPopupViewModel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public ContextMenuPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Context Menu";
        }

        public override void Open()
        {
            var        shell       = new Shell32.Shell();
            var        dir         = Path.GetDirectoryName(Workspace.CurrentItem.Path);
            var        file        = Path.GetFileName(Workspace.CurrentItem.Path);
            var        shellFolder = shell.NameSpace(dir) as Folder3;
            FolderItem folderItem = shellFolder?.ParseName(file) as FolderItem2;
            if (folderItem != null) 
                _verbs = folderItem.Verbs().Cast<FolderItemVerb>().ToList();

            BaseItems =
                _verbs.Where(v => !string.IsNullOrEmpty(v.Name))
                    .Select(v => new BaseItem(v.Name.Replace("&", "")) {Path = v.Name});

            Items = BaseItems;
            SetHeaderIconByKey("appbar_lines_horizontal_4");
            base.Open();
        }

        protected override void ItemSelected(IItem selectedItem)
        {
            var current = Workspace.CurrentItem;
            var verb = _verbs.First(v => v.Name == selectedItem.Path);
            verb.DoIt();
            Workspace.ActiveLister.Refresh();

            var currentItem = Workspace.CurrentItems.FirstOrDefault(i => i.Path == current.Path);
            Workspace.FocusItem(currentItem);
            IsOpen = false;
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            var filtered = new ItemFilter<IItem>(BaseItems, Text);
            if (!filtered.Any())
            {
                ea.Handled = true;
                if (Text.Any())
                {
                    Text = Text.Substring(0, Text.Length - 1);
                    SetCaret(Text.Length);
                }
                return;
            }

            Items = filtered;
        }

        private  List<FolderItemVerb> _verbs;
    }
}