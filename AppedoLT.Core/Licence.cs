using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net.NetworkInformation;
using System.Web.UI.WebControls;

namespace AppedoLT.Core
{
    public class Licence
    {
        public string DecryptPassword(string encryptedPassword)
        {
            byte[] passByteData = Convert.FromBase64String(encryptedPassword);
            string originalPassword = System.Text.Encoding.Unicode.GetString(passByteData);
            return originalPassword;
        }

        public String ReadFile()
        {
            //string strFileName = HttpContext.Current.Server.MapPath("");
            //strFileName = strFileName.Substring(0, strFileName.LastIndexOf("CAPPP.PL"));
            string strFileName = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "setting\\lic.txt";
            System.IO.StreamReader myFile = new System.IO.StreamReader(strFileName);
            string myString = myFile.ReadToEnd();
            myFile.Close();
            return myString;
        }

        public void WriteLicFile(String Data)
        {
            try
            {
                //string strFileName = "lic.txt";
                StreamWriter sw;
                //string strFilePath = HttpContext.Current.Server.MapPath("setting") + @"\" + strFileName; ;
                //string strFileName = HttpContext.Current.Server.MapPath("");
                string strFileName = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "setting\\lic.txt"; ;
                //strFileName = strFileName.Substring(0, strFileName.LastIndexOf("CAPPP.PL"));
                //strFileName += "CAPPP.PL\\setting\\lic.txt";
                if (!File.Exists(strFileName))
                {
                    sw = new StreamWriter(strFileName, true);
                    sw.WriteLine(Data);
                    sw.Close();
                }
            }
            catch
            {

            }
        }

        public string EncryptPassword(string txtPassword)
        {
            byte[] passBytes = System.Text.Encoding.Unicode.GetBytes(txtPassword);
            string encryptPassword = Convert.ToBase64String(passBytes);
            return encryptPassword;
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            } return sMacAddress;
        }
    }
}
