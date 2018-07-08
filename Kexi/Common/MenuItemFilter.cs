using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kexi.ViewModel.Item;

namespace Kexi.Common
{
    public class MenuItemFilter : ItemFilter<MenuCommandBoundItem>
    {

        public MenuItemFilter(IEnumerable<MenuCommandBoundItem> items, string filterString)
            : base(items, filterString)
        {
        }

        protected override bool MatchItemStartsWith(MenuCommandBoundItem item, FilterPart part)
        {
            var shortcut = false;
            if (FilterParts.Count == 1 && part.FilterString.Length > 1)
            {
                var strings = part.FilterString.ToUpper().Select(c => c.ToString());
                var pattern = string.Join(".*", strings);
                shortcut = Regex.IsMatch(item.PathFilterString, pattern, RegexOptions.None);
            }

            return shortcut || base.MatchItemStartsWith(item, part);
        }
    }
}