using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.View
{
    /// <summary>
    /// Interaction logic for StatusBarView.xaml
    /// </summary>
    public partial class StatusBarView : UserControl
    {
        public StatusBarView()
        {
            InitializeComponent();
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            if (DataContext is ILister lister)
            {
                var workspace = lister.Workspace;
                new ClearFilterCommand(workspace).Execute();
            }
        }
    }
}