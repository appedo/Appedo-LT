using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{
    public partial class frmVUScriptName : Telerik.WinControls.UI.RadForm
    {
        public string name = string.Empty;
        public XmlNode node;
        string _type;
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();

        public frmVUScriptName(string type)
        {
            InitializeComponent();
            _type = type;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Validate() == true)
            {
                if (repositoryXml.doc.SelectNodes("root/vuscripts/vuscript[@name='" + txtName.Text + "']").Count > 0)
                {
                    MessageBox.Show("Script name already exist");
                }
                else
                {
                    XmlNode vuscripts = repositoryXml.doc.SelectNodes("root/vuscripts")[0];
                    node = repositoryXml.doc.CreateElement("vuscript");
                    node.Attributes.Append(repositoryXml.GetAttribute("name", txtName.Text));
                    node.Attributes.Append(repositoryXml.GetAttribute("id", repositoryXml.ScriptId));
                    node.Attributes.Append(repositoryXml.GetAttribute("type", _type));
                    node.Attributes.Append(repositoryXml.GetAttribute("exclutionfiletypes", string.Empty));
                    node.Attributes.Append(repositoryXml.GetAttribute("dynamicreqenable", false.ToString()));
                    vuscripts.AppendChild(node);
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool Validate()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                erpRequired.SetError(txtName, "Required");
            }
            else
            {
                erpRequired.SetError(txtName, string.Empty);
            }
            if (erpRequired.GetError(txtName) == string.Empty) return true;
            else return false;
        }
      
    }
}
