using System.Windows;
using System.Windows.Controls;
using Kexi.ViewModel.Dock;

namespace Kexi.UI.View
{
    public partial class PreviewerControl
    {
        public PreviewerControl()
        {
            InitializeComponent();
            DataContextChanged += PreviewerControl_DataContextChanged;
        }

        private void PreviewerControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToolPreviewViewModel preview)
            {
                preview.ScrollDownAction = () => scrollViewer.PageDown();
                preview.ScrollUpAction   = () => scrollViewer.PageUp();
            }
        }
    }
}