using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Kexi.Common;
using Kexi.Converter;
using Kexi.ViewModel.Lister;

namespace Kexi.View
{
    public class ViewHandler
    {
        public static readonly string[] Views = {"Details", "Icons", "Thumbs"};

        public ViewHandler(ILister lister)
        {
            _lister   = lister;
            _listView = Application.Current.MainWindow;
        }

        private readonly ILister _lister;
        private readonly Window  _listView;

        public void SetIconView()
        {
            _lister.CurrentViewMode = ViewType.Icon;
        }

        public void SetThumbView()
        {
            _lister.CurrentViewMode = ViewType.Thumbnail;
        }

        public void SetDetailsView()
        {
            _lister.CurrentViewMode = ViewType.Detail;
        }

        public ViewBase GetIconView()
        {
            var xamlView = _listView.FindResource("iconView") as ViewBase;
            return xamlView;
        }

        public ViewBase GetThumbView()
        {
            var xamlView = _listView.FindResource("thumbView") as ViewBase;
            return xamlView;
        }

        public ViewBase GetDetailView1()
        {
            var xamlView = _listView.FindResource("detailView") as ViewBase;
            return xamlView;
        }

        public GridView GetDetailView()
        {
            var lister   = _lister;
            var gridView = new GridView();

            if (lister != null)
            {
                var st          = lister.Options.HeadersVisible ? "gridHeaderStyle" : "gridHeaderHiddenStyle";
                var headerStyle = _listView.FindResource(st) as Style;
                gridView.ColumnHeaderContainerStyle = headerStyle;
                foreach (var col in lister.Columns)
                {
                    var gc = new GridViewColumn();

                    var header = new GridViewColumnHeader();
                    header.Content = col.Header;
                    gc.Header      = header;
                    switch (col.Type)
                    {
                        case ColumnType.Image:
                        {
                            gc.CellTemplate = GetImageDataTemplate(col);
                            if (col.BindingExpression == "Thumbnail" || col.BindingExpression == "Details.Thumbnail")
                            {
                                gc.Width = lister.Options.FontSize * 1.8;
                            }

                            break;
                        }
                        case ColumnType.Bool:
                            gc.CellTemplate = GetBoolCellTemplate(col);
                            break;
                        case ColumnType.Number:
                        case ColumnType.RightAligned:
                            gc.CellTemplate = GetRightAlignDataTemplate(col);
                            break;
                        case ColumnType.Highlightable:
                            gc.CellTemplate = GetHighlightableTemplate(col);
                            break;
                        case ColumnType.SyntaxHighlighted:
                            gc.CellTemplate = GetSyntaxHighlightedTemplate();
                            break;
                        default:
                            gc.DisplayMemberBinding = GetBinding(col);
                            break;
                    }

                    switch (col.Size)
                    {
                        case ColumnSize.FullWidth:
                        {
                            //TODO: Hack
                            if (Application.Current.MainWindow != null)
                            {
                                gc.Width = Application.Current.MainWindow.Width - 60;
                            }

                            break;
                        }
                        case ColumnSize.Auto:
                            gc.Width = double.NaN;
                            break;
                    }

                    if (col.Width.HasValue) gc.Width = col.Width.Value;
                    gridView.Columns.Add(gc);
                }
            }
            return gridView;

        }

        private DataTemplate GetSyntaxHighlightedTemplate()
        {
            var template = Application.Current.FindResource("SyntaxHighlightingTemplate") as DataTemplate;
            return template;
        }

        private DataTemplate GetHighlightableTemplate(Column col)
        {
            var template = new DataTemplate();
            var factory  = new FrameworkElementFactory(typeof(ContentControl));
            var workspaceBinding = new Binding
            {
                Path   = new PropertyPath("DataContext"),
                Source = Application.Current.MainWindow
            };
            var binding = new MultiBinding();
            binding.Bindings.Add(new Binding(col.BindingExpression));
            binding.Bindings.Add(workspaceBinding);
            binding.Converter = new StringToXamlConverter();
            factory.SetValue(ContentControl.ContentProperty, binding);
            template.VisualTree = factory;
            return template;
        }

        private DataTemplate GetBoolCellTemplate(Column col)
        {
            var template = new DataTemplate();
            var factory  = new FrameworkElementFactory(typeof(CheckBox));
            factory.SetBinding(ToggleButton.IsCheckedProperty, GetBinding(col));
            factory.SetValue(UIElement.IsEnabledProperty, false);
            template.VisualTree = factory;

            return template;
        }

        private Binding GetBinding(Column col)
        {
            var binding = new Binding(col.BindingExpression);
            binding.Converter = col.Converter;
            binding.IsAsync = col.IsAsync;
            return binding;
        }

        private DataTemplate GetRightAlignDataTemplate(Column col)
        {
            var template = new DataTemplate();
            var factory  = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
            factory.SetBinding(TextBlock.TextProperty, GetBinding(col));
            template.VisualTree = factory;

            return template;
        }

        private DataTemplate GetImageDataTemplate(Column col)
        {
            var template = new DataTemplate();
            var factory  = new FrameworkElementFactory(typeof(Image));
            factory.SetBinding(Image.SourceProperty, GetBinding(col));

            factory.SetBinding(UIElement.OpacityProperty, new Binding("ThumbnailOpacity"));
            template.VisualTree = factory;

            return template;
        }
    }
}