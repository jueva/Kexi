using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            Workspace = workspace;
            Configuration = KeyConfigurationSerializer.GetConfiguration();
            var keyModeBindings = Configuration.Bindings;

            Workspace.PropertyChanged += Workspace_PropertyChanged;
            _viStyleKeyHandler        =  new ViStyleKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.ViStyle)?.KeyBindings);
            _classicKeyHandler        =  new ClassicKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.Classic)?.KeyBindings);
            //Livefilter uses same Keybindingings as Classic
            _liveFilterKeyHandler = new LiveFilterKeyHandler(workspace, keyModeBindings.FirstOrDefault(b => b.KeyMode == KeyMode.Classic)?.KeyBindings);
        }

        private Workspace Workspace { get; }
        public IEnumerable<KexBinding> ViBindings => _viStyleKeyHandler.Bindings;
        public IEnumerable<KexBinding> ClassicBindings => _classicKeyHandler.Bindings;

        public IEnumerable<KexBinding> AllBindings => ViBindings.Concat(ClassicBindings);

        private readonly ClassicKeyHandler    _classicKeyHandler;
        private readonly LiveFilterKeyHandler _liveFilterKeyHandler;
        private readonly ViStyleKeyHandler    _viStyleKeyHandler;
        public KeyConfiguration Configuration { get; }

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Workspace.ActiveLister)) _classicKeyHandler.ClearSearchString();
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