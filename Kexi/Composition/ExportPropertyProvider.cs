using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.Composition
{
    public interface IExportPropertyProviderMetadata
    {
        Type TargetListerType { get; }
        string Name { get; }
        string Key { get; }
    }

    [AttributeUsage(AttributeTargets.Class), MetadataAttribute]

    public class ExportPropertyProvider : ExportAttribute, IExportPropertyProviderMetadata
    {
        public ExportPropertyProvider(Type targetListerType, string name, string key = null) : base(typeof(IPropertyProvider))
        {
            TargetListerType = targetListerType;
            Name             = name;
            Key              = key ?? name.Replace(" ","");
        }

        public Type TargetListerType { get; }
        public string Name { get; }
        public string Key { get;  }
    }

}
