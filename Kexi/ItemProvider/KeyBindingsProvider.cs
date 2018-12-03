using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.ItemProvider
{
    [Export]
    public class KeyBindingsProvider
    {
        [ImportingConstructor]
        public KeyBindingsProvider(Workspace workspace, [ImportMany] IEnumerable<IKexiCommand> commands)
        {
            _workspace = workspace;
            _commands  = commands;
        }

        private readonly IEnumerable<IKexiCommand> _commands;
        private readonly Workspace                 _workspace;

        public IEnumerable<KexBindingItem> GetBindings()
        {
            var allCommands = _commands.Select(n => new KexBindingItem(n.GetType().Name, ""));

            var activeBindings = _workspace.KeyDispatcher.ActiveBindings;

            var missing = allCommands.Where(c => activeBindings.All(b => b.CommandName != c.CommandName));
            return activeBindings.Select(b => new KexBindingItem(b)).Concat(missing);
        }

    }
}