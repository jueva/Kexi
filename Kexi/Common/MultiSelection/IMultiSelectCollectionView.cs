using System.Windows.Controls.Primitives;

//http://grokys.blogspot.co.at/2010/07/mvvm-and-multiple-selection-part-iii.html
namespace Kexi.Common.MultiSelection
{
    public interface IMultiSelectCollectionView
    {
        void AddControl(Selector selector);
        void RemoveControl(Selector selector);
    }
}
