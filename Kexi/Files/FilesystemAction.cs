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
            return PasteAction.Paste(target, items, action, flags);
        }

        public string Delete(IEnumerable<FileItem> selectedItems)
        {
            return new DeleteAction(_notificationHost).Delete(selectedItems);
        }

        public void Undo(string path, StringCollection items, FileAction action)
        {
            switch (action)
            {
                case FileAction.Copy:
                    new UndoCopyAction(path, items).DoUndo();
                    break;
                case FileAction.Move:
                    new UndoMoveAction(path, items).DoUndo();
                    break;
                case FileAction.Delete:
                    new UndoDeleteAction(path, items).DoUndo();
                    break;
            }
        }
    }
}