using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.ViewModel.Commands;

namespace Kexi.ViewModel
{
    [Export]
    public class AdressbarViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public AdressbarViewModel(Workspace workspace, Options options, CommandRepository commandRepository, BreadcrumbViewModel breadcrumbViewModel)
        {
            Workspace = workspace;
            BreadcrumbViewModel = breadcrumbViewModel;
            Options = options;
            CommandRepository = commandRepository;

            RecentLocationsDatasource = new RecentLocationPopupViewModel(Workspace)
            {
                HorizontalOffset = -24,
                VerticalOffset = 5,
                Width = 250
            };

            ShowRecentLocationPopupCommand = new RelayCommand(c =>
            {
                RecentLocationsDatasource.PopupTarget = c as Button;
                RecentLocationsDatasource.PopupVisible = true;
            });

            ClearCommand = new RelayCommand(c =>
            {
                SearchBoxText = "";
                FocusSearchBox();
            });

            //FocusSearchBoxCommand = new RelayCommand(c => FocusSearchBox());
        }

        public BreadcrumbViewModel BreadcrumbViewModel { get; }

        public Options Options { get; }

        public CommandRepository CommandRepository { get; }

        public Workspace Workspace { get; }

        public RecentLocationPopupViewModel RecentLocationsDatasource { get; }

        public RelayCommand ShowRecentLocationPopupCommand { get; }

        public string SearchBoxText
        {
            get => _searchBoxText;
            set
            {
                if (value.Equals(_searchBoxText))
                    return;
                _searchBoxText = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearchBoxFocused
        {
            get => _isSearchBoxFocused;
            set
            {
                if (value.Equals(_isSearchBoxFocused))
                    return;
                _isSearchBoxFocused = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearchSelected
        {
            get => _isSearchSelected;
            set
            {
                if (value == _isSearchSelected) return;
                _isSearchSelected = value;
                OnPropertyChanged();
            }
        }

        public void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    Workspace.FocusCurrentOrFirst();
                    CommandRepository.GetCommandByName(nameof(DoSearchCommand)).Execute(SearchBoxText);
                    SearchBoxText = "";
                    Options.RestoreAdressbarVisibility();
                    break;
                case Key.Escape:
                    SearchBoxText = "";
                    Workspace.FocusCurrentOrFirst();
                    Options.RestoreAdressbarVisibility();
                    break;
            }
        }

        public RelayCommand ClearCommand { get; }

        //public ICommand FocusSearchBoxCommand { get; }

        private void FocusSearchBox()
        {
            IsSearchBoxFocused = false;
            IsSearchBoxFocused = true;
        }
        
        private string _searchBoxText;
        private bool _isSearchBoxFocused;
        private bool _isSearchSelected;

    }
}
