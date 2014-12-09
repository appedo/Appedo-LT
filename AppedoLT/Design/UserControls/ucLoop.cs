using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class ucLoop : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucLoop _instance;
        public static ucLoop GetInstance()
        {
            if (_instance == null)
                _instance = new ucLoop();
            return _instance;
        }
        private ucLoop()
        {
            InitializeComponent();

        }
        public ucLoop GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtLoopCount.Text = xmlNode.Attributes["loopcount"].Value;
            this.txtLoopCount.Tag = xmlNode.Attributes["loopcount"];
            this.txtName.Text = xmlNode.Attributes["name"].Value;
            this.txtName.Tag = xmlNode.Attributes["name"];
            this.Dock = DockStyle.Fill;
            return this;
        }
        private void txt_Validated(object sender, EventArgs e)
        {
            try
            {
                RadTextBox txt = (RadTextBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
                if (txt.Text != attr.Value)
                {
                    if (attr.Name == "name" && _treeNode != null) _treeNode.Text = txt.Text;
                    attr.Value = txt.Text;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void txtName_Validated(object sender, EventArgs e)
        {
            try
            {
                RadTextBox txt = (RadTextBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
                if (txt.Text != attr.Value)
                {
                    XmlNode vuscipt = Constants.GetInstance().FindThirdRoot((XmlNode)attr.OwnerElement);
                    if (vuscipt.SelectSingleNode(".//loop[@name='" + txt.Text + "']") != null)
                    {
                        txt.Text = Constants.GetInstance().GetUniqueLoopName(txt.Text, vuscipt, 1);
                    }
                    attr.Value = txt.Text;
                    if (attr.Name == "name" && _treeNode != null) _treeNode.Text = txt.Text;
                }
               
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(char.IsControl(e.KeyChar) == true || char.IsDigit(e.KeyChar) == true))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
