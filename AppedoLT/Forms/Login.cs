using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmLogin : Form
    {
        private string _userid = string.Empty;

        public string Userid { get { return _userid; } set { _userid = value; } }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort, 120000);
                    Dictionary<string, string> header = new Dictionary<string, string>();
                    header.Add("userid", txtEmailid.Text);
                    header.Add("pwd", txtPassword.Text);
                    header.Add("module", Application.ProductName);
                    header.Add("version", Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf('.')));

                    server.Send(new TrasportData("login", string.Empty, header));
                    TrasportData respose = server.Receive();
                    if (respose.Operation.ToLower() == "ok" && respose.Header["success"] == "1")
                    {
                        _userid = respose.Header["userid"];
                        Design.mnuiLogin.Text = "&Logout";
                    }
                    else
                    {
                        MessageBox.Show(respose.Header["message"]);
                        this.DialogResult = DialogResult.None;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.None;
            }
        }

        private bool ValidateData()
        {
            if (txtEmailid.Text.Trim() == string.Empty)
            {
                errLogin.SetError(txtEmailid, "Required");
            }
            else
            {
                errLogin.SetError(txtEmailid, string.Empty);
            }
            if (txtEmailid.Text.Trim() == string.Empty)
            {
                errLogin.SetError(txtPassword, "Required");
            }
            else
            {
                errLogin.SetError(txtPassword, string.Empty);
            }
            if (errLogin.GetError(txtEmailid) != string.Empty || errLogin.GetError(txtPassword) != string.Empty) return false;
            else return true;
        }
    }
}
