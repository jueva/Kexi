using System.Windows.Input;

namespace Kexi.Common
{
    public static class KeyExtensions
    {
        public static bool IsModifier(this Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return true;
                default:
                    return false;
            }
        }
    }
}