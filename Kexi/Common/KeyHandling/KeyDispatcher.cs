using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
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
        private const string KeyConfiguration = @".\keyBindings.xml";


        public KeyDispatcher(Workspace workspace)
        {
            Workspace                 =  workspace;
            var serializer = new XmlSerializer(typeof(KeyConfiguration));
            using (var file = new FileStream(KeyConfiguration, FileMode.Open))
            {
                var configuration = (KeyConfiguration) serializer.Deserialize(file);
                var keyModeBindings = configuration.Bindings;

                Workspace.PropertyChanged += Workspace_PropertyChanged;
                _viStyleKeyHandler        =  new ViStyleKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.ViStyle)?.KeyBindings);
                _classicKeyHandler        =  new ClassicKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.Classic)?.KeyBindings);
                //Livefilter uses same Keybindingings as Classic
                _liveFilterKeyHandler     =  new LiveFilterKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.Classic)?.KeyBindings);
            }
        }

        private          Workspace            Workspace { get; }
        private readonly ClassicKeyHandler    _classicKeyHandler;
        private readonly LiveFilterKeyHandler _liveFilterKeyHandler;
        private readonly ViStyleKeyHandler    _viStyleKeyHandler;

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Workspace.ActiveLister))
            {
                _classicKeyHandler.ClearSearchString();
            }
        }

        public bool Execute(KeyEventArgs args, ILister lister, string group = null)
        {
            switch (Workspace.Options.KeyMode)
            {
                case KeyMode.Classic:
                    return _classicKeyHandler.Execute(args, lister, group);
                case KeyMode.LiveFilter:
                    return _liveFilterKeyHandler.Execute(args, lister, group);
                default:
                    return _viStyleKeyHandler.Execute(args, lister, group);
            }
        }

        //private void SaveConfiguration()
        //{
            //var configuration = new KeyConfiguration();
            //configuration.Bindings.Add(new KeyModeBindings
            //{
            //    KeyMode = KeyMode.ViStyle,
            //    Bindings = bindings
            //});
            //configuration.Bindings.Add(new KeyModeBindings
            //{
            //    KeyMode = KeyMode.Classic,
            //    Bindings = new List<KexBinding>(),
            //});
            //configuration.Bindings.Add(new KeyModeBindings
            //{
            //    KeyMode = KeyMode.LiveFilter,
            //    Bindings = new List<KexBinding>(),
            //});
            //using (var f = new FileStream(@"c:\temp\keyBindings1.xml", FileMode.Create))
            //{
            //    new XmlSerializer(typeof(KeyConfiguration)).Serialize(f, configuration);
            //    f.Flush();
            //}
        //}
    }
}