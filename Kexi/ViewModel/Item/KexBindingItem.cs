using Kexi.Common.KeyHandling;

namespace Kexi.ViewModel.Item
{
    public class KexBindingItem : BaseItem
    {
        public KexBindingItem(KexBinding binding) : this(binding.CommandName, binding.Group)
        {
            Binding     = binding;
        }

        public KexBindingItem(string commandName, string lister)
        {
            CommandName = DisplayName = FilterString = commandName;
            Lister      = lister;
        }

        public KexBinding Binding     { get; }
        public string Key => Binding?.ToString() ?? "None";
        public string     CommandName { get; }
        public string     Lister      { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is KexBindingItem item) || Binding == null)
                return false;

            return Binding.Equals(item.Binding);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ CommandName.GetHashCode() ^ Lister?.GetHashCode() ?? 0;
        }
    }
}