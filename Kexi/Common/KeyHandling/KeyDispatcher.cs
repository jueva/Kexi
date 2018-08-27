using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class KeyDispatcher
    {
        public const  Key AlternateEscape = Key.Oem102;
        public static Key MoveDownKey     = Key.J;
        public static Key MoveUpKey       = Key.K;
        public static Key MoveLeftKey     = Key.H;
        public static Key MoveRightKey    = Key.L;

        public KeyDispatcher(Workspace workspace)
        {
            Workspace                 =  workspace;
            Workspace.PropertyChanged += Workspace_PropertyChanged;
            _classicKeyHandler        =  new ClassicKeyHandler(workspace);
            _liveFilterKeyHandler     =  new LiveFilterKeyHandler(workspace);
            _viStyleKeyHandler        =  new ViStyleKeyHandler(workspace);
        }

        public List<KexBinding> Bindings
        {
            get
            {
                switch (Workspace.Options.KeyboardMode)
                {
                    case KeyboardMode.Classic:
                        return _classicKeyHandler.Bindings;
                    case KeyboardMode.LiveFilter:
                        return _liveFilterKeyHandler.Bindings;
                    default:
                        return _viStyleKeyHandler.Bindings;
                }
            }
        }

        private          Workspace            Workspace { get; }
        private readonly ClassicKeyHandler    _classicKeyHandler;
        private readonly LiveFilterKeyHandler _liveFilterKeyHandler;
        private readonly ViStyleKeyHandler    _viStyleKeyHandler;

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Workspace.ActiveLister)) _classicKeyHandler.ClearSearchString();
        }

        public bool Execute(KeyEventArgs args, ILister lister, string group = null)
        {
            switch (Workspace.Options.KeyboardMode)
            {
                case KeyboardMode.Classic:
                    return _classicKeyHandler.Execute(args, lister, group);
                case KeyboardMode.LiveFilter:
                    return _liveFilterKeyHandler.Execute(args, lister, group);
                default:
                    return _viStyleKeyHandler.Execute(args, lister, group);
            }
        }
    }
}