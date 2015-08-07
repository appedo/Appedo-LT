using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppedoLT.Core
{
   public static class Session
    {
       public static string UserID = string.Empty;
       public static string MachineUniqueID = string.Empty;
       public static int UserCount = 0;
       public static bool IsLicenseValid = false;
       public static bool SessionActive = false;
    }
}
