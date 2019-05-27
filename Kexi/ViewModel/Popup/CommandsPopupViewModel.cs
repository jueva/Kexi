using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Kexi.Common;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class CommandsPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public CommandsPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title     = "Commands";
        }

        public override void Open()
        {
            BaseItems = CommandRepository.CommandCache.Value.Select(c => new BaseItem(GetName(c.Key), c.Key, c.Key));
            SetHeaderIconByKey("appbar_control_guide");
            base.Open();
        }

        protected override void ItemSelected(BaseItem selectedItem)
        {
            IsOpen = false;
            CommandRepository.Execute(selectedItem.Path);
        }

        internal string GetName(string name)
        {
            var matches = Regex.Matches(name, "[A-Z][^A-Z]*").Cast<Match>()
                .Where(m => m.Value.ToLower() != "command")
                .Select(m => m.Value.Trim());
            return string.Join(" ", matches);
        }
    }
}