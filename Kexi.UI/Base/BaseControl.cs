using System.Windows.Controls;

namespace Kexi.UI.Base
{
    public class BaseControl<T> : UserControl
        where T : class
    {
        public T ViewModel => DataContext as T;
    }
}