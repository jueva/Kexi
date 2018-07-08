using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kexi.ViewModel;
using Kexi.ViewModel.Dock;

//https://wpffilepreviewer.codeplex.com/SourceControl/latest#WpfDocumentPreviewer/PreviewControl.xaml.cs

namespace Kexi.View
{
    /// <summary>
    ///     Interaction logic for PreviewerControl.xaml
    /// </summary>
    public partial class PreviewerControl : UserControl, INotifyPropertyChanged
    {
        public PreviewerControl()
        {
            InitializeComponent();
            DataContextChanged += PreviewerControl_DataContextChanged;
        }

        public string FileName
        {
            get => Path.GetFileName(fileName);
            set
            {
                fileName = value;
                SetFileName(fileName);
                RaisePropertyChanged(nameof(ImageSource));
                RaisePropertyChanged(nameof(FileName));
            }
        }

        public ImageSource ImageSource => IconFromFileName(fileName);

        private PreviewContentView ViewModel => DataContext as PreviewContentView;
        private string             fileName;

        private void SetFileName(string fileName)
        {
            try
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    Task.Run(() =>
                    {
                        if (previewHandlerHost1.Open(fileName))
                            wfh1.Visibility = Visibility.Visible;
                    });
                });
            }
            catch
            {
                //Ignore
            }
        }

        private void PreviewerControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ToolPreviewViewModel model)
                if (model.Content is PreviewContentView content)
                    content.PropertyChanged += D_PropertyChanged;
        }

        private void D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.PropertyName == nameof(PreviewContentView.Path)) SetFileName(ViewModel.Path);
            });
        }

        internal static BitmapSource IconFromFileName(string fileName)
        {
            var bmpImage = new BitmapImage();
            if (fileName != null && fileName.Contains("."))
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(fileName);
                    var bmp  = icon.ToBitmap();
                    var strm = new MemoryStream();
                    bmp.Save(strm, ImageFormat.Png);
                    bmpImage.BeginInit();
                    strm.Seek(0, SeekOrigin.Begin);
                    bmpImage.StreamSource = strm;
                    bmpImage.EndInit();
                }
                catch
                {
                    //Ignore
                }

            return bmpImage;
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}