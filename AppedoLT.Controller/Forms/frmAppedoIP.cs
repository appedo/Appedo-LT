using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppedoLTController
{
    public partial class frmAppedoIP : Form
    {
        public frmAppedoIP()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                Test();
                MessageBox.Show("Tested successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Test();
                ControllerXml.GetInstance().doc.SelectSingleNode("//runs").Attributes["appedoipaddress"].Value = txtIpAddress.Text;
                ControllerXml.GetInstance().Save();
                MessageBox.Show("Saved successfully");
            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.None;
                MessageBox.Show(ex.Message);
            }
        }
        private void Test()
        {
            Trasport loadGenConnection = new Trasport(txtIpAddress.Text, ConfigurationManager.AppSettings["appedoport"], 5000);
           
            loadGenConnection.Send(new TrasportData("TEST", string.Empty, null));
            TrasportData data = loadGenConnection.Receive();
        }
    }
}
