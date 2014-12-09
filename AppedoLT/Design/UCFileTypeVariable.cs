using System;
using System.Data;
using System.Windows.Forms;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class UCFileTypeVariable : UserControl
    {
        public UCFileTypeVariable()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select file";
            fdlg.InitialDirectory = @"c:\";
            fdlg.FileName = txtFileName.Text;
            fdlg.Filter = "Text and CSV Files(*.txt, *.csv)|*.txt;*.csv|Text Files(*.txt)|*.txt|CSV Files(*.csv)|*.csv|All Files(*.*)|*.*";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {

                txtFileName.Text = fdlg.FileName;
            }
        }

        public void btnViewData_Click(object sender, EventArgs e)
        {

            if (txtFileName.Text.Trim().Length == 0)
            {

                MessageBox.Show("File location required");
                dgvData.DataSource = null;
            }
            else
            {
                if (System.IO.File.Exists(txtFileName.Text))
                {
                    Import();
                    Application.DoEvents();
                }
                else
                {
                    if (e != null)
                    {
                        MessageBox.Show("File is not exists");
                    }
                }
            }
        }
        private void Import()
        {
            if (txtFileName.Text.Trim() != string.Empty)
            {
                try
                {
                    DataTable dt =Utility.GetDataTableFromCSVFile(txtFileName.Text,txtDelimiter.Text);
                    dgvData.DataSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        public void ClearAll()
        {
            txtFileName.Text = string.Empty;
            dgvData.DataSource = null;
        }

        private void txtStartFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) == false && char.IsControl(e.KeyChar) == false)
            {
                e.Handled = true;
            }
        }

        private void txtStartFrom_Leave(object sender, EventArgs e)
        {
            if (txtStartFrom.Text.Trim().Length == 0 || Convert.ToInt16(txtStartFrom.Text)==0)
            {
                txtStartFrom.Text ="1";
            }
        }
    }
}
