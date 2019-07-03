using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.Files
{
    public class DeleteAction
    {
        private readonly INotificationHost _notificationHost;

        public DeleteAction(INotificationHost notificationHost)
        {
            _notificationHost = notificationHost;
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
    }
}
