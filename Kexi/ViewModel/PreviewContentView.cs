using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kexi.Annotations;
using Kexi.Common;
using Kexi.Files;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    public class PreviewContentView : INotifyPropertyChanged
    {
        public PreviewContentView(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }
        public Options   Options   => Workspace.Options;

        public string Path
        {
            get { return path; }
            set
            {
                if (value == path) return;
                path = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string path;

        public Task SetItem(FileItem fileItem)
        {
            var targetResolver = new FileItemTargetResolver(fileItem);
            targetResolver.Parse();
            return Task.Run(() => { Path = targetResolver.TargetPath; });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}