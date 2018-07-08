using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class DialogPopupViewModel : PopupViewModel<IItem>
    {
        private Action<string> _completeAction;

        [ImportingConstructor]
        public DialogPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            TitleVisible    = true;
        }

        public void Open(string title, Action<string> completeAction, params string[] options)
        {
            Title           = title;
            BaseItems       = options.Select(i => new BaseItem(i));
            _completeAction = completeAction;
            SetHeaderIconByKey("appbar_question");
            base.Open();
        }

        public override void ItemSelected(IItem selectedItem)
        {
            _completeAction(selectedItem.DisplayName);
            IsOpen = false;
        }
    }
}