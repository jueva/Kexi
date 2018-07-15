using System.ComponentModel.Composition;
using System.Windows.Controls;
using Kexi.Common;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class ConsoleCommandPopupViewModel : PopupViewModel<ConsoleCommandItem>
    {
        [ImportingConstructor]
        public ConsoleCommandPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title              = "Command";
            TitleVisible = true;
            HideInputAtStartup = false;
        }

        public override void Open()
        {
            var consoleLister = Workspace.ActiveLister as ConsoleLister;
            BaseItems = consoleLister?.CommandHistory;
            SetHeaderIconByKey("appbar_console");
            base.Open();
        }

        protected override void ItemSelected(ConsoleCommandItem selectedItem)
        {
            CommandRepository.GetCommandByName(nameof(ExecuteConsoleCommand)).Execute(ItemSelectedFromListView
                ? selectedItem?.Command
                : Text);
            Text = "";
            base.ItemSelected(selectedItem);
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            if (Items == null || ItemsView == null)
                return;

            var filtered = new ItemFilter<ConsoleCommandItem>(BaseItems, Text);

            Items = filtered;
            FocusInput();
            UnSelectListView();
            ItemSelectedFromListView = false;
        }
    }
}