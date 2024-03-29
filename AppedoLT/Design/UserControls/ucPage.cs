﻿using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set Page name and delay.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class ucPage : UserControl
    {    
        RadTreeNode _treeNode = null;
        private static ucPage _instance;

        public static ucPage GetInstance()
        {
            if (_instance == null)
                _instance = new ucPage();
            return _instance;
        }
        private ucPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Used to created ucPage object.
        /// </summary>
        /// <param name="xmlNode">page xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
        public ucPage GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.lblPage.Text = xmlNode.Attributes["name"].Value;
            this.txtPageDelay.Text = xmlNode.Attributes["delay"].Value;
            this.txtPageDelay.Tag = xmlNode.Attributes["delay"];
            this.Dock = DockStyle.Fill;
            return this;
        }


        private void txt_Validated(object sender, EventArgs e)
        {
            try
            {
                RadTextBox txt = (RadTextBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
                //If there is any change in page name, We need to update in page xml node.
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

        private void lblDelay_Click(object sender, EventArgs e)
        {

        }

        private void ucPage_Load(object sender, EventArgs e)
        {

        }
    }
}
