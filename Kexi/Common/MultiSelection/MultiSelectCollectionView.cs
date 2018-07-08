using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Kexi.Common.MultiSelection
{
    //http://grokys.blogspot.co.at/2010/07/mvvm-and-multiple-selection-part-iii.html
    public class MultiSelectCollectionView<T> : ListCollectionView, IMultiSelectCollectionView
    {
        private readonly List<Selector> _controls = new List<Selector>();

        private bool _ignoreSelectionChanged;

        public MultiSelectCollectionView(IList list)
            : base(list)
        {
            SelectedItems = new ObservableCollection<T>();
        }

        public ObservableCollection<T> SelectedItems { get; }

        void IMultiSelectCollectionView.AddControl(Selector selector)
        {
            _controls.Add(selector);
            SetSelection(selector);
            selector.SelectionChanged += control_SelectionChanged;
        }

        void IMultiSelectCollectionView.RemoveControl(Selector selector)
        {
            if (_controls.Remove(selector)) selector.SelectionChanged -= control_SelectionChanged;
        }

        private void SetSelection(Selector selector)
        {
            var multiSelector = selector as MultiSelector;
            var listBox       = selector as ListBox;

            if (multiSelector != null)
            {
                multiSelector.SelectedItems.Clear();

                foreach (var item in SelectedItems) multiSelector.SelectedItems.Add(item);
            }
            else if (listBox != null)
            {
                listBox.SelectedItems.Clear();

                foreach (var item in SelectedItems) listBox.SelectedItems.Add(item);
            }
        }

        private void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_ignoreSelectionChanged)
            {
                var changed = false;

                _ignoreSelectionChanged = true;

                try
                {
                    foreach (T item in e.AddedItems)
                        if (!SelectedItems.Contains(item))
                        {
                            SelectedItems.Add(item);
                            changed = true;
                        }

                    foreach (T item in e.RemovedItems)
                        if (SelectedItems.Remove(item))
                            changed = true;

                    if (changed)
                        foreach (var control in _controls)
                            if (!Equals(control, sender))
                                SetSelection(control);
                }
                finally
                {
                    _ignoreSelectionChanged = false;
                }
            }
        }
    }
}