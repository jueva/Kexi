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
            var allBindings = _workspace.KeyDispatcher.Bindings;
            //foreach (var c in allCommands.ToArray())
            //{
            //    //TODO: SecondKey
            //    foreach (var b in allBindings.Where(bi => bi.CommandName == c.CommandName && bi.Group != c.Lister)) 
            //    {
            //        allCommands.Remove(c);
            //        allCommands.Add(new KexBindingItem(b));
            //    }
            //}

            var missing = allCommands.Where(c => allBindings.All(b => b.CommandName != c.CommandName));
            return allBindings.Select(b => new KexBindingItem(b)).Concat(missing);
        }
    }
}