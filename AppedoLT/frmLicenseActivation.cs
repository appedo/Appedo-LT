using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AppedoLT.Core;
using System.Net;
using System.Xml;
namespace AppedoLT
{
    public partial class frmLicenseActivation : Telerik.WinControls.UI.RadForm
    {
        Constants constants =  Constants.GetInstance();
        public frmLicenseActivation()
        {
            InitializeComponent();
        }
        private bool Validate()
        {
            if (txtFirstname.Text.Trim() == string.Empty)
            {
                errLicense.SetError(txtFirstname, "Required");
            }
            else
            {
                errLicense.SetError(txtFirstname, string.Empty);
            }

            if (txtCountry.Text.Trim() == string.Empty)
            {
                errLicense.SetError(txtCountry, "Required");
            }
            else
            {
                errLicense.SetError(txtCountry, string.Empty);
            }

            if (txtPhoneno.Text.Trim() == string.Empty)
            {
                errLicense.SetError(txtPhoneno, "Required");
            }
            else
            {
                if (phone(txtPhoneno.Text))
                {
                    errLicense.SetError(txtPhoneno, string.Empty);
                }
                else
                {
                    errLicense.SetError(txtPhoneno, "Invalid phone number");
                }
            }
            if(txtEmail.Text.Trim()==string.Empty)
            {
                
                errLicense.SetError(txtEmail, "Required");
            }
            else
            {
                if (email(txtEmail.Text))
                {
                    errLicense.SetError(txtEmail, string.Empty);
                }
                else
                {
                    errLicense.SetError(txtEmail, "Invalid email address");
                }
            }
            if (constants.MACAddress == string.Empty)
            {
                MessageBox.Show("Can't read mac address. Please use admin login");
                return false;
            }
            if (errLicense.GetError(txtFirstname) != string.Empty || errLicense.GetError(txtCountry) != string.Empty || errLicense.GetError(txtPhoneno) != string.Empty || errLicense.GetError(txtEmail) != string.Empty) return false;
            else return true;
        }
        private void btnGetTrialLicense_Click(object sender, EventArgs e)
        {
            if (Validate() == true)
            {
                bool success = false;
                bool failure = false;
                string message = string.Empty;

                string postData = "parameter=" + GetInfo("TRIAL").GetInfoToGetLicense();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://licmgr.softsmith.com/SSITLicenseManager/newLicenseRequest");
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
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseStr);
                    success = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[0].InnerText);
                    failure = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[1].InnerText);
                    message = doc.ChildNodes[1].ChildNodes[2].InnerText;
                    if (failure == false)
                    {
                        MessageBox.Show("License file sent to your mail address. Please download \"floodgates.txt\" from your mail and upload to license management.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public bool phone(string no)
        {
            System.Text.RegularExpressions.Regex expr = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
            if (expr.IsMatch(no))
            {
                return true;
            }
            else return false;
        }
        public bool email(string email)
        {
            System.Text.RegularExpressions.Regex expr = new Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (expr.IsMatch(email))
            {
                return true;
            }
            else return false;
        }
        public LicenseInfo GetInfo(string Mode)
        {
            LicenseInfo info = new LicenseInfo();
            info.FirstName = txtFirstname.Text;
            info.LastName = txtLastname.Text;
            info.Country = txtCountry.Text;
            info.CreatedDate = DateTime.Now.ToString();
            info.EmailAddress = txtEmail.Text;
            info.PhoneNo = txtPhoneno.Text;
            info.ProductId = "2013100701";
            info.ProductVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            info.MacId = constants.MACAddress;
            info.Mode = Mode;
            return info;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnGetLicense_Click(object sender, EventArgs e)
        {
            if (Validate() == true)
            {
                bool success = false;
                bool failure = false;
                string message = string.Empty;

                string postData = "parameter=" + GetInfo("LICENSE").GetInfoToGetLicense();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://licmgr.softsmith.com/SSITLicenseManager/newLicenseRequest");
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
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseStr);
                    success = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[0].InnerText);
                    failure = Convert.ToBoolean(doc.ChildNodes[1].ChildNodes[1].InnerText);
                    message = doc.ChildNodes[1].ChildNodes[2].InnerText;
                    if (failure == false)
                    {
                        MessageBox.Show("License file sent to your mail address. Please download \"floodgates.txt\" from your mail and upload to license management.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        
    }
}
