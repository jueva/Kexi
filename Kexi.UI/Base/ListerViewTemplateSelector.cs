using System.Windows;
using System.Windows.Controls;

namespace Kexi.View
{
    public class ListerViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            return base.SelectTemplate(item, container);
        }
    }
}
