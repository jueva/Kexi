using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kexi.Annotations;

namespace Kexi.Common
{
    public class PathPart : INotifyPropertyChanged
    {
        private string _name;
        private string _path;
        private bool   _first;
        private bool   _highlighted;

        public PathPart(string name, string path)
        {
            Name = name;
            try
            {
                Path = System.IO.Path.GetDirectoryName(path) ?? path;
            }
            catch (Exception)
            {
                Path = path;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
            }
        }

        public bool First
        {
            get { return _first; }
            set
            {
                if (value == _first) return;
                _first = value;
                OnPropertyChanged();
            }
        }

        public bool Highlighted
        {
            get { return _highlighted; }
            set
            {
                if (value == _highlighted) return;
                _highlighted = value;
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