using System;
using System.Linq;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmNewHost : Telerik.WinControls.UI.RadForm
    {
        public string strHostName = string.Empty;
        ErrorProvider errName = new ErrorProvider();
        public frmNewHost()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtHostName.Text != String.Empty)
            {
                strHostName = txtHostName.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
                errName.SetError(txtHostName, "Please fill the required field");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
