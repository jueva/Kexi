using Kexi.ViewModel.Dock;

namespace Kexi.UI.View
{
    public partial class ItemDetailView
    {
        public ItemDetailView()
        {
            InitializeComponent();
            DataContextChanged += ItemDetailView_DataContextChanged;
        }

        private void ItemDetailView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToolDetailViewModel detail)
            {
                detail.ScrollDownAction = () => scrollViewer.PageDown();
                detail.ScrollUpAction   = () => scrollViewer.PageUp();
            }
        }
    }
}