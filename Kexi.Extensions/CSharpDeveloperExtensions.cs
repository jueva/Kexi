using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Mono.Cecil;

namespace Kexi.Extensions
{
    [Export(typeof(IExtendedPropertyProvider))]
    [ExportPropertyProvider(typeof(FileItem), "CSharp Dev Extensions", "Csharp")]
    public class CSharpDeveloperExtensions : IExtendedPropertyProvider
    {
        public string Description => "Extensions for CSharp Developers";

        public Task<IEnumerable<PropertyItem>> GetItems(IItem item, Detaillevel details)
        {
            return Task.Run(() =>
            {
                if (item is FileItem fileItem)
                {
                    var path = fileItem.GetPathResolved();
                    if (IsNetAssembly(path))
                    {
                        return GetItemsInternal(path, details);
                    }
                }
                return Enumerable.Empty<PropertyItem>();
            });
        }

        public bool IsMatch(IItem item)
        {
            return item is FileItem fileItem && (fileItem.Extension == ".exe" || fileItem.Extension == ".dll");
        }

        private static IEnumerable<PropertyItem> GetItemsInternal(string path, Detaillevel details)
        {
            if (IsNetAssembly(path))
            {
                var assembly = AssemblyDefinition.ReadAssembly(path);
                yield return new PropertyItem("Assembly Version", assembly.Name.Version);
                var description = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "AssemblyDescriptionAttribute");
                if (description != null)
                    yield return new PropertyItem("Description", description.ConstructorArguments.FirstOrDefault().Value);
                var token = GetPublicKeyToken(assembly);
                if (!string.IsNullOrEmpty(token))
                    yield return new PropertyItem("Public Key Token", token);
                var debugModes = GetDebugInfo(assembly).ToList();
                if (debugModes.Any())
                    yield return new PropertyItem("Debug Attributes", string.Join(Environment.NewLine, debugModes));
                yield return new PropertyItem("Runtime Version", assembly.MainModule.RuntimeVersion);
                yield return new PropertyItem("References", string.Join(Environment.NewLine, assembly.MainModule.AssemblyReferences));
                if (details == Detaillevel.Full)
                    yield return new PropertyItem("Custom Attributes", string.Join(Environment.NewLine, assembly.CustomAttributes.Select(c => c.AttributeType.Name + " = " + c.ConstructorArguments.FirstOrDefault().Value)));
            }
        }

        private static string GetPublicKeyToken(AssemblyDefinition assembly)
        {
            return string.Join("", assembly.Name.PublicKeyToken.Select(b => b.ToString("x2").ToUpperInvariant()));
        }

        private static bool IsNetAssembly(string path)
        {
            var sb = new StringBuilder(256);
            var hr = GetFileVersion(path, sb, sb.Capacity, out _);
            return hr == 0;
        }

        private static IEnumerable<string> GetDebugInfo(ICustomAttributeProvider assembly)
        {
            var debugAttribute = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.FullName == "System.Diagnostics.DebuggableAttribute");
            if (debugAttribute != null)
            {
                var debugModes = (int) debugAttribute.ConstructorArguments.FirstOrDefault().Value;
                if ((debugModes & 1) != 0)
                    yield return "Default";
                if ((debugModes & 2) != 0)
                    yield return "IgnoreSymbolStoreSequencePoints";
                if ((debugModes & 4) != 0)
                    yield return "EnableEditAndContinue";
                if ((debugModes & 256) != 0)
                    yield return "DisableOptimizations";
            }
        }

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern int GetFileVersion(string path, StringBuilder buffer, int buflen, out int written);
    }
}