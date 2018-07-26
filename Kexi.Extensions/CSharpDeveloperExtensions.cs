using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;
using Mono.Cecil;

namespace Kexi.Extensions
{
    [Export(typeof(IExtendedPropertyProvider))]
    [ExportPropertyProvider(typeof(FileItem), "CSharp Dev Extensions")]
    public class CSharpDeveloperExtensions : IExtendedPropertyProvider
    {
        public string Description => "Extensions for CSharp Developers";

        public async Task<IEnumerable<PropertyItem>> GetItems(IItem item)
        {
            var tempProp = new ObservableCollection<PropertyItem>();
            if (item is FileItem fileItem && (fileItem.Extension == ".exe" || fileItem.Extension == ".dll"))
            {
                var path = await PathResolved(fileItem);
                if (IsNetAssembly(path))
                    try
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(path);
                        tempProp.Add(new PropertyItem("Assembly Version", assembly.Name.Version));
                        var description = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "AssemblyDescriptionAttribute");
                        if (description != null)
                            tempProp.Add(new PropertyItem("Description", description.ConstructorArguments.FirstOrDefault().Value));
                        var debugModes = GetDebugInfo(assembly).ToList();
                        if (debugModes.Any())
                            tempProp.Add(new PropertyItem("Debug Attributes", string.Join(Environment.NewLine, debugModes)));
                        tempProp.Add(new PropertyItem("Runtime Version", assembly.MainModule.RuntimeVersion));
                        tempProp.Add(new PropertyItem("References", string.Join(Environment.NewLine, assembly.MainModule.AssemblyReferences)));
                        tempProp.Add(new PropertyItem("Custom Attributes", string.Join(Environment.NewLine, assembly.CustomAttributes.Select(c => c.AttributeType.Name + " = " + c.ConstructorArguments.FirstOrDefault().Value))));
                    }
                    catch
                    {
                        //Probably no .net assembly
                        //TODO: is there a way to check before read?
                    }
            }

            return await Task.FromResult(tempProp);
        }
        private async Task<string> PathResolved(FileItem item)
        {
            return await Task.Run(() => item.GetPathResolved());
        }

        private static bool IsNetAssembly(string path)
        {
            var sb = new StringBuilder(256);
            var hr = GetFileVersion(path, sb, sb.Capacity, out _);
            return hr == 0;
        }

        private static IEnumerable<string> GetDebugInfo(AssemblyDefinition assembly)
        {
            var debugAttribute = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.FullName == "System.Diagnostics.DebuggableAttribute");
            if (debugAttribute != null)
            {
                var debugModes = (int)debugAttribute.ConstructorArguments.FirstOrDefault().Value;
                if ((debugModes & 1) > 0)
                    yield return "Default";
                if ((debugModes & 2) > 0)
                    yield return "IgnoreSymbolStoreSequencePoints";
                if ((debugModes & 4) > 0)
                    yield return "EnableEditAndContinue";
                if ((debugModes & 256) > 0)
                    yield return "DisableOptimizations";
            }
        }

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern int GetFileVersion(string path, StringBuilder buffer, int buflen, out int written);
    }
}
