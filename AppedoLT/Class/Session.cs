using System.Windows.Forms;

namespace AppedoLT
{
   public static class Session
    {
       private static bool _sessionActive = false;

       public static string UserID = string.Empty;
       public static string MachineUniqueID = string.Empty;
       public static int UserCount = 0;
       public static bool IsLicenseValid = false;
      
       public static bool Login()
       {
           if (_sessionActive == false)
           {
               frmLogin login = new frmLogin();
               if (login.ShowDialog() == DialogResult.OK )
               {
                   _sessionActive = true;
                  
               }
               else
               {
                   _sessionActive = false;
               }
           }
           else
           {
               _sessionActive = true;
           }
           return _sessionActive;
       }
       public static void LogOut()
       {
           _sessionActive = false;
       }
       public static bool RegisterMachine()
       {
         
           return true;
       }
    }
}
