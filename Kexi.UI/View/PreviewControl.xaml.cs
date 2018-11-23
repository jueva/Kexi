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