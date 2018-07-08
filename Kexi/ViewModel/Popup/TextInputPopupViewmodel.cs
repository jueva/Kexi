using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class TextInputPopupViewmodel : PopupViewModel<IItem>
    {
        [ImportingConstructor]
        public TextInputPopupViewmodel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            HideInputAtStartup = false;
        }

        private Action<string> _completeAction;

        public void Open(string title, Action<string> completeAction)
        {
            SetHeaderIconByKey("appbar_interface_textbox");
            _completeAction = completeAction;
            Title           = title;
            TitleVisible    = true;
            base.Open();
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
        }

        public override void ItemSelected(IItem selectedItem)
        {
            var param = selectedItem != null ? selectedItem.DisplayName : Text;
            _completeAction(param);
            Text   = "";
            IsOpen = false;
        }
    }
}