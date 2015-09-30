using System;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Xml;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set WhileLoop condition.
    /// 
    ///  Author: Rasith
    /// </summary>
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

        /// <summary>
        /// Used to created ucWhileLoop object.
        /// </summary>
        /// <param name="xmlNode">WhileLoop xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
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
            //If there is any change in While loop value, We need to update in While loop xml node.
            if (txt.Text != attr.Value)
            {
                if (attr.Name == "name" && _treeNode != null) _treeNode.Text = txt.Text;
                attr.Value = txt.Text;
            }
        }
    }
}
