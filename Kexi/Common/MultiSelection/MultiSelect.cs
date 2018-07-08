using System.Windows;
using System.Windows.Controls.Primitives;

namespace Kexi.Common.MultiSelection
{
    //http://grokys.blogspot.co.at/2010/07/mvvm-and-multiple-selection-part-iii.html
    public static class MultiSelect
    {
        static MultiSelect()
        {
            Selector.ItemsSourceProperty.OverrideMetadata(typeof(Selector), new FrameworkPropertyMetadata(ItemsSourceChanged));
        }

        public static bool GetIsEnabled(Selector target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Selector target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MultiSelect), 
                new UIPropertyMetadata(IsEnabledChanged));

        static void IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            IMultiSelectCollectionView collectionView = selector.ItemsSource as IMultiSelectCollectionView;

            if (selector != null && collectionView != null)
            {
                if ((bool)e.NewValue)
                {
                    collectionView.AddControl(selector);
                }
                else
                {
                    collectionView.RemoveControl(selector);
                }
            }
        }

        static void ItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = sender as Selector;

            var oldCollectionView = e.OldValue as IMultiSelectCollectionView;
            if (GetIsEnabled(selector))
            {
                var newCollectionView = e.NewValue as IMultiSelectCollectionView;

                if (oldCollectionView != null)
                {
                    oldCollectionView.RemoveControl(selector);
                }

                if (newCollectionView != null)
                {
                    newCollectionView.AddControl(selector);
                }
            }
        }
    }
}
