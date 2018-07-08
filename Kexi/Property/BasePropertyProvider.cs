using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.Property
{
    public abstract class BasePropertyProvider<T> : ViewModelBase, IPropertyProvider<T>, IDisposable
        where T : class, IItem
    {
        protected BasePropertyProvider(Workspace workspace)
        {
            Workspace        = workspace;
            ThumbMaxHeight = 80;
            PropertiesTop    = new ObservableCollection<PropertyItem>();
            PropertiesBottom = new ObservableCollection<PropertyItem>();
        }

        // ReSharper disable once MemberCanBeProtected.Global, used in Databinding
        // ReSharper disable once UnusedAutoPropertyAccessor.Global, used in Databinding
        public Workspace Workspace { get; }

        public virtual void Dispose()
        {
            CancellationTokenSource?.Dispose();
            Item      = null;
            Thumbnail = null;
        }

        public ObservableCollection<PropertyItem> PropertiesTop
        {
            get => _propertiesTop;
            private set
            {
                _propertiesTop = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PropertyItem> PropertiesBottom
        {
            get => _propertiesBottom;
            private set
            {
                _propertiesBottom = value;
                OnPropertyChanged();
            }
        }

        IItem IPropertyItemContainer.Item => Item;

        public T Item
        {
            get => _item;
            protected set
            {
                if (Equals(value, _item))
                    return;
                _item = value;
                OnPropertyChanged();
            }
        }

        public async Task SetItem(IItem item)
        {
            await SetItem(item as T);
        }

        public virtual async Task SetItem(T item)
        {
            //TODO: Check if this has to be done over Dispatcher
            Item             = item;
            Thumbnail = await GetThumbnail();
            PropertiesTop    = await GetTopItems();
            PropertiesBottom = await GetBottomItems();

            var additional = KexContainer.Container.InnerCompositionContainer.GetExports<IExtendedPropertyProvider, IExportPropertyProviderMetadata>()
                .Where(i => i.Metadata.TargetListerType == typeof(T));

            foreach (var provider in additional)
            {
                var items = await provider.Value.GetItems(item);
                foreach (var i in items)
                    PropertiesBottom.Add(i);
            }
        }

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            private set
            {
                _thumbnail = value;
                OnPropertyChanged();
            }
        }

        public double ThumbMaxHeight
        {
            get => _thumbMaxHeight;
            protected set
            {
                if (value.Equals(_thumbMaxHeight)) return;
                _thumbMaxHeight = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            protected set
            {
                if (value.Equals(_width)) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        public int RotateThumb
        {
            get => _rotateThumb;
            protected set
            {
                if (value.Equals(_rotateThumb)) return;
                _rotateThumb = value;
                OnPropertyChanged();
            }
        }

        public  CancellationTokenSource            CancellationTokenSource { get; set; }
        private T                                  _item;
        private ObservableCollection<PropertyItem> _propertiesBottom;
        private ObservableCollection<PropertyItem> _propertiesTop;
        private int                                _rotateThumb;
        private double                             _thumbMaxHeight;
        private BitmapSource                       _thumbnail;
        private double _width;

        protected abstract Task<ObservableCollection<PropertyItem>> GetTopItems();
        protected abstract Task<ObservableCollection<PropertyItem>> GetBottomItems();
        protected abstract Task<BitmapSource> GetThumbnail();
    }

}