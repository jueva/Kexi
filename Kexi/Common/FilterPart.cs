using System.Linq;

namespace Kexi.Common
{
    public class FilterPart
    {
        public FilterPart(string part)
        {
            if (part.Length > 0 && part[0] == '-')
            {
                Negate = part.Length <= 1 || part[1] != '-';
                FilterString = part.Substring(1).Trim();
            }
            else
            {
                Negate = false;
                FilterString = part;
            }
        }

        public string FilterString { get; }

        public bool Negate { get; }

        public bool HasFilter => !string.IsNullOrEmpty(FilterString);
    }
}
