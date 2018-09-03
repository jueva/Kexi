using System.ComponentModel.Composition;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [Export]
    public class SetKeyBindingPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public SetKeyBindingPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler, PropertyEditorPopupViewModel propertyEditorPopup) : base(workspace, options, mouseHandler)
        {
            Title              = "Press Key then Return to finish";
            HideInputAtStartup = true;
            TitleVisible       = true;
        }

        public KexBinding Binding     { get; private set; }
        public string     CommandName { get; set; }
        public string     Group       { get; set; }

        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            if (ea.Key.IsModifier())
            {
                return;
            }

            if (ea.Key == Key.Escape)
            {
                Binding = null;
                Close();
            }
            else if (ea.Key == Key.Return)
            {
                Close();
            }
            else
            {
                Binding = Binding == null
                    ? new KexBinding(Group, ea.Key, ea.KeyboardDevice.Modifiers, CommandName, null)
                    : new KexDoubleBinding(Binding.Group, Binding.Key, Binding.Modifier, ea.Key, ea.KeyboardDevice.Modifiers, CommandName, null);

                Text += $"{ea.KeyboardDevice.Modifiers}+{ea.Key}";
            }

            ea.Handled = true;
        }

    }
}