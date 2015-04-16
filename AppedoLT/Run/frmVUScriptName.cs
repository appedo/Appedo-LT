using AppedoLT.Core;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{
    public partial class frmVUScriptName : Telerik.WinControls.UI.RadForm
    {
        public string name = string.Empty;
        string _type;
        public VuscriptXml vuscriptXml = null;

        public frmVUScriptName(string type)
        {
            InitializeComponent();
            _type = type;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Validate() == true)
            {
                if (VuscriptXml.GetScriptName().Contains(txtName.Text))
                {
                    MessageBox.Show("Script name already exist");
                    return;
                }
               
                string uniqueid = Constants.GetInstance().UniqueID;
                vuscriptXml = new VuscriptXml(uniqueid);

                XmlNode node = vuscriptXml.Doc.SelectSingleNode("vuscript");
                node.Attributes.Append(vuscriptXml.GetAttribute("name", txtName.Text));
                node.Attributes.Append(vuscriptXml.GetAttribute("id", uniqueid));
                node.Attributes.Append(vuscriptXml.GetAttribute("type", _type));
                node.Attributes.Append(vuscriptXml.GetAttribute("exclutionfiletypes", string.Empty));
                node.Attributes.Append(vuscriptXml.GetAttribute("dynamicreqenable", false.ToString()));
               
                this.Close();

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
