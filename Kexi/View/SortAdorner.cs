using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kexi.View
{
    public class SortAdorner : Adorner
    {
        private static readonly Geometry _AscGeometry =
            Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private static readonly Geometry _DescGeometry =
            Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        private readonly ListSortDirection Direction;

        public SortAdorner(UIElement element, ListSortDirection direction)
            : base(element)
        {
            Direction = direction;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            drawingContext.PushTransform(
                new TranslateTransform(
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2));

            drawingContext.DrawGeometry(Brushes.DarkGray, null,
                Direction == ListSortDirection.Ascending ? _AscGeometry : _DescGeometry);

            drawingContext.Pop();
        }
    }
}