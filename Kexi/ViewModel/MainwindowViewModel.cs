using Kexi.ViewModel.Dock;

namespace Kexi.ViewModel
{
    public class MainwindowViewModel : ViewModelBase
    {
        public Workspace          Workspace          { get; set; }
        public AdressbarViewModel AdressbarViewModel { get; set; }
        public StatusbarViewModel StatusbarViewModel { get; set; }
        public DockViewModel      DockViewModel      { get; set; }
    }
}