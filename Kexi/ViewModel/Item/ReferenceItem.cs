using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.Shell;
using Mono.Cecil;

namespace Kexi.ViewModel.Item
{
    public class ReferenceItem : BaseItem
    {
        public ReferenceItem(string assemblyPath, string rootpath, string assembly, AssemblyNameReference r)
            : this(assemblyPath, rootpath, assembly, r.Name, r.Version, r.Attributes, r.Culture, r.PublicKey, r.PublicKeyToken)
        {
        }

        public ReferenceItem(string assemblyPath, string rootpath, string assembly, string name, Version version, AssemblyAttributes attributes, string culture, byte[] publicKey, byte[] publicKeyToken)
        {
            FilterString = DisplayName = name;
            _assembly = assembly;
            _assemblyPath = assemblyPath;
            _version = version.ToString();
            _attributes = attributes;
            _culture = string.IsNullOrEmpty(culture) ? "Neutral" : culture;
            _publicKey = getToken(publicKey);
            _publicKeyToken = getToken(publicKeyToken);
            _assemblyDirectory = rootpath;
            _relativePath = assemblyPath.Substring(rootpath.Length);
            _relativePath = assemblyPath.Substring(rootpath.Length);
            var target = System.IO.Path.Combine(_assemblyDirectory,name+".dll");
            if (File.Exists(target))
            {
                _targetPath = target;
                Path = target;
                void a()
                {
                    Thumbnail = new NativeFileInfo(target).Icon;
                    Thumbnail.Freeze();
                }
                Task.Factory.StartNew(a);
                ItemType = ItemType.Container;
            }

        }


        private string getToken(byte[] token)
        {
            var builder = new StringBuilder();
            foreach(var b in token)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        public AssemblyAttributes Attributes
        {
            get { return _attributes; }
            set
            {
                if (value == _attributes) return;
                _attributes = value;
                OnPropertyChanged();
            }
        }

        public string Culture
        {
            get { return _culture; }
            set
            {
                if (value == _culture) return;
                _culture = value;
                OnPropertyChanged();
            }
        }

        public string PublicKey
        {
            get { return _publicKey; }
            set
            {
                if (Equals(value, _publicKey)) return;
                _publicKey = value;
                OnPropertyChanged();
            }
        }

        public string PublicKeyToken
        {
            get { return _publicKeyToken; }
            set
            {
                if (Equals(value, _publicKeyToken)) return;
                _publicKeyToken = value;
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (value == _version)
                    return;
                _version = value;
                OnPropertyChanged();
            }
        }

        public string Assembly
        {
            get { return _assembly; }
            set
            {
                if (value == _assembly) return;
                _assembly = value;
                OnPropertyChanged();
            }
        }

        private string _version;
        private string _assembly;
        private string _assemblyPath;

        public string AssemblyPath
        {
            get { return _assemblyPath; }
            set
            {
                if (value == _assemblyPath) return;
                _assemblyPath = value;
                OnPropertyChanged();
            }
        }

        private AssemblyAttributes _attributes;
        private string _culture;
        private string _publicKey;
        private string _publicKeyToken;
        private string _assemblyDirectory;
        private string _relativePath;
        private string _targetPath;

        public string TargetPath
        {
            get { return _targetPath; }
            set
            {
                if (value == _targetPath) return;
                _targetPath = value;
                OnPropertyChanged();
            }
        }

        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                if (value == _relativePath) return;
                _relativePath = value;
                OnPropertyChanged();
            }
        }

        public string AssemblyDirectory
        {
            get { return _assemblyDirectory; }
            set
            {
                if (value == _assemblyDirectory) return;
                _assemblyDirectory = value;
                OnPropertyChanged();
            }
        }

    }
}
