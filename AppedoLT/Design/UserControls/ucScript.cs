using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    /// <summary>
    /// User control used to set Scripts detail.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class ucScript : UserControl
    {

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

        /// <summary>
        /// Used to created ucScript object.
        /// </summary>
        /// <param name="xmlNode">Script xml node</param>
        /// <param name="treeNode">Tree node from UI</param>
        /// <returns></returns>
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

                MessageBox.Show("Please save any changes done, before validating or running scripts.");

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
                if (Session.Login())
                {
                    try
                    {
                        ValidateVariableVersion();
                        TrasportData respose = null;
                        Dictionary<string, string> header = new Dictionary<string, string>();

                        header.Add("userid", Session.UserID);
                        Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                        UploadFile(server, new TrasportData("VARIABLES", GetVariableXmlWithContent(), header), "Variables");
                        respose = server.Receive();
                        server.Close();
                        header.Clear();
                        server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                        VuscriptXml vuscriptXML = (VuscriptXml)_treeNode.Tag;
                        vuscriptXML.Save();
                        XmlNode scriptNode = vuscriptXML.Doc.SelectSingleNode("//vuscript");
                        string id = scriptNode.Attributes["id"].Value;
                        string name = scriptNode.Attributes["name"].Value;
                        string zipFilePath = MakeScriptZip(id, name);
                        header.Add("userid", Session.UserID);
                        header.Add("scriptname", name);
                        UploadFile(server, new TrasportData("VUSCRIPT", header, zipFilePath), name);
                        respose = server.Receive();
                        server.Close();
                        File.Delete(zipFilePath);
                        MessageBox.Show(respose.DataStr);

                    }                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void UploadFile(Trasport server, TrasportData data, string name)
        {

            long totalByte = 0;
            long recivedByte = 0;
            bool Success = true;

            new Thread(() =>
            {
                try
                {
                    server.Send(data, ref totalByte, ref recivedByte, ref Success);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    Success = false;
                }
            }).Start();

            while (((totalByte == 0 && recivedByte == 0) || recivedByte < totalByte))
            {
                if (totalByte > 0)
                {

                    frmDownloadProgress frm = new frmDownloadProgress(false);
                    frm.Text = "Uploading[" + name + "]...";
                    new Thread(() =>
                    {
                        frm.UpdateStatus(ref totalByte, ref recivedByte, ref Success);
                    }).Start();
                    frm.ShowDialog();
                }
                if (Success == false)

                    break;
                Thread.Sleep(1000);
            }
            if (Success == false)
            {
                throw new Exception("Upload Failed");
            }
        }

        private string MakeScriptZip(string scriptid, string scriptName)
        {
            string _scriptResourcePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + scriptid;
            string zipPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + scriptName + ".rar";
            Constants.GetInstance().Zip(_scriptResourcePath, zipPath);
            return zipPath;
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

        private void btnUpload_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (Session.Login())
                {
                    try
                    {
                        TrasportData respose = null;
                        Dictionary<string, string> header = new Dictionary<string, string>();
                        Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                        VuscriptXml vuscriptXML = (VuscriptXml)_treeNode.Tag;
                        vuscriptXML.Save();
                        XmlNode scriptNode = vuscriptXML.Doc.SelectSingleNode("//vuscript");
                        string id = scriptNode.Attributes["id"].Value;
                        string name = scriptNode.Attributes["name"].Value;
                        header.Add("userid", Session.UserID);
                        header.Add("scriptname", name);
                        string zipFilePath = MakeScriptZip(id, name);
                        UploadFile(server, new TrasportData("VUSCRIPT", header, zipFilePath), name);
                        respose = server.Receive();
                        server.Close();
                        File.Delete(zipFilePath);
                        MessageBox.Show(respose.DataStr);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
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
