using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.Enumerations;
using System.Diagnostics;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class frmVUScriptNameHttp : Telerik.WinControls.UI.RadForm
    {
      
        public string name = string.Empty;
        public VuscriptXml vuscriptXml=null;
        string _type;

        public frmVUScriptNameHttp()
        {
            InitializeComponent();
            _type = "http";
            ddlParentContainer.SelectedIndex = 1;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Validate() == true)
            {
                if(VuscriptXml.GetScriptName().Contains(txtName.Text))
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

                if (rbtnFirefox.ToggleState == ToggleState.On)
                {
                    AppedoLT.Core.Constants.GetInstance().SetFirefoxProxy();
                    Process.Start("firefox.exe", txtOpenurl.Text);
                }

                this.Close();

            }
            //if (loadScenario.VUScripts.Exists(f => f.Name == txtName.Text) == true)
            //{
            //    MessageBox.Show("Name alredy exist");
            //}
            //else
            //{
               

            //    // XmlNodeList list=_XmlDoc.ChildNodes;
            //   // XmlAttribute attributes=new XmlAttribute("Name");
            //   // attributes.Value="txtName.Text";
            //    //_vUScript.Attributes.Append(attributes);
            //   // _vUScript.ap
            //    //VUScript vuscript = new VUScript();

            //    //vuscript.Name = txtName.Text;
 
            //    //if (loadScenario.VUScripts.Count == 0) vuscript.VUScriptid = "1";
            //    //else 
            //    //{
            //    //  vuscript.VUScriptid = (loadScenario.VUScripts.Max(r => Convert.ToInt32(r.VUScriptid))+1).ToString();
            //    //}
            //    //loadScenario.VUScripts.Add(vuscript);
            //    //loadScenario.VUScriptSettings.Add(VUScriptSetting.GetDefault( vuscript.VUScriptid ));
            //    //ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(loadScenario));
            //    //name = txtName.Text;
            //    this.Close();
            //}
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
