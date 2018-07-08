using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Item
{
    public class MenuCommandBoundItem : CommandBoundItem
    {
        private int _level;

        public MenuCommandBoundItem(string name, ICommand command) : base(name, command)
        {
            PathFilterString = FilterString;
            Items = new List<MenuCommandBoundItem>();
        }

        public IEnumerable<MenuCommandBoundItem> Items { get; set; }
        public string PathFilterString { get; set; }

        public int Level
        {
            get { return _level; }
            set
            {
                //if (value > 0)
                //{
                //    DisplayName = new String(' ', value*2) + DisplayName;
                //}
                _level = value;
            }
        }
    }
}
