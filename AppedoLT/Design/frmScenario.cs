using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{
    /// <summary>
    /// Form used to create and update scenario.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class frmScenario : Telerik.WinControls.UI.RadForm
    {
     
        string _scenarioName = string.Empty;
        XmlNode _scenario;
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();
        List<XmlNode> _alreadyMappedScripts = new List<XmlNode>();
        string _scenarioId="0";

        /// <summary>
        /// Create new scenario.
        /// </summary>
        /// <param name="VUScripts">List of available scripts</param>
        public frmScenario(Dictionary<string,string> VUScripts)
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

            foreach (string key in VUScripts.Keys)
            {
                rightList.Rows.Add(key,VUScripts[key]);
            }

            listBoxMove1.SetItems(leftList, rightList);
        }

        /// <summary>
        /// Edit scenario
        /// </summary>
        /// <param name="VUScripts">Available scripts</param>
        /// <param name="scenario">Scenario need to be edit</param>
        public frmScenario(Dictionary<string, string> VUScripts, XmlNode scenario)
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

            foreach (string key in VUScripts.Keys)
            {
                if (scenario.SelectSingleNode("script[@id='" + key + "']") == null)
                {
                    rightList.Rows.Add(key, VUScripts[key]);
                }
                else
                {
                    XmlNode alreadyMapped = scenario.SelectSingleNode("script[@id='" + key + "']");
                    if (alreadyMapped != null)
                    {
                        _alreadyMappedScripts.Add(alreadyMapped);
                    }
                    leftList.Rows.Add(key,VUScripts[key]);
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
                        script = repositoryXml.Doc.CreateElement("script");
                        script.Attributes.Append(repositoryXml.GetAttribute("id", dr[0].ToString()));
                        script.Attributes.Append(repositoryXml.GetAttribute("name", dr[1].ToString()));

                        XmlNode setting = repositoryXml.Doc.CreateElement("setting");
                        setting.Attributes.Append(repositoryXml.GetAttribute("type", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("durationtime", "0;0;0"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("incrementtime", "0;0;0"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("iterations", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("maxuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("startuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("incrementuser", "1"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("browsercache", "false"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("startuserid", "0"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("replythinktime", "true"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("enableparallelcon", "false"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("parallelconnections", "6"));
                        setting.Attributes.Append(repositoryXml.GetAttribute("bandwidth", "-1"));
                        script.AppendChild(setting);
                    }
                    _scenario.AppendChild(script);
                }
                repositoryXml.Doc.SelectNodes("//scenarios")[0].AppendChild(_scenario);
                repositoryXml.Save();
                this.Close();

                MessageBox.Show("Scenario mapped successfully");
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }

        }

        /// <summary>
        /// Validate user input.
        /// </summary>
        /// <returns>True if user input is valid</returns>
        private bool Validate()
        {
            bool isValid=true;
            if (txtScenarioName.Text.Trim() == string.Empty)
            {
                erpRequired.SetError(txtScenarioName, "Required");
            }
            else
            {
                XmlNode existingNode = repositoryXml.Doc.SelectSingleNode("//scenarios/scenario[@name='" + txtScenarioName .Text+ "' and @id !='"+_scenarioId+"']");
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
