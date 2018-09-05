using System.IO;
using System.Xml.Serialization;

namespace Kexi.Common.KeyHandling
{
    public class KeyConfigurationSerializer
    {
        public const string KeyConfiguration = @".\keyBindings.xml";

        public KeyConfiguration GetConfiguration()
        {
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(KeyConfiguration));
                return (KeyConfiguration) serializer.Deserialize(file);
            }
        }

        public void SaveConfiguration(KeyConfiguration configuration)
        {
            var serializer = new XmlSerializer(typeof(KeyConfiguration));
            using (TextWriter writer = new StreamWriter(KeyConfiguration))
            {
                serializer.Serialize(writer, configuration);
                writer.Close();
            }
        }
    }
}