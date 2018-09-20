namespace Kexi.ViewModel.Item
{
    public class TaskItem : BaseItem
    {
        public TaskItem(string displayName) : base(displayName)
        {
        }

        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress == value)
                    return;

                _progress = value;
                OnPropertyChanged();
            }
        }

        private int _progress;
    }
}