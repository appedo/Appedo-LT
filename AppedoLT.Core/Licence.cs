using System;
using System.IO;
using System.Net.NetworkInformation;

namespace AppedoLT.Core
{
    public class Licence1
    {
        public string DecryptPassword(string encryptedPassword)
        {
            byte[] passByteData = Convert.FromBase64String(encryptedPassword);
            string originalPassword = System.Text.Encoding.Unicode.GetString(passByteData);
            return originalPassword;
        }

        public String ReadFile()
        {
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
                StreamWriter sw;
                string strFileName = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "setting\\lic.txt"; ;
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
