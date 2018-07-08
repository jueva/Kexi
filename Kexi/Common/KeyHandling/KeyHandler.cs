using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;
using Microsoft.CodeAnalysis.Operations;

namespace Kexi.Common.KeyHandling
{
    [Serializable]
    public class KeyHandler
    {
        public const  Key AlternateEscape = Key.Oem102;
        public static Key MoveDownKey     = Key.J;
        public static Key MoveUpKey       = Key.K;
        public static Key MoveLeftKey     = Key.H;
        public static Key MoveRightKey    = Key.L;
        public static string CommanderGroup = "Commandbar";

        public KeyHandler(Workspace workspace)
        {
            Workspace = workspace;
        }


        public Workspace Workspace { get; set; }

        public INotificationHost NotificationHost => Workspace.NotificationHost;

        public CommandRepository CommandRepository => Workspace.CommandRepository;

        internal static         List<KexBinding> Bindings { get; set; }
        private          ICommand         _lastCommand;

        private Key?          firstKey;
        private ModifierKeys? firstModifier;

        public bool Execute(KeyEventArgs args, ILister lister, string group = null)
        {
            var        k            = args.Key == Key.System ? args.SystemKey : args.Key;
            var        modifierKeys = args.KeyboardDevice.Modifiers;
            KexBinding binding      = null;
            var groupName = group ?? lister?.GetType().Name;
            var listerCommands = Bindings.Where(b => b.Group == groupName || b.Group == "");

            if (firstKey != null && !IsModifier(k))
            {
                binding =
                    listerCommands.OfType<KexDoubleBinding>()
                        .FirstOrDefault(b =>
                            b.Key == firstKey && b.Modifier == firstModifier
                            && b.SecondKey == k && b.SecondModifier == modifierKeys
                            );

                if (binding == null && k != Key.Escape)
                {
                    NotificationHost.AddInfo($"Binding {firstKey}-{k} not found");
                    firstKey      = null;
                    firstModifier = null;
                    return true;
                }

                NotificationHost.ClearCurrentMessage();
                firstKey      = null;
                firstModifier = null;
            }
            else
            {
                if (Workspace.CommanderMode) binding = Bindings.FirstOrDefault(b =>b.Group == CommanderGroup && b.Key == k && b.Modifier == modifierKeys);

                if (binding == null)
                    binding = listerCommands.FirstOrDefault(b => b.Key == k && b.Modifier == modifierKeys);

                if (binding is KexDoubleBinding)
                {
                    if (firstKey == null)
                    {
                        firstKey      = k;
                        firstModifier = modifierKeys;
                        NotificationHost.AddInfo("Press second key to perform action");
                        args.Handled = true;
                        return true;
                    }

                    firstKey      = null;
                    firstModifier = null;
                }
            }

            if (binding != null)
            {
                if (binding.Command == null)
                {
                    binding.Command = CommandRepository.GetCommandByName(binding.CommandName);
                }
                try
                {
                    if (binding.Command == CommandRepository.GetCommandByName(nameof(RepeatLastCommandCommand)))
                        _lastCommand?.Execute(lister);
                    else
                        _lastCommand = binding.Command;

                    //Simulated Cursor Down/up brings better Focus/Scrolling/Grouping Handling
                    if (binding.CommandName == nameof(MoveCursorDownCommand)) 
                    {
                        args.Handled = true;
                        KeyEventArgs newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Down);
                        newArgs.RoutedEvent = Keyboard.KeyDownEvent;
                        InputManager.Current.ProcessInput(newArgs);
                        return true;
                    }
                    if (binding.CommandName == nameof(MoveCursorUpCommand))
                    {
                        args.Handled = true;
                        KeyEventArgs newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Up);
                        newArgs.RoutedEvent = Keyboard.KeyDownEvent;
                        InputManager.Current.ProcessInput(newArgs);
                        return true;
                    }
                    var commandArgs = new CommandArgument(lister, binding.CommandArguments);
                    args.Handled = true;
                    binding.Command.Execute(commandArgs);
                    return true;
                }
                catch (Exception ex)
                {
                    NotificationHost.AddError(ex.Message, ex);
                }
            }

            args.Handled = false;
            return false;
        }

        private bool IsModifier(Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return true;
                default:
                    return false;
            }
        }
    }
}