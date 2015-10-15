using System;
using System.Collections.Generic;
using System.Xml;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using AppedoLT.Core;
using System.Windows.Forms;

namespace AppedoLT
{
    /// <summary>
    /// Form  used to do parameterization. It will show list of available variables includes extracted variables.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class frmFindAndReplace : Telerik.WinControls.UI.RadForm
    {
        public string _resultName = string.Empty;
        public string _resultValue = string.Empty;
        VuscriptXml _parentNode = null;
        ucDesign _frm;
        bool isFromSigleValue = false;

        /// <summary>
        /// If you want parameterize value only.
        /// </summary>
        /// <param name="value">Variable value</param>
        /// <param name="parent">Contain variable info</param>
        public frmFindAndReplace(string sourceText, VuscriptXml parent,ucDesign frm)
        {
            InitializeComponent();
            isFromSigleValue = true;
            txtValue.Text = sourceText;
            isFromSigleValue = true;
            _parentNode=parent;
            _frm = frm;
            LoadVariables();
        }

        /// <summary>
        /// Load all available variable into list.
        /// </summary>
        private void LoadVariables()
        {
            tvVariables.Nodes.Clear();

            //Load all variable
            foreach (XmlNode var in VariableXml.GetInstance().doc.SelectNodes("//variables/variable"))
            {
                if (var.Attributes["type"].Value == "file")
                {
                    foreach (string col in var.Attributes["columns"].Value.Split(';'))
                    {
                        RadTreeNode item = new RadTreeNode();
                        item.Text = var.Attributes["name"].Value + "." + col;
                        item.ImageIndex = 0;
                        tvVariables.Nodes.Add(item);
                    }
                }
                else
                {
                    RadTreeNode item = new RadTreeNode();
                    item.ImageIndex = 0;
                    item.Text = var.Attributes["name"].Value;
                    tvVariables.Nodes.Add(item);
                }
            }
            //Load all extractor varialbes
            foreach (XmlNode var in _parentNode.Doc.SelectNodes("//extractor"))
            {
                if (var.Attributes["selctiontype"].Value == "all")
                {
                    RadTreeNode item2 = new RadTreeNode();
                    item2.Text = var.Attributes["name"].Value + "_arraysize";
                    item2.ImageIndex = 0;
                    tvVariables.Nodes.Add(item2);

                    RadTreeNode item3 = new RadTreeNode();
                    item3.Text = var.Attributes["name"].Value + "_n";
                    item3.ImageIndex = 0;
                    tvVariables.Nodes.Add(item3);
                }
                else if (var.Attributes["selctiontype"].Value == "random")
                {
                    RadTreeNode item2 = new RadTreeNode();
                    item2.Text = var.Attributes["name"].Value + "_arraysize";
                    item2.ImageIndex = 0;
                    tvVariables.Nodes.Add(item2);

                    RadTreeNode item1 = new RadTreeNode();
                    item1.Text = var.Attributes["name"].Value + "_rand";
                    item1.ImageIndex = 0;
                    tvVariables.Nodes.Add(item1);

                    RadTreeNode item3 = new RadTreeNode();
                    item3.Text = var.Attributes["name"].Value + "_n";
                    item3.ImageIndex = 0;
                    tvVariables.Nodes.Add(item3);
                }
                else if (var.Attributes["selctiontype"].Value == "single")
                {
                    RadTreeNode item = new RadTreeNode();
                    item.Text = var.Attributes["name"].Value;
                    item.ImageIndex = 0;
                    tvVariables.Nodes.Add(item);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to replace?","Replace",MessageBoxButtons.YesNoCancel)==DialogResult.Yes)
            {
               _parentNode.Doc.InnerXml= _parentNode.Doc.InnerXml.Replace(txtSourceText.Text, txtValue.Text);
               _parentNode.Save();
               _frm.LoadTreeItem();
            }
        }

        private void tvVariables_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            TabItem tabItem = (TabItem)tabsRequestParameter.SelectedTab;
            RadTextBox txt = (RadTextBox)tabItem.ContentPanel.Controls["txtValue"];
            if (txt.SelectionLength == 0)
            {
                txt.Text = "$$" + tvVariables.SelectedNode.Text + "$$";
            }
            else
            {
                string val = "$$" + tvVariables.SelectedNode.Text + "$$";
                txt.Text = txt.Text.Insert(txt.SelectionStart, val).Remove(txt.SelectionStart + val.Length, txt.SelectionLength);
            }

            if (tvVariables.Nodes.Count < 0) btnOk.Enabled = false;
            else btnOk.Enabled = true;
        }

        private RadTextBox GetTextBox(XmlAttribute attribute)
        {
            RadTextBox txt=new RadTextBox();
            txt.Dock = System.Windows.Forms.DockStyle.Fill;
            txt.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txt.Location = new System.Drawing.Point(0, 0);
            txt.Multiline = true;
            txt.Name = attribute.Name;
            txt.Text = attribute.Value;
            txt.Tag = attribute;
            return txt;
        }

        private TabItem GetTabItem(XmlAttribute attribute)
        {
            TabItem tabi = new TabItem();
            tabi.ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            tabi.ContentPanel.CausesValidation = true;
            tabi.ContentPanel.Controls.Add(GetTextBox(attribute));
            tabi.ContentPanel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabi.ContentPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            tabi.ContentPanel.Location = new System.Drawing.Point(0, 24);
            tabi.ContentPanel.Size = new System.Drawing.Size(315, 126);
            tabi.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tabi.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            tabi.Name = attribute.Name;
            tabi.StretchHorizontally = false;
            tabi.Text = attribute.Name;
            return tabi;
        }

        private void frmFindAndReplace_Load(object sender, EventArgs e)
        {

        }
    }
}
