using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public class ReplaceHost
    {
        public string CurrentSchema { get; set; }
        public string CurrentHost { get; set; }
        public string CurrentPort { get; set; }
        public string NewSchema { get; set; }
        public string NewHost { get; set; }
        public string NewPort { get; set; }
    }
}