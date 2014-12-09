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

namespace AppedoLT
{
    public partial class ucTransaction : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucTransaction _instance;
        public static ucTransaction GetInstance()
        {
            if (_instance == null)
                _instance = new ucTransaction();
            return _instance;
        }
        private ucTransaction()
        {
            InitializeComponent();
        }
        public ucTransaction GetControl(XmlNode transaction,RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = transaction;
            this.txtStartTransactionName.Text = transaction.Attributes["transactionname"].Value;
            this.txtStartTransactionName.Tag = transaction.Attributes["transactionname"];
            this.Dock = DockStyle.Fill;
            return this;
        }
        private void txt_Validated(object sender, EventArgs e)
        {
            RadTextBox txt = (RadTextBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value )
            {
                if (attr.Name == "name" && _treeNode!=null) _treeNode.Text = txt.Text;
                attr.Value = txt.Text;
            }
        }
    }
}
