using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

//From: https://github.com/bbougot/AcrylicWPF

namespace AcrylicWPF
{
    public class NoiseEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(NoiseEffect), 0);

        public static readonly DependencyProperty RandomInputProperty =
            RegisterPixelShaderSamplerProperty("RandomInput", typeof(NoiseEffect), 1);

        public static readonly DependencyProperty RatioProperty = DependencyProperty.Register("Ratio", typeof(double),
            typeof(NoiseEffect), new UIPropertyMetadata(0.5d, PixelShaderConstantCallback(0)));

        //pack://application:,,,/Kexi.UI;Component/Media/{file}"
        public NoiseEffect()
        {
            var pixelShader = new PixelShader {UriSource = new Uri("/Kexi.UI;component/Acrylic/Noise.ps",UriKind.Relative)};
            PixelShader = pixelShader;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("pack://application:,,,/Kexi.UI;component/Acrylic/noise.png");
            bitmap.EndInit();
            RandomInput =
                new ImageBrush(bitmap)
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, 800, 600),
                    ViewportUnits = BrushMappingMode.Absolute
                };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RandomInputProperty);
            UpdateShaderValue(RatioProperty);
        }

        public Brush Input
        {
            get => ((Brush) (GetValue(InputProperty)));
            set => SetValue(InputProperty, value);
        }

        /// <summary>The second input texture.</summary>
        public Brush RandomInput
        {
            get => ((Brush) (GetValue(RandomInputProperty)));
            set => SetValue(RandomInputProperty, value);
        }

        public double Ratio
        {
            get => ((double) (GetValue(RatioProperty)));
            set => SetValue(RatioProperty, value);
        }
    }
}