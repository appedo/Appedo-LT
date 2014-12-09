using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class UCMultipartPost : UserControl
    {
        List<string> type = new List<string>();
        private static UCMultipartPost _instance;
        public static UCMultipartPost GetInstance(XmlNode formData)
        {
            if (_instance == null)
                _instance = new UCMultipartPost();
            _instance.SetValue(formData);
            return _instance;
        }
        private UCMultipartPost()
        {
            InitializeComponent();
            type.AddRange(new string[] { "String", "File" });
            ddlType.DataSource = type;
            this.Dock = DockStyle.Fill;
        }

        private void SetValue(XmlNode multipartData)
        {
            lstParams.Items.Clear();
            foreach (XmlNode param in multipartData.ChildNodes)
            {
                RadListBoxItem item = new RadListBoxItem();
                item.Text = param.Attributes["name"].Value.Trim('"');
                item.Tag = param;
                lstParams.Items.Add(item);
            }
            if (lstParams.Items.Count > 0) lstParams.SelectedIndex = 0;
        }

        private void lstParams_SelectedItemChanged(object sender, Telerik.WinControls.UI.UIElements.ListBox.RadListBoxSelectionChangeEventArgs e)
        {
            if (lstParams.SelectedItem != null)
            {
                XmlNode param = (XmlNode)((RadListBoxItem)lstParams.SelectedItem).Tag;
                txtName.Text = param.Attributes["name"].Value.Trim('"');
                txtName.Tag = param.Attributes["name"];
                txtValue.Text = param.Attributes["value"].Value;
                txtValue.Tag = param.Attributes["value"];

                if (param.Attributes["contenttype"] != null)
                {
                    txtContentType.Text = param.Attributes["contenttype"].Value;
                    txtContentType.Tag = param.Attributes["contenttype"];
                }
                else
                {
                    txtContentType.Text = string.Empty;
                    txtContentType.Tag = null;
                }

                if (param.OuterXml.Contains("filename=") == true)
                {
                    ddlType.SelectedItem = "File";
                    lblValue.Text = "File Path:";

                }
                else
                {
                    ddlType.SelectedItem = "String";
                    lblValue.Text = "Value:";
                }
            }

        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            XmlAttribute node = (XmlAttribute)txtName.Tag;
            if ("\"" + txtName.Text + "\"" != node.Value)
            {
                node.Value = "\"" + txtName.Text + "\"";
            }
        }

        private void txtValue_Validated(object sender, EventArgs e)
        {
            XmlAttribute node = (XmlAttribute)txtValue.Tag;
            if (txtValue.Text != node.Value)
            {
                node.Value = txtValue.Text;
            }
        }

        private void txtContentType_Validated(object sender, EventArgs e)
        {
            if (txtContentType.Tag != null)
            {
                XmlAttribute node = (XmlAttribute)txtContentType.Tag;
                if (txtContentType.Text != node.Value)
                {
                    node.Value = txtContentType.Text;
                }
            }
            else if(txtContentType.Text.Trim()!=string.Empty)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (new RequestParameter((XmlAttribute)txtValue.Tag).ShowDialog() == DialogResult.OK)
            {
                txtValue.Text = ((XmlAttribute)txtValue.Tag).Value;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (new RequestParameter((XmlAttribute)txtName.Tag).ShowDialog() == DialogResult.OK)
            {
                txtName.Text = ((XmlAttribute)txtValue.Tag).Value;
            }
        }

   }
}
