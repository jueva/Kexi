using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;

namespace Kexi
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class KexContainer : IDisposable
    {
        private const           string               PluginDirectory = "plugins";
        private static readonly CompositionContainer CompositionContainer;

        private static readonly Lazy<KexContainer> LazyContainer = new Lazy<KexContainer>(() => new KexContainer(), true);

        static KexContainer()
        {
            var cats     = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Kexi")).Select(a => new AssemblyCatalog(a));
            var catalogs = new List<ComposablePartCatalog>(cats);

            if (Directory.Exists(PluginDirectory))
                catalogs.Add(new DirectoryCatalog(PluginDirectory));

            var catalog = new AggregateCatalog(catalogs);
            CompositionContainer = new CompositionContainer(catalog);
        }

        public static KexContainer Container => LazyContainer.Value;

        public CompositionContainer InnerCompositionContainer => CompositionContainer;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static void Dispose(bool disposing)
        {
            if (disposing) CompositionContainer?.Dispose();
        }

        public void Compose(object value)
        {
            CompositionContainer.ComposeParts(value);
        }

        public static T Resolve<T>()
        {
            var export = CompositionContainer?.GetExport<T>();
            if (export == null)
                throw new CompositionException($"Cant Resolve {typeof(T)}");
            return export.Value;
        }

        public static IEnumerable<T> ResolveMany<T>()
        {
            return CompositionContainer.GetExports<T>().Select(i => i.Value);
        }

        public static IEnumerable<Lazy<T1>> ResolveMany<T1, T2>()
        {
            return CompositionContainer.GetExports<T1, T2>();
        }
    }
}