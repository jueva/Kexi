using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DeleteKeyBindingCommand : IKexiCommand
    {
        [ImportingConstructor]
        public DeleteKeyBindingCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is KeyCommandsLister;
        }

        public void Execute(object parameter)
        {
            if (_workspace.CurrentItem is KexBindingItem binding)
            {
                var keyConfiguration = _workspace.KeyDispatcher.Configuration;
                var keyMode = _workspace.Options.KeyMode == KeyMode.ViStyle
                    ? KeyMode.ViStyle
                    : KeyMode.Classic;
                var sourceBindings = keyConfiguration.Bindings.SingleOrDefault(b => b.KeyMode == keyMode)?.KeyBindings;
                var bindings = sourceBindings?.Where(b => b.Equals(binding.Binding)).ToArray();
                if (bindings != null)
                {
                    sourceBindings.RemoveAll(b => b.Equals(binding.Binding));
                    binding.Binding = null;
                }

            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}