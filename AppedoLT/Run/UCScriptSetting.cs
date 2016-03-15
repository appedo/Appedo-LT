using System;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using System.IO;

namespace AppedoLT
{
    public partial class UCScriptSetting : UserControl
    {
        private string _VUScriptid = string.Empty;

        private string scenarioID = string.Empty;
        public string strScriptName = "Script Name : ";
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();

        XmlNode _setting;
        public XmlNode vUScriptSetting
        {
            set
            {
                value = value.SelectNodes("setting")[0];
                if (value.Attributes["type"].Value == "1")
                {
                    rbtnIteration.Checked = true;
                }
                else
                {
                    rbtnDuraion.Checked = true;
                }
                ucDurationTime.Time = new TimeSpan(int.Parse(value.Attributes["durationtime"].Value.Split(';')[0]), int.Parse(value.Attributes["durationtime"].Value.Split(';')[1]), int.Parse(value.Attributes["durationtime"].Value.Split(';')[2]));
                txtMaxuser.Text = value.Attributes["maxuser"].Value;
                txtStatUserCount.Text = value.Attributes["startuser"].Value;
                txtIteration.Text = value.Attributes["iterations"].Value;
                txtIncrementUser.Text = value.Attributes["incrementuser"].Value;
                ucIncrementTime.Time = new TimeSpan(int.Parse(value.Attributes["incrementtime"].Value.Split(';')[0]), int.Parse(value.Attributes["incrementtime"].Value.Split(';')[1]), int.Parse(value.Attributes["incrementtime"].Value.Split(';')[2]));
                chkBrowseCache.Checked = Convert.ToBoolean(value.Attributes["browsercache"].Value);
                if (value.Attributes["replythinktime"] != null)
                {
                    radReplyThinkTime.Checked = Convert.ToBoolean(value.Attributes["replythinktime"].Value);
                }
                else
                {
                    radReplyThinkTime.Checked = true;
                    radReplyThinkTime.Hide();
                }

                if (value.Attributes["parallelconnections"] != null)
                {                   
                    txtParallelCon.Text = value.Attributes["parallelconnections"].Value;                    
                }
                else
                {                    
                    txtParallelCon.Text = "6";                                       
                }
            }
            get
            {
                VUScriptSetting vUScriptSettingObj = new VUScriptSetting();
                if (_setting.Attributes["replythinktime"] != null)
                {
                    if (radReplyThinkTime.Checked)
                    {
                        _setting.Attributes["replythinktime"].Value = "true";
                    }
                    else
                    {
                        _setting.Attributes["replythinktime"].Value = "false";
                    }
                }

                if (chkBrowseCache.Checked == true) _setting.Attributes["browsercache"].Value = "true";
                else _setting.Attributes["browsercache"].Value = "false";
                if (rbtnIteration.Checked==true)
                {
                    _setting.Attributes["type"].Value = "1";
                }
                else
                {
                    _setting.Attributes["type"].Value = "2";
                }
                _setting.Attributes["durationtime"].Value = ucDurationTime.Time.Hours.ToString() + ";" + ucDurationTime.Time.Minutes.ToString() + ";" + ucDurationTime.Time.Seconds.ToString();
                _setting.Attributes["startuser"].Value = txtStatUserCount.Text;
                _setting.Attributes["iterations"].Value = txtIteration.Text;
                _setting.Attributes["maxuser"].Value = txtMaxuser.Text;
                _setting.Attributes["incrementuser"].Value = txtIncrementUser.Text;
                _setting.Attributes["incrementtime"].Value = ucIncrementTime.Time.Hours.ToString() + ";" + ucIncrementTime.Time.Minutes.ToString() + ";" + ucIncrementTime.Time.Seconds.ToString();
                if (_setting.Attributes["parallelconnections"] != null) 
                {
                    _setting.Attributes["parallelconnections"].Value = txtParallelCon.Text;
                }
                
                
                return _setting;
            }
        }
        public void SetLoadScenario(XmlNode setting)
        {
            _setting = setting;
            lblScriptName.Text = "Script Name : " + strScriptName;
        }

        public UCScriptSetting()
        {
            InitializeComponent();


        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //   _loadScenario.VUScriptSettings.RemoveAll(f => f.VUScriptid == _VUScriptid && f.ScenarioId == scenarioID);
                // _loadScenario.VUScriptSettings.Add(this.vUScriptSetting);
                //  ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(_loadScenario));
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
        }

        private void txtIteration_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtIteration.Text == "0")
                {
                    txtIteration.Text = "1";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
        }

