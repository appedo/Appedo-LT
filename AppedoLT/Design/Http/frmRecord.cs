using System;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
namespace AppedoLT
{
    public partial class frmRecord : Telerik.WinControls.UI.RadForm
    {
        Record rd = null;
        Design frm = null;
        XmlNode _vuscript;
        public frmRecord(Design _frm, string vuName, XmlNode _vuScript)
        {
            InitializeComponent();
            ThreadPool.SetMaxThreads(2, 2);
            ddlParentContainer.SelectedIndex = 1;
            this.Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2)-(this.Size.Width/2), 2);
            lblRequest.Text = string.Empty;
            Label.CheckForIllegalCrossThreadCalls = false;
            rd = new Record(lblRequest, txtContainer,ddlParentContainer, _vuScript);
            frm = _frm;
            rd.Start();

        }
        public frmRecord(Design _frm,XmlNode vuscript)
        {
            InitializeComponent();
            _vuscript = vuscript;
            lblRequest.Text = string.Empty;
           
            Label.CheckForIllegalCrossThreadCalls = false;
            rd = new Record(lblRequest, txtContainer,ddlParentContainer, vuscript);
            frm = _frm;
            rd.Start();

        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            rd.Stop();
            ThreadPool.SetMaxThreads(50, 50);
            frm.Visible = true; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string result = Utility.SerializeObjectToXML(Utility.GetFileContent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\Repository.xml"),rd.loadscenario);
        }

        private void ddlParentContainer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
