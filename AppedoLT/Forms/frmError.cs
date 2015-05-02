using AppedoLT.Core;
using AppedoLT.DataAccessLayer;

namespace AppedoLT
{
    public partial class frmError : Telerik.WinControls.UI.RadForm
    {
        //RunTimeException runTimeException = new RunTimeException();
        public frmError(RunTimeException _ex)
        {
            InitializeComponent();
           // radGridView1.DataSource = _ex.requestexception;

        }
    }
}
