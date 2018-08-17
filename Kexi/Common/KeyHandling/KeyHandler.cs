using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    [Serializable]
    public class KeyHandler : IKeyHandler
    {
        public const  Key AlternateEscape = Key.Oem102;
        public static Key MoveDownKey     = Key.J;
        public static Key MoveUpKey       = Key.K;
        public static Key MoveLeftKey     = Key.H;
        public static Key MoveRightKey    = Key.L;

        public KeyHandler(Workspace workspace)
        {
            Workspace             = workspace;
            _classicKeyHandler    = new ClassicKeyHandler(workspace);
            _liveFilterKeyHandler = new LiveFilterKeyHandler(workspace);
            _viStyleKeyHandler    = new ViStyleKeyHandler(workspace);
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

        private Workspace            Workspace { get; set; }
        private readonly ClassicKeyHandler    _classicKeyHandler;
        private readonly LiveFilterKeyHandler _liveFilterKeyHandler;
        private readonly ViStyleKeyHandler    _viStyleKeyHandler;

    }
}