using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Kexi.Common
{
    public class ProcessHelper
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All                     = 0x001F0FFF,
            Terminate               = 0x00000001,
            CreateThread            = 0x00000002,
            VirtualMemoryOperation  = 0x00000008,
            VirtualMemoryRead       = 0x00000010,
            VirtualMemoryWrite      = 0x00000020,
            DuplicateHandle         = 0x00000040,
            CreateProcess           = 0x000000080,
            SetQuota                = 0x00000100,
            SetInformation          = 0x00000200,
            QueryInformation        = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize             = 0x00100000
        }

        public static string GetExecutablePath(Process Process)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return GetExecutablePathAboveVista(Process.Id);
            }
 
            return Process.MainModule.FileName;
        }

        public static string GetExecutablePathAboveVista(int ProcessId)
        {
            var buffer = new StringBuilder(1024);
            var hprocess = OpenProcess(ProcessAccessFlags.QueryLimitedInformation,
                false, ProcessId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    var size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
            StringBuilder lpExeName, out int size);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);
    }
}