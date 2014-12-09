using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class ucLog : UserControl
    {
        RadTreeNode _treeNode =null;
        private static ucLog _instance;
        public static ucLog GetInstance()
        {
            if (_instance == null)
                _instance = new ucLog();
            return _instance;
        }
        private ucLog()
        {
            InitializeComponent();
            
        }
        public ucLog GetControl(XmlNode log,RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = log;
            this.txtLogName.Text = log.Attributes["name"].Value;
            this.txtLogName.Tag = log.Attributes["name"];
            this.txtMessage.Text = log.Attributes["message"].Value;
            this.txtMessage.Tag = log.Attributes["message"];
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

        private void lblDelay_Click(object sender, EventArgs e)
        {

        }
    }
}
