using System;

namespace Kexi.ViewModel.Item
{
    public class TaskItem : BaseItem
    {
        public TaskItem(string displayName, Action action) : base(displayName)
        {
            Action = action;
        }

        public Action Action { get; }
    }
}