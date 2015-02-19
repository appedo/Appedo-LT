using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmDownloadProgress : Form
    {
        bool cancel = false;

        public frmDownloadProgress()
        {
            InitializeComponent();
        }
        public void UpdateStatus(ref int total, ref int received, ref bool success)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = total;
            progressBar1.Value = received;

            while (((total == 0 && received == 0) || received < total))
            {

                progressBar1.Value = received;
                txtMsg.Text = total.ToString() + " / " + received.ToString();
                if(cancel==true) success = false;
                if (success == false) break;
                Thread.Sleep(1000);
            }
            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
        }
    }
}
