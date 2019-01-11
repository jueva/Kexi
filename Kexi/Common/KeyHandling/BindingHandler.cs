using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class BindingHandler
    {
        private static readonly string CommanderGroup = "Commandbar";

        public BindingHandler(Workspace workspace, List<KexBinding> bindings)
        {
            _workspace = workspace;
            _bindings  = bindings;
        }

        private readonly List<KexBinding> _bindings;
        private readonly Workspace        _workspace;
        private          Key?             _firstKey;
        private          ModifierKeys?    _firstModifier;

        public bool Handle(KeyEventArgs args, ILister lister)
        {
            var        key              = args.Key == Key.System ? args.SystemKey : args.Key;
            var        modifierKeys   = args.KeyboardDevice.Modifiers;
            KexBinding binding        = null;
            var        groupName      = lister?.GetType().Name;
            var        listerCommands = _bindings.Where(b => b.Group == groupName || string.IsNullOrEmpty(b.Group));
            if (key.IsModifier())
                return true;

            if (_firstKey != null)
            {
                binding = GetDoubleBinding(listerCommands, key, modifierKeys, out binding);
                if (binding == null)
                {
                    args.Handled = true;
                    return true;
                }
            }
            else
            {
                if (_workspace.CommanderMode) 
                    binding = _bindings.FirstOrDefault(b => b.Group == CommanderGroup && b.Key == key && b.Modifier == modifierKeys);

                if (binding == null)
                    binding = listerCommands.FirstOrDefault(b =>
                        (b.Key == key && b.Modifier == modifierKeys && !(b is KexDoubleBinding))
                        || (b.Key == key && b.Modifier == modifierKeys));

                if (binding is KexDoubleBinding)
                {
                    _firstKey      = key;
                    _firstModifier = modifierKeys;
                    _workspace.NotificationHost.AddInfo("Press second key to perform action");
                    args.Handled = true;
                    return true;
                }
            }

            if (binding != null)
            {
                if (binding.Command == null) binding.Command = _workspace.CommandRepository.GetCommandByName(binding.CommandName);
                try
                {
                    //if (binding.Command == _workspace.CommandRepository.GetCommandByName(nameof(RepeatLastCommandCommand)))
                    //{
                    //    _workspace.CommandRepository.Execute(_workspace.CommandRepository.LastCommand);
                    //    args.Handled = true;
                    //    return true;
                    //}

                    switch (binding.CommandName)
                    {
                        //Simulated Cursor Down/up brings better Focus/Scrolling/Grouping Handling
                        case nameof(MoveCursorDownCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? throw new InvalidOperationException(), 0, Key.Down) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(newArgs);
                            EnsureFocus();
                            return true;
                        }
                        case nameof(MoveCursorUpCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? throw new InvalidOperationException(), 0, Key.Up) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(newArgs);
                            EnsureFocus();
                            return true;
                        }
                    }

                    var commandArgs = new CommandArgument(lister, binding.CommandArguments);
                    _workspace.CommandRepository.Execute(binding.Command, commandArgs);
                    args.Handled = true;
                    return true;
                }
                catch (Exception ex)
                {
                    _workspace.NotificationHost.AddError(ex.Message, ex);
                }
            }

            args.Handled = false;
            return false;
        }

        private KexBinding GetDoubleBinding(IEnumerable<KexBinding> listerCommands, Key key, ModifierKeys modifierKeys, out KexBinding binding)
        {
            _workspace.NotificationHost.ClearCurrentMessage();
            binding = listerCommands.OfType<KexDoubleBinding>().FirstOrDefault(b =>
                b.Key == _firstKey && b.Modifier == _firstModifier
                && b.SecondKey == key && b.SecondModifier == modifierKeys
            );

            if (binding == null)
            {
                _workspace.NotificationHost.AddInfo($"Binding {_firstKey}-{key} not found");
            }

            _firstKey      = null;
            _firstModifier = null;

            return binding;
        }

        private void EnsureFocus()
        {
            if (!_workspace.GetSelection<IItem>().Any())
                _workspace.FocusCurrentOrFirst();
        }
    }
}