using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Popup
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Serializer needs")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Serializer needs")]

    [Export]
    public class SetKeyBindingPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public SetKeyBindingPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler,
            PropertyEditorPopupViewModel propertyEditorPopup) : base(workspace, options, mouseHandler)
        {
            Title              = "Press Key then Return to finish";
            HideInputAtStartup = true;
            TitleVisible       = true;
        }

        public KexBinding SourceBinding { get; set; }
        public KexBinding Binding     { get; set; }
        public string     CommandName { get; set; }
        public string     Group       { get; set; }

        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            if (ea.Key.IsModifier())
            {
                return;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ea.Key)
            {
                case Key.Escape:
                    Binding = null;
                    Close();
                    break;
                case Key.Return:
                    SaveAndClose();
                    break;
                default:
                    Binding = Binding == null
                        ? new KexBinding(Group, ea.Key, ea.KeyboardDevice.Modifiers, CommandName, null)
                        : new KexDoubleBinding(Binding.Group, Binding.Key, Binding.Modifier, ea.Key, ea.KeyboardDevice.Modifiers, CommandName, null);

                    Text += $"{ea.KeyboardDevice.Modifiers}+{ea.Key}";
                    break;
            }

            ea.Handled = true;
        }

        private void SaveAndClose()
        {
            if (Binding == null)
                return;

            var keyConfiguration = Workspace.KeyDispatcher.Configuration;
            var keyMode = Options.KeyMode == KeyMode.ViStyle 
                ? KeyMode.ViStyle 
                : KeyMode.Classic;
            var sourceBindings = keyConfiguration.Bindings.SingleOrDefault(b => b.KeyMode == keyMode)?.KeyBindings;
            if (sourceBindings != null)
            {
                if (sourceBindings.Contains(SourceBinding))
                {
                    sourceBindings.Remove(SourceBinding);
                }
                sourceBindings.Add(Binding);
            }
           
            new KeyConfigurationSerializer().SaveConfiguration(keyConfiguration);
            Close();
        }
    }
}