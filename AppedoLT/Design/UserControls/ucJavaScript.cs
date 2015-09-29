using AppedoLT.Core;
using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set JavaScript.
    /// 
    /// Author: Rasith
    /// </summary>
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

        /// <summary>
        /// Used to created ucJavaScript object.
        /// </summary>
        /// <param name="xmlNode">JavaScript xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
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
                //If there is any change in JavaScript value, We need to update in JavaScript xml node.
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
