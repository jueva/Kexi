using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;

namespace Kexi.View
{
    public class KexiListViewItem : ListViewItem
    {
        public KexiListViewItem(MouseHandler mouseHandler)
        {
            _mouseHandler = mouseHandler;
        }

        private readonly MouseHandler _mouseHandler;

        private bool _onLeftButtonDownFired;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsSelected)
            {
                _onLeftButtonDownFired = false;
            }
            else
            {
                base.OnMouseLeftButtonDown(e);
                _onLeftButtonDownFired = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!_mouseHandler.Dragged && IsSelected)
                if (!_onLeftButtonDownFired)
                    base.OnMouseLeftButtonDown(e);
        }
    }
}