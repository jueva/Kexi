using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SpecialFolderPopupViewModel : PopupViewModel<BaseItem>
    {
        private static readonly Lazy<IEnumerable<BaseItem>> AllFolders = new Lazy<IEnumerable<BaseItem>>(
            () =>
            {
                var folders = Enum.GetNames(typeof(Environment.SpecialFolder));
                return folders.Select(GetBaseItem).Where(f => f != null);
            });

        [ImportingConstructor]
        public SpecialFolderPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Folders";
            SetHeaderIconByKey("appbar_folder");
        }

        public override void Open()
        {
            BaseItems = AllFolders.Value.ToList();
            base.Open();
        }

        private static BaseItem GetBaseItem(string s)
        {
            try
            {
                var b = new BaseItem(s);
                var enumValue =
                    (Environment.SpecialFolder) Enum.Parse(typeof(Environment.SpecialFolder), b.DisplayName);
                b.Path      = Environment.GetFolderPath(enumValue);
                b.Thumbnail = ShellNative.GetLargeBitmapSource(b.Path);
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void ItemSelected(BaseItem selectedItem)
        {
            var enumValue =
                (Environment.SpecialFolder) Enum.Parse(typeof(Environment.SpecialFolder), selectedItem.DisplayName);
            var path = Environment.GetFolderPath(enumValue);
            Workspace.ActiveLister.Path = path;
            base.ItemSelected(selectedItem);
            Text   = "";
            IsOpen = false;
        }
    }
}