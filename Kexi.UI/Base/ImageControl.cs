using System.Windows;
using System.Windows.Controls;


/// <summary>
/// http://stackoverflow.com/questions/23107920/animate-image-when-loaded-in-image-control-wpf
/// </summary>
namespace Kexi.UI.Base
{
    public class ImageControl : Image
    {
        public static readonly RoutedEvent SourceChangedEvent = EventManager.RegisterRoutedEvent(
      "SourceChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ImageControl));

        static ImageControl()
        {
            Image.SourceProperty.OverrideMetadata(typeof(ImageControl), new FrameworkPropertyMetadata(SourcePropertyChanged));
        }

        public event RoutedEventHandler SourceChanged
        {
            add { AddHandler(SourceChangedEvent, value); }
            remove { RemoveHandler(SourceChangedEvent, value); }
        }

        private static void SourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj == null)
                return;
            Image image = obj as Image;
            image?.RaiseEvent(new RoutedEventArgs(SourceChangedEvent));
        }
    }
}
