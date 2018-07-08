using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Kexi.Common;

namespace Kexi.ViewModel
{
    public class RenamePopupViewModel : ViewModelBase
    {
        private bool _isOpen;

        public RenamePopupViewModel(Workspace workspace)
        {
            Workspace  = workspace;
            FontHelper = new FontHelper(workspace.Options);
        }

        public Workspace  Workspace  { get; }
        public FontHelper FontHelper { get; }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(PlacementTarget));
                OnPropertyChanged(nameof(HorizontalOffset));
                OnPropertyChanged();
            }
        }

        public UIElement PlacementTarget  {
            get
            {
                var view = Workspace.ActiveLister?.View.ListView;
                return view?.ItemContainerGenerator.ContainerFromItem(view.SelectedItem) as UIElement;
            }
        }

        public double HorizontalOffset => GetNameColumnPosition();

        private double GetNameColumnPosition()
        {
            double nameColumnPosition = 0;
            if (Workspace.ActiveLister?.View.ListView.View is GridView gridView)
            {
                var nameColumnFound = false;
                foreach (var col in gridView.Columns)
                {
                    if (col.DisplayMemberBinding is Binding binding && binding.Path.Path == "DisplayName")
                    {
                        nameColumnFound = true;
                        break;
                    }

                    nameColumnPosition += col.ActualWidth;
                }

                if (!nameColumnFound)
                {
                    nameColumnPosition = gridView.Columns.First().Width;
                }
                return nameColumnPosition;
            }

            return Double.NaN;
        }
    }
}