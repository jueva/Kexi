using System;

namespace Kexi.Common
{
    public class ConfigurationDropdownValuesAttribute : Attribute
    {
        public string[] Values { get; set; }
    }
}