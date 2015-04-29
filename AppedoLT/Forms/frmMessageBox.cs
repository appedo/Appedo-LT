using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace AppedoLT
{
    public partial class frmMessageBox : Telerik.WinControls.UI.RadForm
    {
        public frmMessageBox(string msg)
        {
            InitializeComponent();
            lblMessage.Text = msg;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        { 

        }

     
    }
}
