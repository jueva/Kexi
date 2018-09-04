using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class ClassicKeyHandler : IKeyHandler
    {
        public ClassicKeyHandler(Workspace workspace, List<KexBinding> bindings)
        {
            _workspace      =  workspace;
            Bindings        =  bindings;
            _bindingHandler =  new BindingHandler(workspace, bindings);
            _timer          =  new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1500)};
            _timer.Tick     += _timer_Tick;
        }

        public List<KexBinding> Bindings { get; }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                if (!string.IsNullOrEmpty(_searchString))
                {
                    _workspace.NotificationHost.AddInfo(_searchString);
                    FocusItemMatchingSearchString();
                }
            }
        }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            _timer.Stop();
            if (_bindingHandler.Handle(args, lister, group))
            {
                var type = _bindingHandler.LastCommand.GetType();
                if (type == typeof(DoActionCommand) || type == typeof(HistoryBackCommand) || args.Key == Key.Escape)
                {
                    ClearSearchString();
                }

                return false;
            }

            if (args.Key >= Key.A && args.Key <= Key.Z)
            {
                var k = args.Key.ToString().ToLower()[0];
                if (SearchString.Length == 1 && _lastKey == args.Key || SearchString.Length == 0 && _lastKey == args.Key)
                {
                    FocusNextItemWithSameStartLetter();
                }
                else
                {
                    SearchString += k;
                    _lastKey     =  args.Key;
                }

                args.Handled = true;
                return false;
            }

            return false;
        }

        private readonly BindingHandler _bindingHandler;

        private readonly DispatcherTimer _timer;
        private readonly Workspace       _workspace;
        private          Key?            _lastKey;
        private          string          _searchString;

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
            _lastKey     = null;
        }
    }
}