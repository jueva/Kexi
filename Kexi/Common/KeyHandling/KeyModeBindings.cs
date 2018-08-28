using System;
using System.Collections.Generic;

namespace Kexi.Common.KeyHandling
{
    [Serializable]
    public class KeyModeBindings
    {
        public KeyMode          KeyMode  { get; set; }
        public List<KexBinding> KeyBindings { get; set; }
    }
}