using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel.Commands;

namespace Kexi
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CommandRepository
    {
        internal readonly Lazy<Dictionary<string, IKexiCommand>> CommandCache;

        public CommandRepository()
        {
            CommandCache = new Lazy<Dictionary<string, IKexiCommand>>(() => { return KexiCommands.ToDictionary(c => c.GetType().Name); });
        }

        [ImportMany]
        private IEnumerable<IKexiCommand> KexiCommands { get; set; }

        public ICommand ToggleMenuPopup => GetCommandByName(nameof(ToggleMenuPopupCommand));
        public ICommand HistoryBack         => GetCommandByName(nameof(HistoryBackCommand));
        public ICommand HistoryForward      => GetCommandByName(nameof(HistoryForwardCommand));
        public ICommand GotoParentContainer => GetCommandByName(nameof(GotoParentContainerCommand));
        public ICommand ViewThumbnails      => GetCommandByName(nameof(ViewThumbnailsCommand));
        public ICommand ViewDetails         => GetCommandByName(nameof(ViewDetailsCommand));
        public ICommand OpenNewWindow => GetCommandByName(nameof(OpenNewWindowCommand));
        public ICommand OpenPathInConsole => GetCommandByName(nameof(OpenPathInConsoleCommand));
        public ICommand OpenPathInPowerShell => GetCommandByName(nameof(OpenPathInPowerShellCommand));
        public ICommand Quit => GetCommandByName(nameof(QuitCommand));
        public ICommand Copy => GetCommandByName(nameof(CopyCommand));
        public ICommand Cut => GetCommandByName(nameof(CutCommand));
        public ICommand Paste => GetCommandByName(nameof(PasteCommand));
        public ICommand Delete => GetCommandByName(nameof(DeleteFilesCommand));
        public ICommand Rename => GetCommandByName(nameof(CutCommand)); //TODO: renamecommand
        public ICommand Favorites => GetCommandByName(nameof(ShowFavoritesCommand));
        public ICommand CreateDirectory => GetCommandByName(nameof(ShowCreateElementPopupCommand));
        public ICommand CreateFile => GetCommandByName(nameof(ShowCreateElementPopupCommand));
        public ICommand ShowProperties => GetCommandByName(nameof(ShowFilePropertiesCommand));
        public ICommand Open => GetCommandByName(nameof(DoActionCommand));
        public ICommand Edit => GetCommandByName(nameof(EditFileCommand));
        public ICommand SelectAll => GetCommandByName(nameof(SelectAllCommand));
        public ICommand SelectNone => GetCommandByName(nameof(InvertSelectionCommand)); //TODO: selectNoneCommand
        public ICommand InvertSelection => GetCommandByName(nameof(InvertSelectionCommand));

        public IKexiCommand GetCommandByName(string name)
        {
            return CommandCache.Value[name];
        }
    }
}