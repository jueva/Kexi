using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;

namespace Kexi.ViewModel
{
    public abstract class PopupViewModel : ViewModelBase
    {
        protected PopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler)
        {
            Workspace = workspace;
            Options = options;
            MouseHandler = mouseHandler;
            TitleVisible = Options.PopupTitleVisible;
            _hideInputAtStartup = true;
        }

        protected PopupViewModel()
        {
            //KexContainer.Container.Compose(this); //Change
            TitleVisible        = Options.PopupTitleVisible;
            _hideInputAtStartup = true;
        }

        [Import]
        public Workspace Workspace { get; set; }

        [Import]
        public MouseHandler MouseHandler { get; set; }

        [Import]
        public Options Options { get; set; }

        public Visual HeaderIcon
        {
            get => _headerIcon;
            set
            {
                if (Equals(value, _headerIcon)) return;
                _headerIcon = value;
                OnPropertyChanged();
            }
        }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (value.Equals(_isOpen)) return;
                _isOpen = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool HandlerRegistered
        {
            get => _handlerRegistered;
            set
            {
                if (value == _handlerRegistered) return;
                _handlerRegistered = value;
                OnPropertyChanged();
            }
        }

        public LoadingStatus LoadingStatus
        {
            get => _loadingStatus;
            set
            {
                _loadingStatus = value;
                OnPropertyChanged();
            }
        }

        public bool HideInputAtStartup
        {
            get => _hideInputAtStartup;
            set
            {
                if (value.Equals(_hideInputAtStartup)) return;
                _hideInputAtStartup = value;
                OnPropertyChanged();
            }
        }

        public bool TitleVisible
        {
            get => _titleVisible;
            set
            {
                if (value == _titleVisible) return;
                _titleVisible = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ItemsView
        {
            get => _itemsView;
            protected set
            {
                _itemsView = value;
                OnPropertyChanged();
            }
        }

        public Action<int> SetCaret         { get; set; }
        public Action      SelectAll        { get; set; }
        public Action      FocusTextbox     { get; set; }
        public Action      UnSelectListView { get; set; }

        public bool MousePlacement
        {
            get => _mousePlacement;
            set
            {
                if (value == _mousePlacement) return;
                _mousePlacement = value;
                OnPropertyChanged();
            }
        }

        public bool MenuButtonPlacement
        {
            get => _menuButtonPlacement;
            set
            {
                if (value == _menuButtonPlacement)
                    return;
                _menuButtonPlacement = value;
                OnPropertyChanged();
            }
        }

        private bool   _handlerRegistered;
        private Visual _headerIcon;
        private bool   _hideInputAtStartup;
        private bool            _isOpen;
        private ICollectionView _itemsView;
        private LoadingStatus   _loadingStatus;
        private bool            _mousePlacement;
        private string          _text;
        private string          _title;
        private bool            _titleVisible;
        private bool _menuButtonPlacement;

        protected void SetHeaderIconByKey(string key)
        {
            HeaderIcon = Application.Current.TryFindResource(key) as Canvas;
        }

        protected void OnGotItems(object sender, EventArgs args)
        {
            GotItems?.Invoke(sender, args);
        }

        public event EventHandler GotItems;

        public virtual void TextChanged(object sender, TextChangedEventArgs ea)
        {
        }

        public virtual void KeyDown(object sender, KeyEventArgs ea)
        {
        }

        public virtual void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
        }

        public virtual void SelectionChanged(SelectionChangedEventArgs ea)
        {
        }

        public virtual void PreviewTextInput(object sender, TextCompositionEventArgs ea)
        {
        }

        public abstract void MouseSelection(object o);

        public virtual void Open()
        {
            Text = "";
            IsOpen = true;
        }

        public virtual void Close()
        {
            IsOpen = false;
        }
    }

    public class PopupViewModel<T> : PopupViewModel where T : class, IItem
    {
        protected PopupViewModel(Workspace workspace, Options options, MouseHandler mouseHandler) : base(workspace, options, mouseHandler)
        {
        }

        protected PopupViewModel() {}

        public CommandRepository CommandRepository => Workspace.CommandRepository;

        public bool ItemSelectedFromListView { get; set; }

        public IEnumerable<T> BaseItems
        {
            get => _baseItems;
            set
            {
                _baseItems = value;
                OnPropertyChanged();
                Items = value;
            }
        }

        public IEnumerable<T> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items))
                    return;

                _items = value;
                OnPropertyChanged();
                ItemsView = _items == null ? null : CollectionViewSource.GetDefaultView(_items);
                if (value != null)
                    OnGotItems(this, EventArgs.Empty);
            }
        }

        private IEnumerable<T> _baseItems;

        private IEnumerable<T> _items;

        protected bool IgnoreTextChanged;

        protected virtual void ItemSelected(T selectedItem)
        {
        }

        public override void TextChanged(object sender, TextChangedEventArgs ea)
        {
            if (Items == null || ItemsView == null || IgnoreTextChanged)
                return;

            var filtered = new ItemFilter<T>(BaseItems, Text);
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

        public override void KeyDown(object sender, KeyEventArgs ea)
        {
            if (ea.Key == Key.Escape || ea.Key == KeyDispatcher.AlternateEscape)
            {
                ea.Handled = true;
                IsOpen     = false;
            }
            else if (ea.Key == Key.Return)
            {
                if (ItemsView != null)
                    ItemSelected(ItemsView.CurrentItem as T);
                else
                    ItemSelected(default);
                ea.Handled = true;
            }
        }

        public override void PreviewKeyDown(object sender, KeyEventArgs ea)
        {
            var ctrl  = (ea.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            var shift = (ea.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            if (ea.Key == KeyDispatcher.MoveDownKey)
            {
                if (ctrl)
                {
                    MoveDown();
                    ea.Handled = true;
                }
            }
            else if (ea.Key == KeyDispatcher.MoveUpKey)
            {
                if (ctrl)
                {
                    MoveUp();
                    ea.Handled = true;
                }
            }
            else if (ea.Key == Key.Down)
            {
                MoveDown();
                ea.Handled = true;
            }
            else if (ea.Key == Key.Up)
            {
                MoveUp();
                ea.Handled = true;
            }
            else if (ea.Key == Key.Tab)
            {
                if (shift)
                {
                    MoveUp();
                    ea.Handled = true;
                }
                else
                {
                    MoveDown();
                    ea.Handled = true;
                }
            }
        }

        protected void MoveDown()
        {
            if (ItemsView != null) //maybe no items in Listview (filter)
            {
                ItemsView.MoveCurrentToNext();
                if (ItemsView.IsCurrentAfterLast)
                    ItemsView.MoveCurrentToFirst();
                ItemSelectedFromListView = true;
            }
        }

        protected void MoveUp() //maybe no items in Listview (filter)
        {
            if (ItemsView != null)
            {
                ItemsView.MoveCurrentToPrevious();
                if (ItemsView.IsCurrentBeforeFirst)
                    ItemsView.MoveCurrentToLast();
                ItemSelectedFromListView = true;
            }
        }

        public override void SelectionChanged(SelectionChangedEventArgs ea)
        {
        }

        public override void MouseSelection(object o)
        {
            ItemSelected(o as T);
        }

        protected void FocusInput()
        {
            FocusTextbox?.Invoke();
        }
    }
}