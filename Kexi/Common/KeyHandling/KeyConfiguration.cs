﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.Common.KeyHandling
{
    [Serializable]
    public class KeyConfiguration
    {
        public List<KeyModeBindings> Bindings { get; } = new List<KeyModeBindings>();
    }
}
