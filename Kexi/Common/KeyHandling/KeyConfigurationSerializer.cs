using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kexi.Common.KeyHandling
{
    public class KeyConfigurationSerializer
    {
        public const string KeyConfiguration = @".\keyBindings.xml";

        public KeyConfiguration GetConfiguration()
        {
            var serializer = new XmlSerializer(typeof(KeyConfiguration));
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                var configuration = (KeyConfiguration) serializer.Deserialize(file);
                return configuration;
            }
        }

        public void SaveConfiguration()
        {
            var serializer = new XmlSerializer(typeof(KeyConfiguration));
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                var configuration = (KeyConfiguration) serializer.Deserialize(file);
                //TODO: Saveit
            }
        }
    }
}
