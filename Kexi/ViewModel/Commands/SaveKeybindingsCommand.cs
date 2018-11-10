using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class SaveKeybindingsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public SaveKeybindingsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        [ImportMany]
        public IEnumerable<KeyDispatcher> keyhandlers { get; private set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //TODO: Keymode...
            //var allcommands = KeyHandler.Bindings;
            //var serializer = new XmlSerializer(allcommands.GetType());
            //using (var file = new FileStream(@".\keyBindings.xml", FileMode.Create))
            //{
            //    serializer.Serialize(file, allcommands);
            //    file.Flush();
            //}
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}