using System.Windows.Forms;

namespace AppedoLT
{
    /// <summary>
    /// To maintain login session.
    /// 
    /// Author: Rasith
    /// </summary>
   public static class Session
    {
       private static bool _sessionActive = false;

       public static string UserID = string.Empty;
       public static string MachineUniqueID = string.Empty;
       public static int UserCount = 0;
       public static bool IsLicenseValid = false;
      
       /// <summary>
       /// Promote login screen to user and validate user detail
       /// </summary>
       /// <returns>True: Login success, False: Login failed</returns>
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

       /// <summary>
       /// To clear login session
       /// </summary>
       public static void LogOut()
       {
           _sessionActive = false;
       }

       /// <summary>
       /// To register machine.
       /// </summary>
       /// <returns></returns>
       public static bool RegisterMachine()
       {
           return true;
       }
    }
}
