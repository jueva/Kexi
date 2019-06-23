using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.Files
{
    public class FilesystemAction
    {
        private readonly INotificationHost _notificationHost;

        public FilesystemAction(INotificationHost notificationHost)
        {
            _notificationHost = notificationHost;
        }

        public static FileAction LastFileAction { get; private set; }

        public static void SetClipboard<T>(FileAction fa, IEnumerable<string> items)
        {
            if (!items.Any())
                return;

            var moveEffect = fa == FileAction.Move
                ? new byte[] {2, 0, 0, 0}
                : new byte[] {5, 0, 0, 0};
            var dropEffect = new MemoryStream();
            dropEffect.Write(moveEffect, 0, moveEffect.Length);

            var selection = items;
            var files     = new StringCollection();
            files.AddRange(selection.ToArray());
            var data = new DataObject();
            if (typeof(T) == typeof(FileItem))
                data.SetFileDropList(files);
            else
                data.SetText(string.Join(Environment.NewLine, files));
            data.SetData("Preferred DropEffect", dropEffect);

            Clipboard.Clear();
            Clipboard.SetDataObject(data);
            LastFileAction = fa;
        }

        public string Copy(string target, StringCollection items)
        {
            return Paste(target, items, FileAction.Copy);
        }

        public string Move(string target, StringCollection items)
        {
            return Paste(target, items, FileAction.Move);
        }

        public string Paste(string target, StringCollection items, FileAction action, short flags = ShellNative.FOF_ALLOWUNDO)
        {
            if (!Clipboard.ContainsFileDropList())
                return null;

            if (action != FileAction.Copy && action != FileAction.Move && action != FileAction.Create)
                throw new ArgumentOutOfRangeException(nameof(action), action, "Action should be Copy or Move");

            var   it                    = items.Cast<string>().ToList();
            var   from                  = string.Join("\0", it) + "\0";
            var   to                    = target + "\0";

            if (items.Count == 1)
            {
                var info = new FileInfo(items[0]);
                if (info.Directory.FullName.ToLower() == target.ToLower())
                {
                    flags = (short) (flags | ShellNative.FOF_RENAMEONCOLLISION);
                }
            }

            var fileop = new ShellNative.SHFILEOPSTRUCT
            {
                wFunc  = action == FileAction.Copy ? ShellNative.FO_COPY : ShellNative.FO_MOVE,
                pFrom  = from,
                pTo    = to,
                fFlags = flags
            };
            ShellNative.SHFileOperation(ref fileop);

            return $@"{target}\{it.LastOrDefault()}";
        }

        public string Delete(IEnumerable<FileItem> selectedItems)
            {
            if (selectedItems.FirstOrDefault()?.DisplayName == "..")
            {
                return "Can't delete this item";
            }
            foreach (var i in selectedItems)
                if (i.ItemType == ItemType.Container)
                    try
                    {
                        Directory.Delete(i.Path, true);
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(i.Path)) //.zip files haben ItemType.Container sind aber files
                        {
                            try
                            {
                                File.Delete(i.Path);
                            }
                            catch (Exception innerEx)
                            {
                                _notificationHost.AddError(innerEx);
                                return "Fehler beim Löschen von " + i.Path;
                            }
                        }
                        else
                        {
                            _notificationHost.AddError(ex);
                            return "Fehler beim Löschen von " + i.Path;
                        }
                    }
                else
                    try
                    {
                        File.Delete(i.Path);
                    }
                    catch (Exception ex)
                    {
                        _notificationHost.AddError(ex);
                        return "Fehler beim Löschen von " + i.Path;
                    }

            return null;
        }

        public void Undo(string path, StringCollection items, FileAction action)
        {
            switch (action)
            {
                case FileAction.Copy:
                    new UndoCopyCommand(path, items).DoUndo();
                    break;
                case FileAction.Move:
                    break;
                case FileAction.Delete:
                    break;
            }
            _notificationHost.AddInfo($"{path} - {items.Count} - {action}");
        }
    }
}