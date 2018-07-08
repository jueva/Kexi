using System;
using System.ComponentModel;

namespace Kexi.Interfaces
{
    public interface IItem : INotifyPropertyChanged
    {
        string DisplayName { get; set; }
        string FilterString { get; set; }
        string Path { get; set; }
        ItemType ItemType { get; set; }
        Func<ItemType> TargetType { get; }
        bool Enabled { get; set; }
        bool Highlighted { get; set; }
    }

}
