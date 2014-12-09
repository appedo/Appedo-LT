using System;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System.Net;
using System.Text;
using System.IO;

using System.Collections.Generic;

namespace AppedoLT
{
    public partial class ucScript : UserControl
    {
        public static string UserId = string.Empty;

        RadTreeNode _treeNode = null;
        private static ucScript _instance;

        public static ucScript GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ucScript();
                _instance.Size = new System.Drawing.Size(476, 150);
            }
            return _instance;
        }

        private ucScript()
        {
            InitializeComponent();
        }

        public ucScript GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.Tag = xmlNode;
            this.txtScriptname.Text = xmlNode.Attributes["name"].Value;
            this.txtScriptname.Tag = xmlNode.Attributes["name"];
            if (xmlNode.Attributes["type"].Value == "http")
            {
                this.txtExclutionFileTypes.Visible = this.chkDynamicReqEnable.Visible = this.lblFileTypes.Visible = radLabel6.Visible = true;
            }
            else
            {
                this.txtExclutionFileTypes.Visible = this.chkDynamicReqEnable.Visible = this.lblFileTypes.Visible = radLabel6.Visible = false;
            }

            this.txtExclutionFileTypes.Text = xmlNode.Attributes["exclutionfiletypes"].Value;
            this.txtExclutionFileTypes.Tag = xmlNode.Attributes["exclutionfiletypes"];
            this.chkDynamicReqEnable.Checked = Convert.ToBoolean(xmlNode.Attributes["dynamicreqenable"].Value);
            this.chkDynamicReqEnable.Tag = xmlNode.Attributes["dynamicreqenable"];
            this.Dock = DockStyle.Fill;
            return this;
        }

        private void txt_Validated(object sender, EventArgs e)
        {
            try
            {
                RadTextBox txt = (RadTextBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
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

        private void chkDynamicReqEnable_Leave(object sender, EventArgs e)
        {
            try
            {
                RadCheckBox txt = (RadCheckBox)sender;
                XmlAttribute attr = (XmlAttribute)txt.Tag;
                if (txt.Checked != Convert.ToBoolean(attr.Value))
                {
                    attr.Value = txt.Checked.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Constants.GetInstance().IsValidationScreenOpen == true)
                {
                    MessageBox.Show("Please close existing validation screen");
                }
                else
                {
                    RunTimeException.GetInstance().storeErrors = false;
                    if (((XmlNode)this.Tag).Attributes["type"].Value == "tcp")
                    {
                        XmlNode selectNode = ((XmlNode)this.Tag);
                        frmTCPIPValidation frm = new frmTCPIPValidation(selectNode, (RadTreeNode)_treeNode.Clone(), selectNode.SelectNodes("//vuscript[@name='" + selectNode.Attributes["name"].Value + "']//request").Count);
                        try
                        {
                            frm.Show();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                    else
                    {
                        XmlNode selectNode = ((XmlNode)this.Tag);
                        frmValidation frm = new frmValidation(selectNode, (RadTreeNode)_treeNode.Clone(), selectNode.SelectNodes("//vuscript[@name='" + selectNode.Attributes["name"].Value + "']//request").Count);
                        try
                        {
                            frm.Show();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                    RunTimeException.GetInstance().storeErrors = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {

            try
            {
                if (ucScript.UserId == string.Empty)
                {
                    frmLogin login = new frmLogin();
                    if (login.ShowDialog() == DialogResult.OK && login.Userid != string.Empty)
                    {
                        ucScript.UserId = login.Userid;
                    }
                    else
                    {
                        return;
                    }
                }
                if (ucScript.UserId != string.Empty)
                {
                    ValidateVariableVersion();
                    TrasportData respose = null;
                    Dictionary<string, string> header = new Dictionary<string, string>();

                    header.Add("userid", ucScript.UserId);
                    Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                    server.Send(new TrasportData("VARIABLES", GetVariableXmlWithContent(), header));
                    respose = server.Receive();
                    server.Close();
                    header.Clear();

                    header.Add("userid", ucScript.UserId);
                     server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                    string vuscriptXML = ((XmlNode)_treeNode.Tag).OuterXml;
                    server.Send(new TrasportData("VUSCRIPT", vuscriptXML, header));
                    server.Receive();
                    server.Close();


                    MessageBox.Show("Uploaded succesfully.");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private string GetVariableXmlWithContent()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>" + VariableXml.GetInstance().doc.SelectSingleNode("//variables").OuterXml);
            string filepath = string.Empty;
            foreach (XmlNode var in doc.SelectNodes("//variable"))
            {
                if (var.Attributes["type"].Value == "file")
                {
                    filepath = Constants.GetInstance().ExecutingAssemblyLocation + var.Attributes["vituallocation"].Value;
                    if (File.Exists(filepath))
                    {
                        XmlNode content = doc.CreateElement("content");
                        try
                        {
                            StreamReader file = new StreamReader(new FileStream(filepath, FileMode.Open, FileAccess.Read));
                            content.InnerText = file.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        var.AppendChild(content);
                    }
                }
            }
            return doc.OuterXml;
        }

        private void ValidateVariableVersion()
        {
            foreach (XmlNode variable in VariableXml.GetInstance().doc.SelectNodes("//variables/variable[@type='file']"))
            {
                try
                {
                    if (File.Exists(variable.Attributes["location"].Value))
                    {
                        FileInfo source = new FileInfo(variable.Attributes["location"].Value);
                        if (source.LastWriteTime.Ticks != Convert.ToDouble(variable.Attributes["modified"].Value))
                        {
                            string ticks = source.LastWriteTime.Ticks.ToString();
                            File.Copy(variable.Attributes["location"].Value, Constants.GetInstance().ExecutingAssemblyLocation + "\\" + variable.Attributes["vituallocation"].Value, true);
                            variable.Attributes["modified"].Value = ticks;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
        }
    }
}
