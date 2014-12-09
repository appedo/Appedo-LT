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
    public partial class ucDelay : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucDelay _instance;
        public static ucDelay GetInstance()
        {
            if (_instance == null)
                _instance = new ucDelay();
            return _instance;
        }
        private ucDelay()
        {
            InitializeComponent();
        }

        public ucDelay GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtDelay.Text = xmlNode.Attributes["delaytime"].Value;
            this.txtDelay.Tag = xmlNode.Attributes["delaytime"];
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
