using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Popup
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MenuPopupViewModel : PopupViewModel<MenuCommandBoundItem>
    {
        [ImportingConstructor]
        public MenuPopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
            Title = "Menu";
        }

        private MenuCommandBoundItem[]            _allItems;
        private IEnumerable<MenuCommandBoundItem> _level0Items;
        private bool                              _selected;
        public override void Open()
        {
            var menuImg = Utils.GetImageFromRessource("menu.png");
            _allItems = Traverse(MenuCommandBoundItems(menuImg), i => i.Items).Reverse().ToArray();
            _level0Items = _allItems.Where(i => i.Level == 0);
            SetHeaderIconByKey("appbar_lines_horizontal_4");
            BaseItems = _level0Items.OrderByDescending(i => i.Items.Any());
            base.Open();
        }

        public override void Close()
        {
            _selected = false;
            base.Close();
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            if (Items == null || ItemsView == null)
                return;

            var filterbase = _selected ? BaseItems : Text != null && Text.Any() ? _allItems : _level0Items;

            var filtered = new MenuItemFilter(filterbase, Text);
            if (filtered.IsEmptyAndSinglePart)
            {
                ea.Handled = true;
                if (Text != null && Text.Any())
                {
                    Text = Text.Substring(0, Text.Length - 1);
                    SetCaret(Text.Length);
                }

                return;
            }

            Items = filtered;
            FocusInput();
        }


        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            if (ea.Key == Key.Space)
            {
                ItemSelected(ItemsView.CurrentItem as MenuCommandBoundItem);
                ea.Handled = true;
            }

            base.PreviewKeyDown(sender, ea);
        }

        protected override void ItemSelected(MenuCommandBoundItem commandItem)
        {
            if (commandItem == null)
                return;

            _selected = true;
            if (commandItem.Command == null && commandItem.Items.Any())
            {
                BaseItems = commandItem.Items;
                Text      = "";
            }
            else if (commandItem.Command != null && commandItem.Command.CanExecute(null))
            {
                IsOpen = false;
                commandItem.Command.Execute(null);
            }
        }

        private static IEnumerable<MenuCommandBoundItem> Traverse(IEnumerable<MenuCommandBoundItem> items, Func<MenuCommandBoundItem, IEnumerable<MenuCommandBoundItem>> childSelector)
        {
            var stack = new Stack<MenuCommandBoundItem>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                {
                    child.Level            = next.Level + 1;
                    child.PathFilterString = next.PathFilterString + child.PathFilterString;
                    stack.Push(child);
                }
            }
        }

        private MenuCommandBoundItem[] MenuCommandBoundItems(BitmapSource menuImg)
        {
            var items = new[]
            {
                new MenuCommandBoundItem("View", null)
                {
                    Items = new[]
                    {
                        new MenuCommandBoundItem("View Details",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ViewDetailsCommand)))),
                        new MenuCommandBoundItem("View Icons",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ViewIconsCommand)))),
                        new MenuCommandBoundItem("View Thumbnails",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ViewThumbnailsCommand))))
                    },
                    Thumbnail = menuImg
                },
                new MenuCommandBoundItem("Layout", null)
                {
                    Items = new[]
                    {
                        new MenuCommandBoundItem("Toggle Adressbar",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ToggleAdressbarVisibiblityCommand)))),
                        new MenuCommandBoundItem("Toggle Statusbar",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ToggleStatusbarVisibilityCommand)))),
                        new MenuCommandBoundItem("Toggle Detailspane",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ToggleFileDetailsVisibilityCommand)))),
                        new MenuCommandBoundItem("Toggle Treeview Pane",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ToggleTreeViewVisibilityCommand)))),
                        new MenuCommandBoundItem("Toggle Ribbonpane",
                            new RelayCommand(c => CommandRepository.Execute(nameof(ToggleRibbonVisibilityCommand)))),
                        new MenuCommandBoundItem("Load Layout",
                            new RelayCommand(c => CommandRepository.Execute(nameof(LoadLayoutCommand)))),
                        new MenuCommandBoundItem("Save Layout",
                            new RelayCommand(c => CommandRepository.Execute(nameof(SaveLayoutCommand))))
                    },
                    Thumbnail = menuImg
                },
                new MenuCommandBoundItem("Window", null)
                {
                    Items = new[]
                    {
                        new MenuCommandBoundItem("Adjust Width",
                            new RelayCommand(c => CommandRepository.Execute(nameof(AdjustListerWidthCommand)))),
                        new MenuCommandBoundItem("Adjust Column Width",
                            new RelayCommand(c => CommandRepository.Execute(nameof(AdjustColumnWidth)))),
                        new MenuCommandBoundItem("Split Horizontal",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowSplitHorizontalCommand)))),
                        new MenuCommandBoundItem("Split Vertical",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowSplitVerticalCommand)))),
                        new MenuCommandBoundItem("Move To Next Group",
                            new RelayCommand(c => CommandRepository.Execute(nameof(MoveToNextTabCommand)))),
                        new MenuCommandBoundItem("Move To Previous Group",
                            new RelayCommand(c => CommandRepository.Execute(nameof(MoveToPreviousTabCommand)))),
                        new MenuCommandBoundItem("Maximize",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowMaximizeCommand)))),
                        new MenuCommandBoundItem("Minimize",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowMinimizeCommand)))),
                        new MenuCommandBoundItem("Restore",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowRestoreCommand)))),
                        new MenuCommandBoundItem("Dock Left",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowDockLeftCommand)))),
                        new MenuCommandBoundItem("Dock Right",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowDockRightCommand)))),
                        new MenuCommandBoundItem("Center",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowCenterCommand)))),
                        new MenuCommandBoundItem("Full Height",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowFullHeightCommand)))),
                        new MenuCommandBoundItem("Next Monitor",
                            new RelayCommand(c => CommandRepository.Execute(nameof(WindowNextMonitorCommand))))
                    },
                    Thumbnail = menuImg
                },
                new MenuCommandBoundItem("Listers", null)
                {   
                    //if injected in constructor: same listers/datasource for different process/services lister winodws => collection modified 
                    Items = KexContainer.ResolveMany<ILister>()
                        .Where(l => !string.IsNullOrEmpty(l.Title))
                        .Where(l => l.ShowInMenu)
                        .Select(l => new MenuCommandBoundItem(l.Title, new RelayCommand(c =>
                        {
                            Workspace.Open(l);
                            l.Refresh();
                        }))),
                    Thumbnail = menuImg
                },
                new MenuCommandBoundItem("Options", new RelayCommand(c => CommandRepository.Execute(nameof(ShowSettingsListerCommand)))),
                new MenuCommandBoundItem("GroupBy", new RelayCommand(c => CommandRepository.Execute(nameof(ShowGroupByPopupCommand)))),
                new MenuCommandBoundItem("Sort", new RelayCommand(c => CommandRepository.Execute(nameof(ShowSortPopupCommand)))),
                new MenuCommandBoundItem("Toggle Hidden Items", new RelayCommand(c =>
                {
                    new ToggleHiddenItemsCommand(Workspace);
                    Workspace.ActiveLister.Refresh();
                })),
                new MenuCommandBoundItem("Refresh", new RelayCommand(c => CommandRepository.Execute(nameof(RefreshListerCommand)))),
                new MenuCommandBoundItem("Console Command", new RelayCommand(c => CommandRepository.Execute(nameof(ShowConsolePopupCommand)))),
                new MenuCommandBoundItem("Restart as Admin", new RelayCommand(c => CommandRepository.Execute(nameof(RestartAsAdminCommand)))),
                new MenuCommandBoundItem("Quit", new RelayCommand(c => CommandRepository.Execute(nameof(QuitCommand)))),
            };
            return items;
        }
    }
}