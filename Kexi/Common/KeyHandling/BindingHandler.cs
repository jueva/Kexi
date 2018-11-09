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

        public IKexiCommand LastCommand { get; private set; }

        public bool Handle(KeyEventArgs args, ILister lister, string group = null)
        {
            var        k              = args.Key == Key.System ? args.SystemKey : args.Key;
            var        modifierKeys   = args.KeyboardDevice.Modifiers;
            KexBinding binding        = null;
            var        groupName      = group ?? lister?.GetType().Name;
            var        listerCommands = _bindings.Where(b => b.Group == groupName || string.IsNullOrEmpty(b.Group));

            if (_firstKey != null && !k.IsModifier())
            {
                binding =
                    listerCommands.OfType<KexDoubleBinding>()
                        .FirstOrDefault(b =>
                            b.Key == _firstKey && b.Modifier == _firstModifier
                            && b.SecondKey == k && b.SecondModifier == modifierKeys
                        );

                if (binding == null && k != Key.Escape)
                {
                    _workspace.NotificationHost.AddInfo($"Binding {_firstKey}-{k} not found");
                    _firstKey      = null;
                    _firstModifier = null;
                    args.Handled = true;
                    return true;
                }

                _workspace.NotificationHost.ClearCurrentMessage();
                _firstKey      = null;
                _firstModifier = null;
            }
            else
            {
                if (_workspace.CommanderMode) binding = _bindings.FirstOrDefault(b => b.Group == CommanderGroup && b.Key == k && b.Modifier == modifierKeys);

                if (binding == null)
                    binding = listerCommands.FirstOrDefault(b => b.Key == k && b.Modifier == modifierKeys);

                switch (binding)
                {
                    case KexDoubleBinding _ when _firstKey == null:
                        _firstKey      = k;
                        _firstModifier = modifierKeys;
                        _workspace.NotificationHost.AddInfo("Press second key to perform action");
                        args.Handled = true;
                        return true;
                    case KexDoubleBinding _:
                        _firstKey      = null;
                        _firstModifier = null;
                        break;
                }
            }

            if (binding != null)
            {
                if (binding.Command == null) binding.Command = _workspace.CommandRepository.GetCommandByName(binding.CommandName);
                try
                {
                    if (binding.Command == _workspace.CommandRepository.GetCommandByName(nameof(RepeatLastCommandCommand)))
                        LastCommand?.Execute(lister);
                    else
                        LastCommand = binding.Command;

                    switch (binding.CommandName)
                    {
                        //Simulated Cursor Down/up brings better Focus/Scrolling/Grouping Handling
                        case nameof(MoveCursorDownCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? throw new InvalidOperationException(), 0, Key.Down) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(newArgs);
                            return true;
                        }
                        case nameof(MoveCursorUpCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? throw new InvalidOperationException(), 0, Key.Up) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(newArgs);
                            return true;
                        }
                    }

                    var commandArgs = new CommandArgument(lister, binding.CommandArguments);
                    args.Handled = true;
                    binding.Command.Execute(commandArgs);
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

    }
}