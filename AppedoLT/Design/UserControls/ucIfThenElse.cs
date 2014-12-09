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
    public partial class ucIfThenElse : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucIfThenElse _instance;
        public static ucIfThenElse GetInstance()
        {
            if (_instance == null)
                _instance = new ucIfThenElse();
            return _instance;
        }
        private ucIfThenElse()
        {
            InitializeComponent();
        }

        public ucIfThenElse GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtExpression.Text = xmlNode.Attributes["condition"].Value;
            this.txtExpression.Tag = xmlNode.Attributes["condition"];
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
    }
}
