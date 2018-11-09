using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;
using ThreadState = System.Diagnostics.ThreadState;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(ConsoleLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsoleLister : BaseLister<ConsoleItem>
    {
        [ImportingConstructor]
        public ConsoleLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository)
            : base(workspace, notificationHost, options, commandRepository)
        {
            Title = PathName = Path = "Console";
            Items = new ObservableCollection<ConsoleItem>();
            BindingOperations.EnableCollectionSynchronization(Items, _locker);
            CommandHistory = new Stack<ConsoleCommandItem>();
        }

        [ExportContextMenuCommand(typeof(ConsoleLister), "Break Current Command")]
        public RelayCommand BreakConsole
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var args = c as CommandArgument;
                    if (!(args?.Lister is ConsoleLister lister))
                        return;

                    if (_currentProcess != null)
                    {
                        _currentProcess.CancelOutputRead();
                        _currentProcess.CancelErrorRead();
                        _currentProcess.Close();
                        lister.Items.Add(new ConsoleItem("Break by User") {Highlighted = true});
                    }
                });
            }
        }

        [ExportContextMenuCommand(typeof(ConsoleLister), "Focus Next Command")]
        public RelayCommand FocusNextConsoleCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var args = c as CommandArgument;
                    if (!(args?.Lister is ConsoleLister lister))
                        return;

                    var maxIndex     = lister.Items.Count() - 1;
                    var currentIndex = lister.ItemsView.CurrentPosition + 1;
                    while (currentIndex <= maxIndex)
                    {
                        if (lister.Items[currentIndex].Highlighted)
                            break;
                        currentIndex++;
                    }

                    var focusIndex = currentIndex < maxIndex ? currentIndex : currentIndex - 1;
                    var item       = lister.Items[focusIndex];
                    lister.View.FocusItem(item);
                    lister.View.ListView.ScrollIntoView(item);
                });
            }
        }

        [ExportContextMenuCommand(typeof(ConsoleLister), "Focus Previous Command")]
        public RelayCommand FocusPreviousConsoleCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    var args = c as CommandArgument;
                    if (!(args?.Lister is ConsoleLister lister))
                        return;

                    var currentIndex = lister.ItemsView.CurrentPosition - 1;
                    while (currentIndex > 0)
                    {
                        if (lister.Items[currentIndex].Highlighted)
                            break;
                        currentIndex--;
                    }

                    if (currentIndex >= 0)
                    {
                        var item = lister.Items[currentIndex];
                        lister.View.FocusItem(item);
                        lister.View.ListView.ScrollIntoView(item);
                    }
                });
            }
        }

        [ExportContextMenuCommand(typeof(ConsoleLister), "Clear Console")]
        public RelayCommand CearConsole
        {
            get
            {
                return new RelayCommand(c =>
                    {
                        var args = c as CommandArgument;
                        if (!(args?.Lister is ConsoleLister lister))
                            return;
                        lister.Items.Clear();
                        lister.Items.Add(new ConsoleItem("")); //no key-events on empty list
                    }
                );
            }
        }

        public string                Command   { get; set; }
        public IEnumerable<FileItem> Selection { get; set; }

        public Stack<ConsoleCommandItem> CommandHistory { get; }

        public override IEnumerable<Column> Columns { get; } =
            new ObservableCollection<Column>
            {
                new Column("", "DisplayName", ColumnType.Text, ColumnSize.FullWidth)
            };

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set
            {
                if (value == _workingDirectory)
                    return;
                _workingDirectory = value;
                OnNotifyPropertyChanged();
                SetTitleAndPath();
            }
        }

        private readonly object                  _locker = new object();
        private          Process                 _currentProcess;
        private          string                  _workingDirectory;

        public override Task Refresh(bool clearFilterAndGroup = true)
        {
            if (View == null || string.IsNullOrEmpty(Command))
                return Task.CompletedTask;

            var commandString = $"{DateTime.Now.ToShortTimeString()} - {WorkingDirectory ?? ""}> {Command}";
            Items.Add(new ConsoleItem(commandString, Command) {Highlighted = true});
            View.FocusCurrentOrFirst();
            var item = new ConsoleCommandItem(Command, WorkingDirectory);
            CommandHistory.Push(item);

            var start = new ProcessStartInfo();
            SetupEnviroment(start);
            var p = new Process {StartInfo = start};

            p.OutputDataReceived += Process_OutputDataReceived;
            p.ErrorDataReceived  += P_ErrorDataReceived;

            p.Start();

            p.EnableRaisingEvents =  true;
            p.Exited              += P_Exited;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            _currentProcess = p;
            return Task.CompletedTask;
        }

        protected override Task<IEnumerable<ConsoleItem>> GetItems()
        {
            return null;
        }

        private void P_Exited(object sender, EventArgs e)
        {
            LoadingStatus = LoadingStatus.Loaded;
            _currentProcess = null;
        }

        private void SetupEnviroment(ProcessStartInfo start)
        {
            var tempPath = start.EnvironmentVariables["PATH"];
            if (!tempPath.EndsWith(";"))
                tempPath += ";";
            start.EnvironmentVariables["PATH"] = tempPath;
            start.FileName                     = @"cmd.exe";
            start.Arguments                    = "/c " + Command;
            if (Selection != null && Selection.Any())
                start.Arguments = start.Arguments.Replace("%%", string.Join(" ", Selection.Select(i => i.Path)));

            start.StandardOutputEncoding = Encoding.GetEncoding(Options.ConsoleEncoding);
            start.StandardErrorEncoding  = Encoding.GetEncoding(Options.ConsoleEncoding);
            start.WorkingDirectory       = WorkingDirectory;
            start.UseShellExecute        = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError  = true;
            start.RedirectStandardInput  = true;
            start.CreateNoWindow         = true;
            start.ErrorDialog            = false;
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) Application.Current.Dispatcher.Invoke(() =>
            {
                Items.Add(new ConsoleItem(e.Data) {HasError = true, Enabled = false});
            });
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var item = new ConsoleItem(e.Data);
                    Items.Add(item);

                    Workspace.ActiveLister.View.ListView.SelectionMode = SelectionMode.Single;
                    Workspace.FocusItem(item);
                    Workspace.ActiveLister.View.ListView.SelectionMode = SelectionMode.Extended;
                });
                
            }
        }

        public override void DoAction(ConsoleItem item)
        {
            CommandRepository.GetCommandByName(nameof(ShowConsolePopupCommand)).Execute(item.Command);
        }

        private void SetTitleAndPath()
        {
            if (string.IsNullOrEmpty(WorkingDirectory))
                Title = Path = PathName = "Console";
            else
                Title = Path = Path = "Console://" + WorkingDirectory;
        }
    }
}