        private void txtIncrementUser_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtIncrementUser.Text == "0")
                {
                    txtIncrementUser.Text = "1";
                }
                else if (rbtnDuraion.Checked && txtIncrementUser.Text != "")
                {
                    if (Convert.ToInt32(txtIncrementUser.Text) > Convert.ToInt32(txtMaxuser.Text))
                    {
                        txtIncrementUser.Text = (Convert.ToInt32(txtMaxuser.Text) - Convert.ToInt32(txtStatUserCount.Text)).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
        }

        private bool Validate()
        {
            if (rbtnDuraion.Checked == true)
            {
                if (int.Parse(txtStatUserCount.Text) > int.Parse(txtMaxuser.Text))
                {
                    MessageBox.Show("Please enter start user count less than or equal max user");
                    txtStatUserCount.Focus();
                    return false;
                }
                else if (ucDurationTime.Time.TotalMilliseconds == 0)
                {
                    MessageBox.Show("Please enter valid duration");
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }
        }

        public void Save()
        {
            //bool LicenceValid = false;
            //if (File.Exists("") == false)
            //{
            //    if (rbtnIteration.Checked == true)
            //    {
            //        if (int.Parse(txtStatUserCount.Text) > 10)
            //        {
            //            MessageBox.Show("Max 10 allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else
            //        {
            //            LicenceValid = true;
            //        }

            //    }
            //    else
            //    {
            //        if (int.Parse(txtStatUserCount.Text) > 10)
            //        {
            //            MessageBox.Show("Max 10 allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else if (int.Parse(txtMaxuser.Text) > 10)
            //        {
            //            MessageBox.Show("Max 10 allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else
            //        {
            //            LicenceValid = true;
            //        }

            //    }

            //}
            //else
            //{
            //    int maxUserCount = 10;

            //    if (rbtnIteration.Checked == true)
            //    {
            //        if (int.Parse(txtStatUserCount.Text) > maxUserCount)
            //        {
            //            MessageBox.Show("Max " +maxUserCount+" allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else
            //        {
            //            LicenceValid = true;
            //        }

            //    }
            //    else
            //    {
            //        if (int.Parse(txtStatUserCount.Text) > maxUserCount)
            //        {
            //            MessageBox.Show("Max " + maxUserCount + " allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else if (int.Parse(txtMaxuser.Text) > maxUserCount)
            //        {
            //            MessageBox.Show("Max " + maxUserCount + " allowed for trail version");
            //            txtStatUserCount.Focus();
            //        }
            //        else
            //        {
            //            LicenceValid = true;
            //        }

            //    }
            //}

            if (Validate() == true)
            {
                XmlNode node = vUScriptSetting;
                repositoryXml.Save();
                MessageBox.Show("Saved");
            }
        }

        public int GetMaxUser(XmlNode licence)
        {
            return 10;
        }

        private void txtStatUserCount_Validated(object sender, EventArgs e)
        {
            try
            {
                if (txtStatUserCount.Text == "0")
                {
                    txtStatUserCount.Text = "1";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
        }

        private void txtMaxuser_Validated(object sender, EventArgs e)
        {
            try
            {
                if (txtMaxuser.Text == "0")
                {
                    txtMaxuser.Text = "1";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
        }

        private void tbtnRampUpDuration_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDuraion.Checked == false)
            {
                ucDurationTime.Visible = lblTime.Visible = false;
                lblDurationIteration.Visible = txtIteration.Visible = true;
                lblDurationIteration.Location = new System.Drawing.Point(6, 60);
                txtIteration.Location = new System.Drawing.Point(130, 61);
            }
            else
            {
                ucDurationTime.Visible = lblTime.Visible = true;
                lblDurationIteration.Visible = txtIteration.Visible = false;
                lblDurationIteration.Location = new System.Drawing.Point(6, 60);
                txtIteration.Location = new System.Drawing.Point(130, 61);
            }
        }
        /// <summary>
        /// This is to validate the maximum number of connections 
        /// validate : MAX connections allowed 16 & MIN connections 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtParallelCon_Leave(object sender, EventArgs e)
        {
            if (txtParallelCon.Text.Trim().Length==0)
            {
                MessageBox.Show("Please specify max connections");
            }
            else if (int.Parse(txtParallelCon.Text) < 1)
            {
                MessageBox.Show("Max 0 connections not allowed, Please specify the valid number of connections");
                txtParallelCon.Focus();
            }
            else if (int.Parse(txtParallelCon.Text) > 16)
            {
                MessageBox.Show("Max 16 connections allowed");
                txtParallelCon.Focus();
            }
            
        }

        
        
        
    }
}
