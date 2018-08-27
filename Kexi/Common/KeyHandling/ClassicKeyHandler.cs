using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;

namespace Kexi.Common.KeyHandling
{
    public class ClassicKeyHandler : IKeyHandler
    {
        public ClassicKeyHandler(Workspace workspace)
        {
            _workspace  =  workspace;
            _timer      =  new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            _timer.Tick += _timer_Tick;
        }

        public List<KexBinding> Bindings { get; set; }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                if (!string.IsNullOrEmpty(_searchString)) FocusItemMatchingSearchString();
            }
        }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            _timer.Stop();
            var modifierKeys = args.KeyboardDevice.Modifiers;
            switch (args.Key)
            {
                case Key.Escape:
                    ClearSearchString();
                    break;
                case Key.F1:
                    new ShowCommandsPopupCommand(_workspace, new CommandsPopupViewModel(_workspace, _workspace.Options, new MouseHandler(_workspace))).Execute();
                    break;
                case Key.F4:
                    if ((modifierKeys & ModifierKeys.Control) != 0)
                        new WindowCloseCommand(_workspace).Execute();
                    break;
                case Key.Back:
                    new HistoryBackCommand(_workspace).Execute();
                    break;
                case Key.Tab:
                    if ((modifierKeys & ModifierKeys.Shift) != 0)
                        new CycleTabsBackwardsCommand(_workspace).Execute();
                    else
                        new CycleTabsCommand(_workspace).Execute();
                    break;
                case Key.Return:
                    new DoActionCommand(_workspace).Execute();
                    ClearSearchString();
                    break;
                default:
                    var k = args.Key.ToString().ToLower()[0];
                    if (k >= 'a' && k <= 'z')
                    {
                        if (SearchString.Length == 1 && lastKey == k || SearchString.Length == 0 && lastKey == k)
                        {
                            FocusNextItemWithSameStartLetter();
                        }
                        else
                        {
                            SearchString += k;
                            lastKey      =  k;
                        }
                    }

                    break;
            }

            args.Handled = true;
            return false;
        }

        private readonly DispatcherTimer _timer;
        private readonly Workspace       _workspace;
        private          string          _searchString;
        private          char            lastKey;

        private void FocusItemMatchingSearchString()
        {
            var filter    = new ItemFilter<IItem>(_workspace.CurrentItems, SearchString);
            var selection = filter.Matches.FirstOrDefault();
            if (selection != null) _workspace.FocusItem(selection);

            _timer.Start();
        }

        private void FocusNextItemWithSameStartLetter()
        {
            var baseItems = _workspace.CurrentItems.SkipWhile(i => i.Path != _workspace.CurrentItem.Path).Skip(1);
            var filter    = new ItemFilter<IItem>(baseItems, SearchString);
            var selection = filter.MatchesBeginning.FirstOrDefault();
            if (selection != null) _workspace.FocusItem(selection);

            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            ClearSearchString();
        }

        public void ClearSearchString()
        {
            SearchString = "";
        }
    }
}