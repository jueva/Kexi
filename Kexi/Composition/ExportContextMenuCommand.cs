using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Kexi.Composition
{
    public interface IExportCommandMetadata
    {
        Type TargetListerType { get;  }
        string Name { get;  }
        string Key { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false), MetadataAttribute]
    public class ExportContextMenuCommand: ExportAttribute, IExportCommandMetadata
    {
        public ExportContextMenuCommand(Type targetListerType, string name, string key = null) : base(typeof(ICommand))
        {
            TargetListerType = targetListerType;
            Name = name;
            Key = key ?? name.Replace(" ","");
        }

        public Type TargetListerType { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

    }
}
