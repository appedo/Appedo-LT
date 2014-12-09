using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.IO;
using System.Xml;
using AppedoLT.Core;
using System.Net;
using System.Web;

namespace AppedoLT
{
    public partial class frmLicenseManagement : Telerik.WinControls.UI.RadForm
    {
        Constants constants = Constants.GetInstance();
        public frmLicenseManagement()
        {
            InitializeComponent();
            SetStatus();
        }


        private void radButton1_Click(object sender, EventArgs e)
        {
            if (ofLicense.ShowDialog() == DialogResult.OK)
            {
                txtFilename.Text = ofLicense.FileName;
            }
        }

        private void btnGetLicense_Click(object sender, EventArgs e)
        {
            new frmLicenseActivation().ShowDialog();
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {

                if (File.Exists(txtFilename.Text) == true)
                {
                    using (FileStream fileStream = new FileStream(txtFilename.Text, FileMode.Open, FileAccess.Read))
                    {
                        StreamReader read = new StreamReader(fileStream);
                        string lic = read.ReadToEnd();
                        if (File.Exists(constants.ExecutingAssemblyLocation + "\\Floodgates.bin") == true) File.Delete(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\Floodgates.bin");
                        using (FileStream licFile = new FileStream(constants.ExecutingAssemblyLocation + "\\Floodgates.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            licFile.Write(Encoding.Default.GetBytes(lic), 0, lic.Length);
                        }
                        LicenseInfo info = null;
                        try
                        {
                            info = new LicenseInfo(Constants.GetInstance().GetLicenseDoc());
                        }
                        catch (Exception ex)
                        {
                            File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                            MessageBox.Show("Unable to activate license. Invaild license");
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                            return;
                        }
                        if (info != null)
                        {
                            if (info.IsExpired == true || constants.ApplicationStartTime.Ticks > info.ExpiredData.Ticks)
                            {
                                MessageBox.Show("Lincense is expired.");
                                File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                                return;
                            }
                            else if (info.MacId != constants.MACAddress)
                            {
                                MessageBox.Show("Invalid license.");
                                File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                                return;
                            }
                            else
                            {
                                XmlDocument doc = new XmlDocument();
                                bool success = false;
                                bool failure = false;
                                string message = string.Empty;

                                string postData = "splid=" +HttpUtility.UrlEncode(constants.Encrypt(info.LicenseId));
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://licmgr.softsmith.com/SSITLicenseManager/registerLicense");
                                req.Method = "POST";
                                req.Timeout = 30000;
                                req.ContentLength = postData.Length;
                                try
                                {
                                    req.GetRequestStream().Write(Encoding.Default.GetBytes(postData), 0, postData.Length);
                                    WebResponse res = req.GetResponse();
                                    byte[] resBuffer = new byte[1024];
                                    res.GetResponseStream().Read(resBuffer, 0, Convert.ToInt32(res.ContentLength));
                                    string responseStr = Encoding.Default.GetString(resBuffer, 0, Convert.ToInt32(res.ContentLength));
                                    doc.LoadXml(responseStr);
                                    success = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[0].InnerText);
                                    failure = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[1].InnerText);
                                    message = doc.ChildNodes[1].ChildNodes[2].InnerText;
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                    return;
                                }
                                if (failure == true)
                                {

                                    File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                                    MessageBox.Show(message);
                                    return;
                                }
                                else
                                {
                                    if (info.Mode == "TRIAL")
                                        MessageBox.Show("Trial(30 days) license has been activated");
                                    else
                                    {
                                        MessageBox.Show("License has been activated");
                                    }
                                }
                                SetStatus();
                            }

                        }
                    }
                }
            }
            catch
            {

            }
        }
        private void SetStatus()
        {
            LicenseInfo info = null;
            try
            {
                info = new LicenseInfo(Constants.GetInstance().GetLicenseDoc());
            }
            catch
            {
                lblStatus.Text = "No license";
                return;
            }
            if (info != null)
            {
                if (info.IsExpired == true || constants.ApplicationStartTime.Ticks > info.ExpiredData.Ticks)
                {
                    lblStatus.Text = "Lincense is expired.";
                    File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                    return;
                }
                else if (info.MacId != constants.MACAddress)
                {
                    lblStatus.Text = "Invalid license.";
                    File.Delete(constants.ExecutingAssemblyLocation + "\\Floodgates.bin");
                    return;
                }
                else
                {
                    if (info.Mode == "TRIAL")
                        lblStatus.Text = "Trial  license is active";
                    else
                    {
                        lblStatus.Text = "License is activated";
                    }
                }
            }
        }

    }
}
