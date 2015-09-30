using AppedoLT.Core;
using System;
using System.Collections.Generic;
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
           DeregisterMachine();
       }

       /// <summary>
       /// To register machine.
       /// </summary>
       /// <returns></returns>
       public static bool RegisterMachine()
       {
           try
           {
               Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort, 120000);
               Dictionary<string, string> header = new Dictionary<string, string>();
               header.Add("userid", UserID);
               header.Add("machineid", Constants.GetInstance().MachineUniqueID);
               server.Send(new TrasportData("registermachine", string.Empty, header));
               TrasportData respose = server.Receive();
               if (respose.Header["success"] != "1") return false;
               else
               {
                   MachineUniqueID = Constants.GetInstance().MachineUniqueID;
               }
           }
           catch (Exception ex)
           {
               ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
               return false;
           }

           return true;
       }

       public static void DeregisterMachine()
       {
           try
           {
               Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort, 60000);
               Dictionary<string, string> header = new Dictionary<string, string>();
               header.Add("userid", UserID);
               header.Add("machineid", MachineUniqueID);
               server.Send(new TrasportData("deregistermachine", string.Empty, header));
               TrasportData respose = server.Receive();
               MachineUniqueID = string.Empty;
           }
           catch (Exception ex)
           {
               ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
           }
       }
    }
}
