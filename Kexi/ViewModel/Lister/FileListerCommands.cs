using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    public class FileListerCommands
    {
        private readonly CommandRepository _commandRepository;
        private readonly INotificationHost _notificationHost;
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public FileListerCommands(Workspace workspace, CommandRepository commandRepository,
            INotificationHost notificationHost)
        {
            _workspace = workspace;
            _notificationHost = notificationHost;
            _commandRepository = commandRepository;
        }

        private IEnumerable<FileItem> GetSelectedItems(object commandArgument)
        {
            if (!(commandArgument is FileLister lister))
                throw new ArgumentNullException(nameof(commandArgument));

            return lister.SelectedItems;
        }
            
        [ExportContextMenuCommand(typeof (FileLister), "Copy Path To Clipboard")]
        public ICommand CopyPathToClipBoardCommand
        {
            get
            {
                return new RelayCommand(commandArgument =>
                {
                        var path = string.Join(Environment.NewLine,
                            GetSelectedItems(commandArgument).Select(i => i.Path));
                        Clipboard.SetText(path);
                    }
                        );
            }
        }

        [ExportContextMenuCommand(typeof (FileLister), "Open containing folder")]
        public ICommand OpenContainingFolder
        {
            get
            {
                return new RelayCommand(commandArgument =>
                {
                    var item = GetSelectedItems(commandArgument).FirstOrDefault();
                    if (item == null)
                        return;

                    var target = new FileItemTargetResolver(item);
                    var lister = KexContainer.Resolve<FileLister>();
                    lister.Path = new FileInfo(target.TargetPath).DirectoryName;
                    _workspace.Open(lister);
                    lister.Refresh();
                }
                        );
            }
        }

        [ExportContextMenuCommand(typeof (FileLister), "Copy Name To Clipboard")]
        public ICommand CopyNameToClipboardCommand
        {
            get
            {
                return new RelayCommand(commandArgument =>
                {
                    var name = string.Join(Environment.NewLine,
                        GetSelectedItems(commandArgument).Select(i => i.DisplayName));
                    Clipboard.SetText(name);
                    }
                        );
            }
        }
        [ExportContextMenuCommand(typeof (FileLister), "Clear Readonly Flag")]
        public ICommand ClearReadonlyFlag
        {
            get
            {
                return new RelayCommand(commandArgument =>
                {
                    foreach (IItem pi in GetSelectedItems(commandArgument))
                    {
                        FileAttributes attr = File.GetAttributes(pi.Path);
                        string path = pi.Path;
                        if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            File.SetAttributes(path, (attr ^ FileAttributes.ReadOnly));
                    }
                });
            }
        }

        [ExportContextMenuCommand(typeof (FileLister), "Send As Mail")]
        public ICommand SendAsMail
        {
            get
            {
                return new RelayCommand(commandArgument =>
                {
                    try
                    {
                        var message = new MapiMailMessage("", "");
                        foreach (IItem file in GetSelectedItems(commandArgument))
                        {
                            message.Files.Add(file.Path);
                        }
                        message.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        _notificationHost.AddError(ex);
                    }
                });
            }
        }

        [ExportContextMenuCommand(typeof (FileLister), "Add To Favorites")]
        public ICommand AddToFavorites => _commandRepository.GetCommandByName(nameof(CreateShortcutInFavoritesCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Open Command Prompt")]
        public ICommand OpenComamndPrompt => _commandRepository.GetCommandByName(nameof(OpenPathInConsoleCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Open In Explorer")]
        public ICommand OpenInExplorer => _commandRepository.GetCommandByName(nameof(OpenPathInExplorerCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Show Latest Items")]
        public ICommand ShowLatestItem => _commandRepository.GetCommandByName(nameof(ShowLatestItemsCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Synchronize Filenames")]
        public ICommand SynchronizeFilenames => _commandRepository.GetCommandByName(nameof(ShowSynchronizeFilenamesPopupCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Show Security")]
        public ICommand ShowSecurity => _commandRepository.GetCommandByName(nameof(ShowSecurityListerCommand));

        [ExportContextMenuCommand(typeof (FileLister), "Show References")]
        public ICommand ShowReferences => _commandRepository.GetCommandByName(nameof(ShowReferenceListerCommand));

        [ExportContextMenuCommand(typeof(FileLister), "Properties")]
        public ICommand ShowAllFilePropertiers => _commandRepository.GetCommandByName(nameof(ShowPropertyListerCommand));
    }
}