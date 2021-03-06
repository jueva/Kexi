﻿using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Popup
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Serializer needs")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification         = "Serializer needs")]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SetKeyBindingPopupViewModel : PopupViewModel<BaseItem>
    {
        [ImportingConstructor]
        public SetKeyBindingPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler,
            PropertyEditorPopupViewModel propertyEditorPopup) : base(workspace, options, mouseHandler)
        {
            HideInputAtStartup = true;
            TitleVisible       = true;
        }

        public  KexBindingItem BindingItem   { get; set; }
        public  KexBinding     SourceBinding => BindingItem.Binding;
        public  string         CommandName   => BindingItem.CommandName;
        public  KexBinding     Binding       { get; set; }
        public  string         Group         { get; set; }
        private bool           _selectListers;

        public override void Open()
        {
            Title          = "Press Key then Return to finish";
            BaseItems      = null;
            _selectListers = false;
            base.Open();
        }

        public override void Close()
        {
            Text = "";
            base.Close();
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            var key = ea.Key == Key.System ? ea.SystemKey : ea.Key;
            if (key.IsModifier() || _selectListers)
            {
                base.PreviewKeyDown(sender, ea);
                return;
            }

            switch (key)
            {
                case Key.Escape:
                    Binding = null;
                    Close();
                    break;
                case Key.Return:
                    SelectListers();
                    break;
                default:
                    if (Binding == null)
                    {
                        Binding = new KexBinding(Group, key, ea.KeyboardDevice.Modifiers, CommandName, null);
                    }
                    else if (!(Binding is KexDoubleBinding))
                    {
                        Text    += ", ";
                        Binding =  new KexDoubleBinding(Binding.Group, Binding.Key, Binding.Modifier, key, ea.KeyboardDevice.Modifiers, CommandName, null);
                    }
                    else
                    {
                        ea.Handled = true;
                        return;
                    }

                    if (ea.KeyboardDevice.Modifiers != ModifierKeys.None)
                        Text += $"{ea.KeyboardDevice.Modifiers}+{key}";
                    else
                        Text += key;
                    SetCaret(Text.Length);
                    break;
            }

            ea.Handled = true;
        }

        private void SelectListers()
        {
            if (Binding == null)
                return;

            _selectListers = true;
            Text           = "";
            Title          = "Choose Target Lister";
            var allListers = KexContainer.ResolveMany<ILister>()
                .Where(l => !string.IsNullOrEmpty(l.Title))
                .Where(l => l.ShowInMenu).Select(l => new BaseItem(l.Title) {Path = l.GetType().Name});

            var items = new[] {new BaseItem("All") {Path = null}}.Concat(allListers).ToList();
            BaseItems = items;
            var index = items.FindIndex(i => i.Path == SourceBinding?.Group);
            ItemsView.MoveCurrentToPosition(index);
        }

        protected override void ItemSelected(BaseItem selectedItem)
        {
            Binding.Group = selectedItem.Path;
            var keyConfiguration = Workspace.KeyDispatcher.Configuration;
            var keyMode = Options.KeyMode == KeyMode.ViStyle
                ? KeyMode.ViStyle
                : KeyMode.Classic;

            var sourceBindings = keyConfiguration.Bindings.Single(b => b.KeyMode == keyMode).KeyBindings;
            if (sourceBindings.Contains(SourceBinding) && SourceBinding.Group == selectedItem.Path)
            {
                sourceBindings.Remove(SourceBinding);
            }

            sourceBindings.Add(Binding);

            if (BindingItem != null)
            {
                BindingItem.Binding = Binding;
            }

            KeyConfigurationSerializer.SaveConfiguration(keyConfiguration);
            base.ItemSelected(selectedItem);
            Close();
        }
    }
}