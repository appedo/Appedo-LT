using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmLogin : Form
    {
        public string Userid { get; set; }

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
                    server.Send(new TrasportData("login", string.Empty, header));
                    TrasportData respose = server.Receive();
                    if (respose.Operation.ToLower() == "ok" && respose.Header["success"] == "1")
                    {
                        Userid = respose.Header["userid"];
                    }
                    else
                    {
                        MessageBox.Show("Invalid Email ID or Password");
                        this.DialogResult = DialogResult.None;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.None;

            }
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private bool ValidateData()
        {
            if(txtEmailid.Text.Trim()==string.Empty)
            {
                errLogin.SetError(txtEmailid ,"Required");
            }
            else
            {
                 errLogin.SetError( txtEmailid,string.Empty);
            }
            if(txtEmailid.Text.Trim()==string.Empty)
            {
                 errLogin.SetError(txtPassword,"Required");
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
