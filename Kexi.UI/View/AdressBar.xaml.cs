using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Kexi.Common;
using Kexi.View;
using Kexi.ViewModel;
using System.Windows.Controls;

namespace Kexi.UI.View
{
    public partial class AdressBar : UserControl
    {
        public AdressBar()
        {
            InitializeComponent();
        }

        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            ((AdressbarViewModel)DataContext).SearchBox_OnKeyDown(sender, e);
        }
    }
}
