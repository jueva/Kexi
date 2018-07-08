using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Serialization;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class LoadKeybindingsCommand : IKexiCommand
    {
        public const string KeyConfiguration = @".\keyBindings.xml";

        [ImportingConstructor]
        public LoadKeybindingsCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter = null)
        {
            var serializer = new XmlSerializer(typeof(List<KexBinding>));
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                KeyHandler.Bindings = (List<KexBinding>) serializer.Deserialize(file);
            }
        }

        public event EventHandler  CanExecuteChanged;
    }
}