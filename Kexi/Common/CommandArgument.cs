using Kexi.ViewModel.Lister;

namespace Kexi.Common
{
    public class CommandArgument
    {
        public CommandArgument(ILister lister, params object[] parameters)
        {
            Lister     = lister;
            Parameters = parameters;
        }

        public ILister Lister { get; }

        public object[] Parameters { get; }
    }
}