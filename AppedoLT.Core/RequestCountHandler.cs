using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppedoLT.Core
{
    public class RequestCountHandler
    {
        // To capture the number of hits
        public static long _ReqCount = 0;
        // To capture the network IP which is connected to the system
        public static string _NetworkConnectedIP = "";
    }
}