using System;
using System.Linq;
using System.Windows.Forms;
using AppedoLT.DataAccessLayer;
using System.IO;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class frmRun : Telerik.WinControls.UI.RadForm
    {
        public string strReportName = string.Empty;
        ErrorProvider errName = new ErrorProvider();
        public frmRun()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtReportName.Text != String.Empty)
            {
                errName.Clear();
                string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + txtReportName.Text;
                if (Directory.Exists(folderPath))
                {
                    MessageBox.Show("Name Alredy Exists");
                }
                else
                {
                    Directory.CreateDirectory(folderPath);
                    Directory.CreateDirectory(folderPath + "\\Report");
                    File.Copy(Constants.GetInstance().ExecutingAssemblyLocation + "\\database.db", folderPath + "\\database.db");
                    File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat");
                    File.WriteAllText(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat", @"sqlite3 " + "\""+Constants.GetInstance().ExecutingAssemblyLocation + @"\Data\" + txtReportName.Text + "\\database.db\"" + " < \"" + Constants.GetInstance().ExecutingAssemblyLocation + "\\commands.txt\"");

                    // For the logging
                    DataServer.GetInstance().ReportName = txtReportName.Text;
                    strReportName = txtReportName.Text;
                    this.DialogResult = DialogResult.OK;
                }
            }
            else
                errName.SetError(txtReportName, "Please fill the required field");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtReportName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) == true || char.IsLetterOrDigit(e.KeyChar) == true || e.KeyChar == '_'))
            {
                e.Handled = true;
            }
        }
    }
}
