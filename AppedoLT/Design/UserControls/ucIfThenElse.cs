using AppedoLT.Core;
using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set IfThenElse condition.
    /// 
    ///  Author: Rasith
    /// </summary>
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

        /// <summary>
        /// Used to created ucIfThenElse object.
        /// </summary>
        /// <param name="xmlNode">IfThenElse xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
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
                //If there is any change in IfThenElse value, We need to update in IfThenElse xml node.
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
