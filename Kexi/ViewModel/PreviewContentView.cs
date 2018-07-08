using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kexi.Annotations;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.TreeView;

namespace Kexi.ViewModel
{
    public class PreviewContentView : INotifyPropertyChanged
    {
        public PreviewContentView(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; }
        public Options Options => Workspace.Options;

        public async Task SetItem(FileItem fileItem)
        {
            var targetResolver = new FileItemTargetResolver(fileItem);
            targetResolver.Parse();
            await Task.Run(() => { Path = targetResolver.TargetPath; });
        }

        private string path;

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
