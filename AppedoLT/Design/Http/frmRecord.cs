using System;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using AppedoLT.Core;
namespace AppedoLT
{
    /// <summary>
    /// Form used to give UI during recoding. Used to give container name.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class frmRecord : Telerik.WinControls.UI.RadForm
    {
        Record rd = null;
        Design frm = null;
        XmlNode _vuscript;


        /// <summary>
        /// Create frmRecord object with parent form, script name, script node.
        /// </summary>
        /// <param name="_frm">Parent form</param>
        /// <param name="vuName">Script name</param>
        /// <param name="_vuScript">xml node to store recoded transactions</param>
        public frmRecord(Design _frm, string vuName, XmlNode _vuScript,int selectedIndex)
        {
            try
            {
                InitializeComponent();
                ThreadPool.SetMaxThreads(2, 2);
                ddlParentContainer.SelectedIndex = selectedIndex;
                this.Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2) - (this.Size.Width / 2), 2);
                lblRequest.Text = string.Empty;
                Label.CheckForIllegalCrossThreadCalls = false;
                rd = new Record(lblRequest, txtContainer, ddlParentContainer, _vuScript.OwnerDocument.SelectSingleNode("//vuscript"));
                frm = _frm;
                rd.Start();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }

        /// <summary>
        /// Create frmRecord object with parent form, script node.
        /// </summary>
        /// <param name="_frm">Parent form</param>
        /// <param name="vuscript">xml node to store recoded transactions</param>
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

        /// <summary>
        /// To stop recoding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// During form closing, We have to enable parent form view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            rd.Stop();
            ThreadPool.SetMaxThreads(50, 50);
            frm.Visible = true; 
        }

        /// <summary>
        /// Used to create First Level Containers when record starts.
        /// </summary>
        private void CreateFirstLevelContainers()
        {
            try
            {
                Common _common = Common.GetInstance();
                XmlNode container = _common.CreateContainer(_vuscript.OwnerDocument, "Initialize");
                _vuscript.AppendChild(container);
                ddlParentContainer.Items[0].Tag = container;

                container = _common.CreateContainer(_vuscript.OwnerDocument, "Actions");
                _vuscript.AppendChild(container);
                ddlParentContainer.Items[1].Tag = container;

                container = _common.CreateContainer(_vuscript.OwnerDocument, "End");
                _vuscript.AppendChild(container);
                ddlParentContainer.Items[2].Tag = container;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }


    }
}
