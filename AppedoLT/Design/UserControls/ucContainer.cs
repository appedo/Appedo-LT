using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;

namespace AppedoLT
{
    /// <summary>
    /// User control used to get container name and update in script xml.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class ucContainer : UserControl
    {
        RadTreeNode _treeNode = null;
        private static ucContainer _instance;
        public static ucContainer GetInstance()
        {
            if (_instance == null)
                _instance = new ucContainer();
            return _instance;
        }
        private ucContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Used to created ucContainer object.
        /// </summary>
        /// <param name="xmlNode">Container xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
        public ucContainer GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            if (_treeNode.Level == 1) this.txtContainer.ReadOnly = true;
            else this.txtContainer.ReadOnly = false;
            this.txtContainer.Text = xmlNode.Attributes["name"].Value;
            this.txtContainer.Tag = xmlNode.Attributes["name"];
            this.Dock = DockStyle.Fill;
            return this;
        }

        private void txtContainer_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                RadTextBox txt = (RadTextBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
                //If there is any change in container name, We need to update in container xml node.
                if (txt.Text != attr.Value)
                {
                    XmlNode vuscipt = Constants.GetInstance().FindThirdRoot((XmlNode)attr.OwnerElement);
                    if (vuscipt.SelectSingleNode(".//container[@name='" + txt.Text + "']") != null)
                    {
                        txt.Text = Constants.GetInstance().GetUniqueContainerName(txt.Text, vuscipt, 1);
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
    }
}
