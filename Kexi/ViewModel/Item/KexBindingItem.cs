using System.Windows.Forms;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Item
{
    public class KexBindingItem : BaseItem
    {

        public KexBindingItem(KexBinding binding, string lister) : this(binding.CommandName, lister)
        {
            if (binding is KexDoubleBinding doubleBinding)
                Key = $"{doubleBinding.Modifier}-{doubleBinding.Key} {doubleBinding.SecondModifier}-{doubleBinding.SecondKey}";
            else
                Key = $"{binding.Modifier}-{binding.Key}";

            CommandName = DisplayName = FilterString = binding.CommandName;
            Lister = lister;
        }

        public KexBindingItem(string commandName, string lister)
        {
            Key = "None";
            CommandName = DisplayName = FilterString = commandName;
            Lister = lister;
        }

        public string Key         { get; }
        public string CommandName { get; }
        public string Lister { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is KexBindingItem o))
                return false;

            return Key == o.Key && CommandName == o.CommandName;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ CommandName.GetHashCode() ^ Lister?.GetHashCode() ?? 0;
        }
    }
}