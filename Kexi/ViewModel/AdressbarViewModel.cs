using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.ViewModel.Commands;

namespace Kexi.ViewModel
{
    public class AdressbarViewModel : ViewModelBase
    {
        public AdressbarViewModel(Workspace workspace)
        {
            Workspace           = workspace;
            BreadcrumbViewModel = new BreadcrumbViewModel(workspace);
            Options             = workspace.Options;
            CommandRepository   = workspace.CommandRepository;

            RecentLocationsDatasource = new RecentLocationPopupViewModel(Workspace)
            {
                HorizontalOffset = -24,
                VerticalOffset   = 5,
                Width            = 250
            };

            ShowRecentLocationPopupCommand = new RelayCommand(c =>
            {
                RecentLocationsDatasource.PopupTarget  = c as Button;
                RecentLocationsDatasource.PopupVisible = true;
            });

            ClearCommand = new RelayCommand(c =>
            {
                SearchBoxText = "";
                FocusSearchBox();
            });
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

        public  RelayCommand ClearCommand { get; }
        private bool         _isSearchBoxFocused;
        private bool         _isSearchSelected;

        private string _searchBoxText;

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

        private void FocusSearchBox()
        {
            IsSearchBoxFocused = false;
            IsSearchBoxFocused = true;
        }
    }
}