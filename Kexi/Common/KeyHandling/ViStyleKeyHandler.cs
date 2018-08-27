using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class ViStyleKeyHandler : IKeyHandler
    {
        private readonly Workspace _workspace;
        private Key?          firstKey;
        private ModifierKeys? firstModifier;
        private ICommand _lastCommand;
        private static string CommanderGroup = "Commandbar";
        private const string KeyConfiguration = @".\keyBindings.xml";

        public ViStyleKeyHandler(Workspace workspace)
        {
            _workspace = workspace;
            var serializer = new XmlSerializer(typeof(List<KexBinding>));
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                Bindings = (List<KexBinding>) serializer.Deserialize(file);
            }
        }
         public  List<KexBinding> Bindings { get; }

         public bool Execute(KeyEventArgs args, ILister lister, string group = null)
        {
            var        k              = args.Key == Key.System ? args.SystemKey : args.Key;
            var        modifierKeys   = args.KeyboardDevice.Modifiers;
            KexBinding binding        = null;
            var        groupName      = group ?? lister?.GetType().Name;
            var        listerCommands = Bindings.Where(b => b.Group == groupName || b.Group == "");

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
                    _workspace.NotificationHost.AddInfo($"Binding {firstKey}-{k} not found");
                    firstKey      = null;
                    firstModifier = null;
                    return true;
                }

                _workspace.NotificationHost.ClearCurrentMessage();
                firstKey      = null;
                firstModifier = null;
            }
            else
            {
                if (_workspace.CommanderMode) binding = Bindings.FirstOrDefault(b => b.Group == CommanderGroup && b.Key == k && b.Modifier == modifierKeys);

                if (binding == null)
                    binding = listerCommands.FirstOrDefault(b => b.Key == k && b.Modifier == modifierKeys);

                switch (binding)
                {
                    case KexDoubleBinding _ when firstKey == null:
                        firstKey      = k;
                        firstModifier = modifierKeys;
                        _workspace.NotificationHost.AddInfo("Press second key to perform action");
                        args.Handled = true;
                        return true;
                    case KexDoubleBinding _:
                        firstKey      = null;
                        firstModifier = null;
                        break;
                }
            }

            if (binding != null)
            {
                if (binding.Command == null) binding.Command = _workspace.CommandRepository.GetCommandByName(binding.CommandName);
                try
                {
                    if (binding.Command == _workspace.CommandRepository.GetCommandByName(nameof(RepeatLastCommandCommand)))
                        _lastCommand?.Execute(lister);
                    else
                        _lastCommand = binding.Command;

                    switch (binding.CommandName)
                    {
                        //Simulated Cursor Down/up brings better Focus/Scrolling/Grouping Handling
                        case nameof(MoveCursorDownCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Down) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(newArgs);
                            return true;
                        }
                        case nameof(MoveCursorUpCommand):
                        {
                            args.Handled = true;
                            var newArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Up) {RoutedEvent = Keyboard.KeyDownEvent};
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

        public string SearchString { get; set; }

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