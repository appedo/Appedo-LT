﻿using AppedoLT.Core;
using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set delay value and update in script xml.
    /// 
    /// Author: Rasith
    /// </summary>
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

        /// <summary>
        /// Used to created ucDelay object.
        /// </summary>
        /// <param name="xmlNode">delay xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
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
                //If there is any change in delay value, We need to update in delay xml node.
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
                //Disable char
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
