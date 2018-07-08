using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.Composition
{
    public interface IExportItemDetailsMetadata
    {
        Type   TargetItemType { get; }
        string Name           { get; }
        string Key            { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    [MetadataAttribute]
    public class ExportItemDetails : ExportAttribute, IExportItemDetailsMetadata
    {
        public ExportItemDetails(Type targetItemType, string name, string key = null) : base(typeof(IPropertyItemContainer))
        {
            TargetItemType = targetItemType;
            Name           = name;
            Key            = key ?? name.Replace(" ", "");
        }

        public Type   TargetItemType { get; set; }
        public string Name           { get; set; }
        public string Key            { get; set; }
    }
}