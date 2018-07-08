using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Composition;
using Kexi.Files;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;
using Microsoft.WindowsAPICodePack.Shell;
using Mono.Cecil;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(FileLister), "File Properties")]
    public class FilePropertyProvider : BasePropertyProvider<FileItem>
    {
        [ImportingConstructor]
        public FilePropertyProvider(Workspace workspace) : base(workspace)
        {
            Width = 100;
        }


        private bool IsMusic    => "music".Equals(GetKind());
        private bool IsPicture  => "picture".Equals(GetKind());
        private bool IsExeOrDll => Item?.Extension == ".exe" || Item?.Extension == ".dll";

        public override void Dispose()
        {
            _shellObject?.Dispose();
            _shellObject = null;
            base.Dispose();
        }

        private ShellObject _shellObject;

        private string GetKind()
        {
            if (Item == null || _shellObject == null)
                return null;

            try
            {
                return !(_shellObject?.Properties?.System?.Kind?.ValueAsObject is string[] kind) || kind.Length == 0
                    ? null
                    : kind[0];
            }
            catch
            {
                return null;
            }
        }

        public override Task SetItem(FileItem item)
        {
            _shellObject = ShellObject.FromParsingName(item.Path);
            return base.SetItem(item);
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            if (Item.IsNetwork()) return await GetNetworkTopItems();

            var tempProp = new ObservableCollection<PropertyItem>();
            if ((CancellationTokenSource?.IsCancellationRequested ?? true) || Item == null)
                return tempProp;

            return await Task.Run(() =>
            {
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.DisplayName));
                tempProp.Add(new PropertyItem("Type", Item.Details.Type));
                return tempProp;
            }, CancellationTokenSource.Token);
        }

        protected override async Task<BitmapSource> GetThumbnail()
        {
            if (Item.IsNetwork())
            {
                ThumbMaxHeight = 80;
                return Item.Details.Thumbnail;
            }

            ThumbMaxHeight = IsPicture ? 120 : 80;
            await Item.Details.SetLargeThumbAsync();
            return Item.Details.LargeThumbnail;
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (CancellationTokenSource?.IsCancellationRequested ?? Item == null)
                return new ObservableCollection<PropertyItem>();

            if (Item.IsNetwork()) return await GetNetworkBottomItems();

            async Task<ObservableCollection<PropertyItem>> FetchProperties()
            {
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", Item.Details.Created),
                    new PropertyItem("Last Modified", Item.Details.LastModified)
                };

                var props = new Files.FilePropertyProvider(_shellObject);

                if (Item.IsLink())
                {
                    var target = new FileItemTargetResolver(Item);
                    tempProp.Add(new PropertyItem("Target:", target.TargetPath));
                }

                if (IsMusic)
                {
                    tempProp.Add(new PropertyItem("Title", _shellObject.Properties.System.Title.ValueAsObject));
                    if (_shellObject.Properties?.System?.Author?.ValueAsObject is string[] authors)
                        tempProp.Add(new PropertyItem("Author(s)", string.Join(",", authors)));
                    tempProp.Add(new PropertyItem("Rating", _shellObject.Properties?.System?.Rating?.ValueAsObject));
                }

                if (IsPicture)
                {
                    tempProp.Add(new PropertyItem("Dimensions", props.GetValue("System.Image.Dimensions")));
                    foreach (var key in props.Items.Keys.Where(k => k.StartsWith("System.Photo")).OrderBy(k => k))
                        tempProp.Add(new PropertyItem(key.Substring(13), props.GetValue(key)));
                }

                if (IsExeOrDll && IsNetAssembly(await PathResolved()))
                    try
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(Item.Path);
                        tempProp.Add(new PropertyItem("Product Name", props.GetValue("System.Software.ProductName")));
                        tempProp.Add(new PropertyItem("Product Version", props.GetValue("System.Software.ProductVersion")));
                        tempProp.Add(new PropertyItem("Assembly Version", assembly.Name.Version));
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

                return tempProp;
            }

            return await Task.Run(FetchProperties);
        }

        private static IEnumerable<string> GetDebugInfo(AssemblyDefinition assembly)
        {
            var debugAttribute = assembly.CustomAttributes.FirstOrDefault(c => c.AttributeType.FullName == "System.Diagnostics.DebuggableAttribute");
            if (debugAttribute != null)
            {
                var debugModes = (int) debugAttribute.ConstructorArguments.FirstOrDefault().Value;
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

        private async Task<string> PathResolved()
        {
            return await Task.Run(() => Item.GetPathResolved());
        }

        private static bool IsNetAssembly(string path)
        {
            var sb = new StringBuilder(256);
            var hr = GetFileVersion(path, sb, sb.Capacity, out _);
            return hr == 0;
        }

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern int GetFileVersion(string path, StringBuilder buffer, int buflen, out int written);

        private async Task<ObservableCollection<PropertyItem>> GetNetworkTopItems()
        {
            var tempProp = new ObservableCollection<PropertyItem>();
            if (CancellationTokenSource.IsCancellationRequested)
                return tempProp;

            return await Task.Run(() =>
            {
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.Name));
                tempProp.Add(new PropertyItem("Type", Item.Details.Type));
                return tempProp;
            }, CancellationTokenSource.Token);
        }

        private async Task<ObservableCollection<PropertyItem>> GetNetworkBottomItems()
        {
            if (CancellationTokenSource.IsCancellationRequested || Item == null)
                return new ObservableCollection<PropertyItem>();

            return await Task.Run(() =>
            {
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Location", "Network"),
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", Item.Details.Created),
                    new PropertyItem("Last Modified", Item.Details.LastModified)
                };

                return tempProp;
            }, CancellationTokenSource.Token);
        }
    }
}