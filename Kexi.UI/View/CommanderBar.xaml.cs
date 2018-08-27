using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.UI.Base;
using Kexi.View;
using Kexi.ViewModel;

namespace Kexi.UI.View
{
    /// <summary>
    /// Interaction logic for CommanderBar.xaml
    /// </summary>

    public partial class CommanderBar : BaseControl<CommanderbarViewModel>
    {

        public CommanderBar()
        {
            InitializeComponent();
        }

        private void _commanderBar_OnKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.Workspace.KeyDispatcher.Execute(e, null, "Commandbar");
        }
    }
}
