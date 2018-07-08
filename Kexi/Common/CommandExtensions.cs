using System.Windows.Input;

namespace Kexi.Common
{
    public static class CommandExtensions
    {
        public static void Execute(this ICommand command)
        {
            if (command.CanExecute(null))
                command.Execute(null);
        }
    }
}