using System.IO;
using System.Xml.Serialization;

namespace Kexi.Common.KeyHandling
{
    public static class KeyConfigurationSerializer
    {
        private const string KeyConfiguration = @".\keyBindings.xml";

        public static KeyConfiguration GetConfiguration()
        {
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(KeyConfiguration));
                return (KeyConfiguration) serializer.Deserialize(file);
            }
        }

        public static void SaveConfiguration(KeyConfiguration configuration)
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