using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kexi.View;

namespace Kexi.Interfaces
{
    public interface IListerView : IDisposable
    {
        KexiListView ListView           { get;  set; }
        GridViewColumnHeader CurrentSortColumn  { get; set; }
        SortAdorner          CurrentSortAdorner { get; set; }
        void FocusCurrentOrFirst();
        Task ShowDetail();
        void FocusItem(IItem selected);
    }
}