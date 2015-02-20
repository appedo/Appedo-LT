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

        public frmDownloadProgress(bool isDownload)
        {
            InitializeComponent();
            if (isDownload) txtTitle.Text = " Downloaded / Total: ";
            else txtTitle.Text = "Uploaded / Total: ";
        }
        public void UpdateStatus(ref long total, ref long received, ref bool success)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum =(int) total;
            progressBar1.Value = (int)received;

            while (((total == 0 && received == 0) || received < total))
            {

                progressBar1.Value = (int)received;
                txtMsg.Text = string.Format("{0:0.###}", (received / 1024)) + " / " + string.Format("{0:0.###}", (total / 1024)) + " (KB)";
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
