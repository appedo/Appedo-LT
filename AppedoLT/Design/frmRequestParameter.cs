using System;
using System.Collections.Generic;
using System.Xml;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class RequestParameter : Telerik.WinControls.UI.RadForm
    {
        
        public string _resultName = string.Empty;
        public string _resultValue = string.Empty;
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();
        bool isFromSigleValue = false;

        public RequestParameter(string name, string value)
        {
            InitializeComponent();
            tabiValue.Name = name;
            tabiValue.Text = name;
            txtValue.Text = value;
            isFromSigleValue = true;
            LoadVariables();
        }
        public RequestParameter(string value)
        {
            InitializeComponent();
            tabiValue.Name = "Value";
            tabiValue.Text = "Value";
            txtValue.Text = value;
            isFromSigleValue = true;
            LoadVariables();
        }
        public RequestParameter(XmlNode node)
        {
            InitializeComponent();
            tabsRequestParameter.Items.Clear();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (!(attribute.Name == "rawname" || attribute.Name == "rawvalue"))
                {
                    TabItem item = GetTabItem(attribute);
                    tabsRequestParameter.Items.Add(item);
                    if (item.Name == "value") tabsRequestParameter.SelectItem(item);
                }
            }
            LoadVariables();
        }
        public RequestParameter(XmlAttribute att)
        {
            InitializeComponent();
            tabsRequestParameter.Items.Clear();
            TabItem item = GetTabItem(att);
            tabsRequestParameter.Items.Add(item);
            tabsRequestParameter.SelectItem(item);
            LoadVariables();
        }
        private void LoadVariables()
        {
            tvVariables.Nodes.Clear();

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
            foreach (XmlNode var in repositoryXml.doc.SelectNodes("//extractor"))
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
            if (isFromSigleValue == false)
            {
                bool isChanged = false;
                foreach (TabItem tabitem in tabsRequestParameter.Items)
                {
                    if (tabitem.Visibility == ElementVisibility.Visible)
                    {
                        RadTextBox txt = (RadTextBox)tabitem.ContentPanel.Controls[tabitem.Name];
                        if (((XmlAttribute)txt.Tag).Value != txt.Text)
                        {
                            ((XmlAttribute)txt.Tag).Value = txt.Text;
                            isChanged = true;
                        }
                    }
                }
                //if (isChanged) repositoryXml.Save();
            }
            else
            {
                _resultValue = txtValue.Text;
            }
        }
        private void btnVariableManager_Click(object sender, EventArgs e)
        {
            frmVariableManager vm = new frmVariableManager();
            vm.ShowDialog();
            LoadVariables();
        }
        private void tvVariables_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            if (isFromSigleValue == false)
            {
                if (tabsRequestParameter.SelectedTab != null)
                {
                    TabItem tabItem = (TabItem)tabsRequestParameter.SelectedTab;
                    RadTextBox txt = (RadTextBox)tabItem.ContentPanel.Controls[tabItem.Name];
                    if (txt.SelectionLength == 0)
                    {
                        txt.Text = "$$" + tvVariables.SelectedNode.Text + "$$";
                    }
                    else
                    {
                        string val = "$$" + tvVariables.SelectedNode.Text + "$$";
                        txt.Text = txt.Text.Insert(txt.SelectionStart, val).Remove(txt.SelectionStart + val.Length, txt.SelectionLength);
                    }
                }
            }
            else
            {
                TabItem tabItem = (TabItem)tabsRequestParameter.SelectedTab;
                RadTextBox txt = (RadTextBox)tabItem.ContentPanel.Controls[0];
                if (txt.SelectionLength == 0)
                {
                    txt.Text = "$$" + tvVariables.SelectedNode.Text + "$$";
                }
                else
                {
                    string val = "$$" + tvVariables.SelectedNode.Text + "$$";
                    txt.Text = txt.Text.Insert(txt.SelectionStart, val).Remove(txt.SelectionStart + val.Length, txt.SelectionLength);
                }
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
    }
}