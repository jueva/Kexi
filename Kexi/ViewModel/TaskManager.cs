using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    public class TaskManager : ViewModelBase
    {
        public TaskManager(Workspace workspace)
        {
            _workspace = workspace;
            Tasks      = new ObservableCollection<TaskItem>();
        }

        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            private set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }

        private readonly Workspace                      _workspace;
        private          ObservableCollection<TaskItem> _tasks;

        public void Run(TaskItem task)
        {
            try
            {
                Tasks.Add(task);
                task.Action();
            }
            finally
            {
                Tasks.Remove(task);
            };
        }

        public async Task RunAsync(TaskItem task)
        {
            try
            {
                Tasks.Add(task);
                await Task.Run(task.Action);
            }
            finally
            {
                Tasks.Remove(task);
            }
        }

        public void RemoveTask(TaskItem task)
        {
            Tasks.Remove(task);
        }
    }
}