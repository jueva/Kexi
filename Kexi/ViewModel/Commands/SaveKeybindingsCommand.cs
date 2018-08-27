using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class SaveKeybindingsCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportMany]
        public IEnumerable<KeyDispatcher> keyhandlers { get; private set; }


        [ImportingConstructor]
        public SaveKeybindingsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

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

        public event EventHandler CanExecuteChanged;
    }
}