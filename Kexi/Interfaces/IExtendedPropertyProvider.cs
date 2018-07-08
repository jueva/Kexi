using System.Collections.Generic;
using System.Threading.Tasks;
using Kexi.ViewModel.Item;

namespace Kexi.Interfaces
{
    public interface IExtendedPropertyProvider
    {
        string Description { get; }
        Task<IEnumerable<PropertyItem>> GetItems(IItem path);
    }
}