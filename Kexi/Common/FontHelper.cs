using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Kexi.Common
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FontHelper
    {
        private readonly Options _options;

        [ImportingConstructor]
        public FontHelper(Options options)
        {
            _options = options;
        }

        public void SetFont(Control c)
        {
            c.FontFamily = _options.FontFamily;
            c.FontSize   = _options.FontSize;
        }
    }
}