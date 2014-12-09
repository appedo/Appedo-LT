using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Xml;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class ucJavaScript : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucJavaScript _instance;
        public static ucJavaScript GetInstance()
        {
            if (_instance == null)
                _instance = new ucJavaScript();
            return _instance;
        }
        private ucJavaScript()
        {
            InitializeComponent();
        }

        public ucJavaScript GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtJavaScript.Text = xmlNode.Attributes["script"].Value;
            this.txtJavaScript.Tag = xmlNode.Attributes["script"];
            this.Dock = DockStyle.Fill;
            return this;
        }
        private void txt_Validated(object sender, EventArgs e)
        {
            try
            {
                RichTextBox txt = (RichTextBox)sender;
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
    }
}
