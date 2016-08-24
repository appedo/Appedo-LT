using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppedoLT.Core;

namespace AppedoLT.Report
{
    public partial class frmReportName : Form
    {
        public string ReportName
        {
            get
            {
                return txtName.Text;
            }
            set
            {
                txtName.Text = value;
            }
        }

        public frmReportName()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string compareReportFolder = Constants.GetInstance().CompareReportsFolderPath + "\\" + txtName.Text;
            if (Directory.Exists(compareReportFolder))
            {
                MessageBox.Show("Report name already exists, select other name", "AppedoLT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
