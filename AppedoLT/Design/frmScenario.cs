using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Net;
using System.Xml;

namespace AppedoLT
{
    public partial class frmScenario : Telerik.WinControls.UI.RadForm
    {
     
        string _scenarioName = string.Empty;
        XmlNode _scenario;
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();
        List<XmlNode> _alreadyMappedScripts = new List<XmlNode>();
        string _scenarioId="0";

        public frmScenario(XmlNode VUScripts)
        {
        
            InitializeComponent();

            DataTable leftList = new DataTable();
            leftList.Columns.Add("id");
            leftList.Columns.Add("name");
            
            DataTable rightList = new DataTable();
            rightList.Columns.Add("id");
            rightList.Columns.Add("name");

            _scenarioName = txtScenarioName.Text;
            if (_scenarioName != String.Empty)
                txtScenarioName.Enabled = false;

            foreach (XmlNode script in VUScripts.ChildNodes)
            {
                rightList.Rows.Add(script.Attributes["id"].Value, script.Attributes["name"].Value);
            }

            //foreach (XmlNode script in VUScripts.ChildNodes)
            //{
            //    if (leftList.Select("id='" + script.Attributes["id"].Value + "'").Length <= 0)
            //        rightList.Rows.Add(script.Attributes["id"].Value, script.Attributes["name"].Value);
            //}
            listBoxMove1.SetItems(leftList, rightList);
        }
        public frmScenario(XmlNode VUScripts,XmlNode scenario)
        {

            InitializeComponent();
            _scenario = scenario;
            _scenarioName = _scenario.Attributes["name"].Value;
            _scenarioId=_scenario.Attributes["id"].Value;
            txtScenarioName.Text = _scenarioName;
            chkIPSpoofing.Checked =Convert.ToBoolean(_scenario.Attributes["enableipspoofing"].Value);
            DataTable leftList = new DataTable();
            leftList.Columns.Add("id");
            leftList.Columns.Add("name");

            DataTable rightList = new DataTable();
            rightList.Columns.Add("id");
            rightList.Columns.Add("name");

            foreach (XmlNode script in VUScripts.ChildNodes)
            {
                if (scenario.SelectSingleNode("script[@id='" + script.Attributes["id"].Value + "']") == null)
                {
                    rightList.Rows.Add(script.Attributes["id"].Value, script.Attributes["name"].Value);
                }
                else
                {
                    XmlNode alreadyMapped=scenario.SelectSingleNode("script[@id='"+script.Attributes["id"].Value+"']");
                    if(alreadyMapped!=null)
                    { _alreadyMappedScripts.Add(alreadyMapped);
                    }
                    leftList.Rows.Add(script.Attributes["id"].Value, script.Attributes["name"].Value);
                }
            }

            listBoxMove1.SetItems(leftList, rightList);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validate()==true )
            {
               
                //New
                if (_scenario == null)
                {
                    _scenario = repositoryXml.CreateScenario();
                }
                //Update
                else
                {
                    _scenario.RemoveAll();
                    _scenario.Attributes.Append(repositoryXml.GetAttribute("id", _scenarioId));
                }
                _scenario.Attributes.Append(repositoryXml.GetAttribute("name", txtScenarioName.Text));
                _scenario.Attributes.Append(repositoryXml.GetAttribute("enableipspoofing",chkIPSpoofing.Checked.ToString()));
                foreach (DataRow dr in listBoxMove1.GetMappedItems().Rows)
                {
                    XmlNode script=null;
                    if ((script = _alreadyMappedScripts.Find(p => p.Attributes["id"].Value == dr[0].ToString())) == null)
                    {
                        script = repositoryXml.doc.CreateElement("script");
                        script.Attributes.Append(repositoryXml.GetAttribute("id", dr[0].ToString()));
                        script.Attributes.Append(repositoryXml.GetAttribute("name", dr[1].ToString()));

                        XmlNode setting = repositoryXml.doc.CreateElement("setting");
                        setting.Attributes.Append(repositoryXml.GetAttribute("type", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("durationtime", "0;0;0"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("incrementtime", "0;0;0"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("iterations", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("maxuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("startuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("incrementuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("browsercache", "false"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("startuserid", "0"));
                        script.AppendChild(setting);
                    }
                    _scenario.AppendChild(script);
                }
                repositoryXml.doc.SelectNodes("//scenarios")[0].AppendChild(_scenario);
                repositoryXml.Save();
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }

        }
        private bool Validate()
        {
            bool isValid=true;
            if (txtScenarioName.Text.Trim() == string.Empty)
            {
                erpRequired.SetError(txtScenarioName, "Required");
            }
            else
            {
                XmlNode existingNode = repositoryXml.doc.SelectSingleNode("//scenarios/scenario[@name='" + txtScenarioName .Text+ "' and @id !='"+_scenarioId+"']");
              if (existingNode != null)
              {
                  erpRequired.SetError(txtScenarioName, "Scenario name already exist");
              }
              else
              {
                  erpRequired.SetError(txtScenarioName, string.Empty);
              }
            }
            if (listBoxMove1.GetMappedItems().Rows.Count == 0)
            {
                erpRequired.SetError(listBoxMove1, "Please map a script");
            }
            else
            {
                erpRequired.SetError(listBoxMove1,string.Empty);
            }

            if (erpRequired.GetError(txtScenarioName) == string.Empty && erpRequired.GetError(listBoxMove1) == string.Empty) return true;
            return false;
        }
       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
     
    }
}
