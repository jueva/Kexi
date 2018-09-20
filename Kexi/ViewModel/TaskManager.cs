using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kexi.Annotations;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    public class TaskManager : INotifyPropertyChanged
    {
        public TaskManager(Workspace workspace)
        {
            _workspace = workspace;
            Tasks      = new ObservableCollection<TaskItem>();
        }

        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }

        private readonly Workspace                      _workspace;
        private          ObservableCollection<TaskItem> _tasks;

        public void Run(TaskItem task, Action action)
        {
            try
            {
                Tasks.Add(task);
                action();
            }
            finally
            {
                Tasks.Remove(task);
            }
        }

        public async Task RunAsync(TaskItem task, Action action)
        {
            try
            {
                Tasks.Add(task);
                await Task.Run(action);
            }
            finally
            {
                Tasks.Remove(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}