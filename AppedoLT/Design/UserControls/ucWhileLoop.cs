using System;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Xml;

namespace AppedoLT
{
    public partial class ucWhileLoop : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucWhileLoop _instance;
        public static ucWhileLoop GetInstance()
        {
            if (_instance == null)
                _instance = new ucWhileLoop();
            return _instance;
        }
        private ucWhileLoop()
        {
            InitializeComponent();

        }
        public ucWhileLoop GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtWhileLoop.Text = xmlNode.Attributes["condition"].Value;
            this.txtWhileLoop.Tag = xmlNode.Attributes["condition"];
            this.Dock = DockStyle.Fill;
            return this;
        }
        private void txt_Validated(object sender, EventArgs e)
        {
            RadTextBox txt = (RadTextBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value)
            {
                if (attr.Name == "name" && _treeNode != null) _treeNode.Text = txt.Text;
                attr.Value = txt.Text;
            }
        }
    }
}
