namespace Kexi.ViewModel
{
    public class CommanderbarViewModel : ViewModelBase
    {
        public CommanderbarViewModel(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }
    }
}