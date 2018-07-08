//http://stackoverflow.com/questions/21848271/redirecting-standard-input-of-console-application

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kexi.Common
{
    /// <summary>
    ///     The console app manager.
    /// </summary>
    public class ConsoleAppManager
    {
        #region Constructors and Destructors

        public ConsoleAppManager(string appName)
        {
            this.appName = appName;

            process.StartInfo.FileName              = this.appName;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            process.StartInfo.RedirectStandardInput  = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents              = true;
            process.StartInfo.CreateNoWindow         = true;

            process.StartInfo.UseShellExecute = false;

            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            process.Exited += ProcessOnExited;
        }

        #endregion

        #region Fields

        private readonly string appName;

        private readonly Process process = new Process();

        /// <summary>
        ///     The the lock.
        /// </summary>
        private readonly object theLock = new object();

        private SynchronizationContext context;

        private string pendingWriteData;

        #endregion

        #region Public Events

        public event EventHandler<string> ErrorTextReceived;

        public event EventHandler ProcessExited;


        public event EventHandler<string> StandartTextReceived;

        #endregion

        #region Public Properties

        public int ExitCode => process.ExitCode;


        public bool Running { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void ExecuteAsync(params string[] args)
        {
            if (Running)
                throw new InvalidOperationException(
                    "Process is still Running. Please wait for the process to complete.");

            var arguments = string.Join(" ", args);

            process.StartInfo.Arguments = arguments;

            context = SynchronizationContext.Current;

            process.Start();
            Running = true;

            new Task(ReadOutputAsync).Start();
            new Task(WriteInputTask).Start();
            new Task(ReadOutputErrorAsync).Start();
        }


        public void Write(string data)
        {
            if (data == null) return;

            lock (theLock)
            {
                pendingWriteData = data;
            }
        }


        public void WriteLine(string data)
        {
            Write(data + Environment.NewLine);
        }

        #endregion

        #region Methods

        protected virtual void OnErrorTextReceived(string e)
        {
            var handler = ErrorTextReceived;

            if (handler != null)
            {
                if (context != null)
                    context.Post(delegate { handler(this, e); }, null);
                else
                    handler(this, e);
            }
        }


        protected virtual void OnProcessExited()
        {
            var handler = ProcessExited;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnStandartTextReceived(string e)
        {
            var handler = StandartTextReceived;

            if (handler != null)
            {
                if (context != null)
                    context.Post(delegate { handler(this, e); }, null);
                else
                    handler(this, e);
            }
        }


        private void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            OnProcessExited();
        }


        private async void ReadOutputAsync()
        {
            var standart = new StringBuilder();
            var buff     = new char[1024];
            int length;

            while (process.HasExited == false)
            {
                standart.Clear();

                length = await process.StandardOutput.ReadAsync(buff, 0, buff.Length);
                standart.Append(buff.SubArray(0, length));
                OnStandartTextReceived(standart.ToString());
                Thread.Sleep(1);
            }

            Running = false;
        }


        private async void ReadOutputErrorAsync()
        {
            var sb = new StringBuilder();

            do
            {
                sb.Clear();
                var buff   = new char[1024];
                var length = await process.StandardError.ReadAsync(buff, 0, buff.Length);
                sb.Append(buff.SubArray(0, length));
                OnErrorTextReceived(sb.ToString());
                Thread.Sleep(1);
            } while (process.HasExited == false);
        }


        private async void WriteInputTask()
        {
            while (process.HasExited == false)
            {
                Thread.Sleep(1);

                if (pendingWriteData != null)
                {
                    await process.StandardInput.WriteLineAsync(pendingWriteData);
                    await process.StandardInput.FlushAsync();

                    lock (theLock)
                    {
                        pendingWriteData = null;
                    }
                }
            }
        }

        #endregion
    }

    public static class ArrayExtension
    {
        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            return array.Skip(startIndex).Take(length).ToArray();
        }
    }
}