using Kexi.ViewModel;

namespace Kexi.UI
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow
    {
        public SetupWindow(Workspace workspace)
        {
            InitializeComponent();
            Loaded += SetupWindow_Loaded;
            DataContext = new SetupViewModel(workspace);
        }

        private void SetupWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DarkThemeButton.Focus();
        }
    }
}