using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Shell;

namespace Kexi.ViewModel.Item
{
    public class ProcessItem : BaseItem
    {
        public ProcessItem(Process process) : base(process.ProcessName)
        {
            Process = process;
        }

        public ProcessDetailItem Details
        {
            get
            {
                if (_details == null)
                    SetDetailsAsync();
                return _details;
            }
            set
            {
                if (Equals(value, _details)) return;
                _details = value;
                OnPropertyChanged();
            }
        }

        public Process Process { get; }

        public BitmapSource LargeThumbnail => Thumbnail;

        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName == value)
                    return;

                _fileName = value;
                OnPropertyChanged();
            }
        }

        public string ModuleName
        {
            get => _moduleName;
            set
            {
                if (value == _moduleName) return;
                _moduleName = value;
                OnPropertyChanged();
            }
        }


        public string UserName
        {
            get => _userName;
            set
            {
                if (value == _userName) return;
                _userName = value;
                OnPropertyChanged();
            }
        }


        private string _fileName;
        private string _moduleName;
        private string _userName;

        private ProcessDetailItem _details;

        private async void SetDetailsAsync()
        {
            Details      = await Task.Run(() => GetDetail());
            Thumbnail    = Details.Thumbnail;
            FilterString = $"{DisplayName}_{Details.Description}";
        }

        private ProcessDetailItem GetDetail()
        {
            try
            {
                var main = Process.MainModule;
                FileName   = main.FileName;
                ModuleName = main.ModuleName;
                var bm = new NativeFileInfo(FileName).Icon;
                bm.Freeze();
                return new ProcessDetailItem
                {
                    Thumbnail   = bm,
                    Description = main.FileVersionInfo.FileDescription,
                    Memory      = Process.PagedMemorySize64,
                    Cpu         = 0,
                    Pid         = Process.Id
                };
            }
            catch
            {
                return new ProcessDetailItem();
            }
        }


        //private string GetProcessOwner(int processId)
        //{
        //    var query       = "Select * From Win32_Process Where ProcessID = " + processId;
        //    var searcher    = new ManagementObjectSearcher(query);
        //    var processList = searcher.Get();

        //    foreach (ManagementObject obj in processList)
        //    {
        //        var argList   = {string.Empty, string.Empty};
        //        var returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
        //        if (returnVal == 0) return argList[0];
        //    }

        //    return "NO OWNER";
        //}
    }
}