using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class frmVariableManager : Telerik.WinControls.UI.RadForm
    {
        ErrorProvider _errMin = new ErrorProvider();
        ErrorProvider _errMax = new ErrorProvider();
        VariableXml _variableXml = VariableXml.GetInstance();
        XmlNode _variables = null;
        string fileCopyException = string.Empty;

        public frmVariableManager()
        {
            InitializeComponent();
            LoadTree();
        
         // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;
           toolTip.SetToolTip(txtVariableName, 
@"Rules For Constructing Variable Name

1. Characters Allowed :   
     Underscore(_)
     Capital Letters ( A – Z )
     Small Letters ( a – z )
     Digits ( 0 – 9 )

2. Blanks & Commas are not allowed.

3. No Special Symbols other than underscore(_) are allowed.

4.First Character should be alphabet or Underscore.");
        }

        private void LoadTree()
        {
            tvVariables.Nodes.Clear();
            _variables = _variableXml.doc.SelectSingleNode("//variables");
            pnlVariableValue.Location = ucFileTypeVariable.Location = pnlMinMax.Location = pnlNumber.Location = pnlCurrentDate.Location;

            foreach (XmlNode var in _variables.SelectNodes("//variable[@from='variablemanager']"))
            {
                tvVariables.Nodes.Add(GetTreeNode(var));
            }
            ddlVariableType.SelectedIndex = 0;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (tvVariables.SelectedNode != null)
            {
                Populate((XmlNode)tvVariables.SelectedNode.Tag);
                MakeReadOnly();
                if (MessageBox.Show("Are You sure You want to delete selected variable", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    XmlNode deleteNode = (XmlNode)tvVariables.SelectedNode.Tag;
                    try
                    {
                        if (deleteNode.Attributes["type"].Value == "file")
                        {
                            if (deleteNode.Attributes["vituallocation"] != null && File.Exists(Constants.GetInstance().ExecutingAssemblyLocation + deleteNode.Attributes["vituallocation"].Value))
                            {

                                File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + deleteNode.Attributes["vituallocation"].Value);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    _variableXml.doc.SelectSingleNode("//variables").RemoveChild(deleteNode);
                    tvVariables.SelectedNode.Remove();
                    ClearAll();
                    btnView_Click(null, null);
                }
            }
        }

        public XmlNode GetVariableInfo(XmlNode var)
        {
            XmlNode variable;
            if (var == null)
            {
                variable = _variableXml.doc.CreateElement("variable");
            }
            else
            {
                if (var.Attributes["type"].Value == "file")
                {
                    if (var.Attributes["vituallocation"] != null && File.Exists(Constants.GetInstance().ExecutingAssemblyLocation + var.Attributes["vituallocation"].Value))
                    {
                        File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + var.Attributes["vituallocation"].Value);
                    }
                }
                var.Attributes.RemoveAll();
                variable = var;
            }
            variable.Attributes.Append(_variableXml.GetAttribute("from", "variablemanager"));
            variable.Attributes.Append(_variableXml.GetAttribute("modified", DateTime.Now.Ticks.ToString()));
            if (rbtnGlobal.Checked == true) variable.Attributes.Append(_variableXml.GetAttribute("policy", "global"));
            else if (rbtnEachUser.Checked == true) variable.Attributes.Append(_variableXml.GetAttribute("policy", "eachuser"));
            else if (rbtnEachIteration.Checked == true) variable.Attributes.Append(_variableXml.GetAttribute("policy", "eachiteration"));

            switch (ddlVariableType.Text)
            {
                case "File":
                    string vituallocation = CopyFileToVariableFolder(ucFileTypeVariable.txtFileName.Text, variable.Attributes["modified"].Value);
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "file"));
                    variable.Attributes.Append(_variableXml.GetAttribute("location", ucFileTypeVariable.txtFileName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("vituallocation", vituallocation));
                    variable.Attributes.Append(_variableXml.GetAttribute("start", ucFileTypeVariable.txtStartFrom.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("delimiter", ucFileTypeVariable.txtDelimiter.Text));
                    string columns = string.Empty;
                    if (ucFileTypeVariable.dgvData.ColumnCount > 0)
                    {
                        foreach (DataGridViewColumn col in ucFileTypeVariable.dgvData.Columns)
                        {
                            columns += col.Name + ";";
                        }
                        columns = columns.Remove(columns.Length - 1);
                    }
                    variable.Attributes.Append(_variableXml.GetAttribute("columns", columns));
                    break;

                case "String":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "string"));
                    variable.Attributes.Append(_variableXml.GetAttribute("value", txtVariableValue.Text));
                    break;

                case "Number":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "number"));
                    variable.Attributes.Append(_variableXml.GetAttribute("value", txtNumber.Text));
                    break;

                case "Current Date":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "currentdate"));
                    variable.Attributes.Append(_variableXml.GetAttribute("index", ddlDateFormat.SelectedIndex.ToString()));
                    variable.Attributes.Append(_variableXml.GetAttribute("value", txtNumber.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("dateformat", ddlDateFormat.Text.ToString()));
                    break;

                case "Random String":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "randomstring"));
                    variable.Attributes.Append(_variableXml.GetAttribute("start", txtMin.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("end", txtMax.Text));
                    break;

                case "Random Number":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "randomnumber"));
                    variable.Attributes.Append(_variableXml.GetAttribute("start", txtMin.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("end", txtMax.Text));
                    break;
            }
            return variable;
        }

        public string CopyFileToVariableFolder(string source, string modified)
        {
            string destination = string.Empty;
            fileCopyException = string.Empty;
            try
            {
                if (File.Exists(source))
                {
                    FileInfo fileinfo = new FileInfo(source);
                    destination = Constants.GetInstance().ExecutingAssemblyLocation + "\\Variables\\" + modified + "_" + fileinfo.Name;
                    File.Copy(source, destination, true);
                    destination = "\\Variables\\" + modified + "_" + fileinfo.Name;
                }
            }
            catch (Exception ex)
            {
                fileCopyException = ex.Message;
            }
            return destination;
        }

        private void txtVariableName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) == true || char.IsLetterOrDigit(e.KeyChar) == true || e.KeyChar == '_') || (txtVariableName.Text.Length==0 && char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void ddlVariableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlMinMax.Visible = ucFileTypeVariable.Visible = pnlVariableValue.Visible = pnlCurrentDate.Visible = pnlNumber.Visible = false;
            if (ddlVariableType.Text == "File")
            {
                ucFileTypeVariable.Visible = true;
            }
            else if (ddlVariableType.Text == "String")
            {
                pnlVariableValue.Visible = true;
            }
            else if (ddlVariableType.Text == "Number")
            {
                pnlNumber.Visible = true;
            }
            else if (ddlVariableType.Text == "Random Number")
            {
                _errMin.Clear();
                _errMax.Clear();
                lblMin.Text = "Min Number";
                lblMax.Text = "Max Number";
                txtMin.Text = String.Empty;
                txtMax.Text = String.Empty;
                pnlMinMax.Visible = true;
            }
            else if (ddlVariableType.Text == "Random String")
            {
                _errMin.Clear();
                _errMax.Clear();
                lblMin.Text = "Min Length";
                lblMax.Text = "Max Length";
                txtMin.Text = String.Empty;
                txtMax.Text = String.Empty;
                pnlMinMax.Visible = true;
            }
            else if (ddlVariableType.Text == "Current Date")
            {
                pnlCurrentDate.Visible = true;
            }
        }

        private void MakeReadOnly()
        {
            txtVariableName.ReadOnly = true;
            txtVariableValue.ReadOnly = true;
            ucFileTypeVariable.txtFileName.ReadOnly = true;
            ucFileTypeVariable.txtStartFrom.ReadOnly = true;
            ucFileTypeVariable.txtDelimiter.ReadOnly = true;
            ddlVariableType.Enabled = false;
            rbtnGlobal.Enabled = rbtnEachUser.Enabled = rbtnEachIteration.Enabled = false;
            txtMin.ReadOnly = true;
            txtMax.ReadOnly = true;
            txtCurrentDate.ReadOnly = true;
            ddlDateFormat.ReadOnly = true;
            txtNumber.ReadOnly = true;
        }

        private void ReleaseReadOnly()
        {
            txtVariableName.ReadOnly = false;
            txtVariableValue.ReadOnly = false;
            ucFileTypeVariable.txtFileName.ReadOnly = false;
            ucFileTypeVariable.txtStartFrom.ReadOnly = false;
            ucFileTypeVariable.txtDelimiter.ReadOnly = false;
            ddlVariableType.Enabled = true;
            rbtnGlobal.Enabled = rbtnEachUser.Enabled = rbtnEachIteration.Enabled = true;
            txtMin.ReadOnly = false;
            txtMax.ReadOnly = false;
            txtCurrentDate.ReadOnly = false;
            ddlDateFormat.ReadOnly = false;
            txtNumber.ReadOnly = false;
        }

        private void ClearAll()
        {
            txtVariableName.Text = string.Empty;
            txtVariableValue.Text = string.Empty;
            txtMin.Text = String.Empty;
            txtMax.Text = String.Empty;
            txtCurrentDate.Text = string.Empty;
            ddlDateFormat.Text = string.Empty;
            rbtnEachIteration.Checked = true;
            ucFileTypeVariable.ClearAll();
            txtNumber.Text = String.Empty;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (tvVariables.SelectedNode != null)
            {
                Populate((XmlNode)tvVariables.SelectedNode.Tag);
                MakeReadOnly();
                btnSave.Text = "&Add New";
                btnModify.Text = "&Modify";
                _errMin.Clear();
                _errMax.Clear();
            }
        }

        private void Populate(XmlNode var)
        {
            try
            {
                ClearAll();
                btnSave.Text = "&Add New"; ;
                if (var.Attributes["policy"].Value == "global") rbtnGlobal.Checked = true;
                else if (var.Attributes["policy"].Value == "eachuser") rbtnEachUser.Checked = true;
                else if (var.Attributes["policy"].Value == "eachiteration") rbtnEachIteration.Checked = true;

                txtVariableName.Text = var.Attributes["name"].Value;
                txtVariableName.Tag = var;

                switch (var.Attributes["type"].Value)
                {
                    case "file":
                        ddlVariableType.SelectedIndex = 0;
                        ucFileTypeVariable.txtFileName.Text = var.Attributes["location"].Value;
                        ucFileTypeVariable.txtStartFrom.Text = var.Attributes["start"].Value;
                        ucFileTypeVariable.txtDelimiter.Text = var.Attributes["delimiter"].Value;
                        ucFileTypeVariable.btnViewData_Click(null, null);
                        break;

                    case "string":
                        ddlVariableType.SelectedIndex = 1;
                        txtVariableValue.Text = var.Attributes["value"].Value; ;
                        break;

                    case "number":
                        ddlVariableType.SelectedIndex = 2;
                        txtNumber.Text = var.Attributes["value"].Value;
                        break;

                    case "currentdate":
                        ddlVariableType.SelectedIndex = 5;
                        ddlDateFormat.SelectedIndex = Convert.ToInt16(var.Attributes["index"].Value);
                        txtCurrentDate.Text = var.Attributes["value"].Value;
                        break;

                    case "randomnumber":
                        ddlVariableType.SelectedIndex = 3;
                        txtMin.Text = var.Attributes["start"].Value;
                        txtMax.Text = var.Attributes["end"].Value; ;
                        break;

                    case "randomstring":
                        ddlVariableType.SelectedIndex = 4;
                        txtMin.Text = var.Attributes["start"].Value;
                        txtMax.Text = var.Attributes["end"].Value; ;
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private RadTreeNode GetTreeNode(XmlNode var)
        {
            RadTreeNode item = new RadTreeNode();
            item.Text = var.Attributes["name"].Value;
            item.Tag = var;
            item.ImageIndex = 0;
            return item;
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (tvVariables.SelectedNode != null)
            {
                if (btnModify.Text == "&Modify")
                {
                    Populate((XmlNode)tvVariables.SelectedNode.Tag);
                    btnModify.Text = "&Update";
                    ReleaseReadOnly();
                }
                else
                {
                    if (ddlVariableType.Text == "File")
                    {
                        if (ucFileTypeVariable.txtDelimiter.Text.Trim().Length > 1)
                        {
                            MessageBox.Show("Please enter valid delimiter");
                            return;
                        }
                    }
                    XmlNode var = GetVariableInfo((XmlNode)txtVariableName.Tag);
                    txtVariableName.Text = var.Attributes["name"].Value;
                    btnModify.Text = "&Modify";
                    MakeReadOnly();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "&Add")
            {
                if (txtVariableName.Text != string.Empty)
                {
                    if (ddlVariableType.Text == "File" || ddlVariableType.Text == "String" || ddlVariableType.Text == "Number" || ddlVariableType.Text == "Current Date")
                    {
                        XmlNode newVar = GetVariableInfo(null);
                        if (ddlVariableType.Text == "File")
                        {
                            if (newVar.Attributes["delimiter"].Value.Trim().Length > 1)
                            {
                                MessageBox.Show("Please enter valid delimiter");
                                return;
                            }
                            if (fileCopyException != string.Empty)
                            {
                                MessageBox.Show("Unable to copy variable file." + Environment.NewLine + fileCopyException);
                                return;
                            }
                        }
                        _variableXml.doc.SelectSingleNode("//variables").AppendChild(newVar);

                        tvVariables.Nodes.Add(GetTreeNode(newVar));
                        ClearAll();
                    }
                    else if (ddlVariableType.Text == "Random Number")
                    {
                        if (txtMin.Text != String.Empty && txtMax.Text != String.Empty)
                        {
                            XmlNode newVar = GetVariableInfo(null);
                            _errMin.Clear();
                            _errMax.Clear();
                            _variableXml.doc.SelectSingleNode("//variables").AppendChild(newVar);
                            tvVariables.Nodes.Add(GetTreeNode(newVar));
                            ClearAll();
                        }
                        else
                        {
                            if (txtMin.Text == String.Empty)
                                _errMin.SetError(txtMin, "Please fill the required field");
                            if (txtMax.Text == String.Empty)
                                _errMax.SetError(txtMax, "Please fill the required field");
                        }
                    }
                    else if (ddlVariableType.Text == "Random String")
                    {
                        if (txtMin.Text != String.Empty && txtMax.Text != String.Empty)
                        {
                            XmlNode newVar = GetVariableInfo(null);
                            _errMin.Clear();
                            _errMax.Clear();
                            _variableXml.doc.SelectSingleNode("//variables").AppendChild(newVar);
                            tvVariables.Nodes.Add(GetTreeNode(newVar));
                            ClearAll();
                        }
                        else
                        {
                            if (txtMin.Text == String.Empty)
                                _errMin.SetError(txtMin, "Please fill the required field");
                            if (txtMax.Text == String.Empty)
                                _errMax.SetError(txtMax, "Please fill the required field");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter variable name");
                }
            }
            else
            {
                ClearAll();
                btnModify.Text = "&Modify";
                btnSave.Text = "&Add";
                ReleaseReadOnly();
                txtVariableName.Focus();
            }

        }

        private void ddlDateFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurrentDate.Text = DateTime.Now.ToString(ddlDateFormat.Text);
        }

        private void txtMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void txtMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar)) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void tvVariables_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            btnView_Click(null, null);
        }

        private void btnUploadVariables_Click(object sender, EventArgs e)
        {
            try
            {

                if (Constants.GetInstance().UserId == string.Empty)
                {
                    frmLogin login = new frmLogin();
                    if (login.ShowDialog() == DialogResult.OK && login.Userid != string.Empty)
                    {
                        Constants.GetInstance().UserId = login.Userid;
                    }
                    else
                    {
                        return;
                    }
                }
                if (Constants.GetInstance().UserId != string.Empty)
                {
                    ValidateVariableVersion();

                    TrasportData respose = null;
                    Dictionary<string, string> header = new Dictionary<string, string>();

                    header.Add("userid", Constants.GetInstance().UserId);
                    Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                    UploadFile(server, new TrasportData("VARIABLES", GetVariableXmlWithContent(), header));
                   
                    respose = server.Receive();
                    server.Close();
                    header.Clear();

                    MessageBox.Show("Uploaded succesfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to upload");
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
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

        private void frmVariableManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            _variableXml.Save();
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

        private List<string> VarList(XmlDocument doc)
        {
            List<string> available = new List<string>();
            foreach (XmlNode var in doc.SelectNodes("//variable"))
            {
                available.Add(var.Attributes["name"].Value);
            }
            return available;
        }

        private List<string> VarList(string receivedvar)
        {
            List<string> available = new List<string>();
            foreach (string var in receivedvar.Split(','))
            {
                available.Add(var);
            }
            return available;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {

                if (Constants.GetInstance().UserId == string.Empty)
                {
                    frmLogin login = new frmLogin();
                    if (login.ShowDialog() == DialogResult.OK && login.Userid != string.Empty)
                    {
                        Constants.GetInstance().UserId = login.Userid;
                    }
                    else
                    {
                        return;
                    }
                }
                if (Constants.GetInstance().UserId != string.Empty)
                {
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                    folderBrowserDialog.Description = "Please select folder to store data file(s)";
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        string tempFilePath = System.IO.Path.GetTempPath() + DateTime.Now.Ticks.ToString() + ".xml";

                        TrasportData respose = null;
                        Dictionary<string, string> header = new Dictionary<string, string>();

                        header.Add("userid", Constants.GetInstance().UserId);
                        Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                        server.Send(new TrasportData("AVAILABLEVARIABLES", string.Empty, header));
                        respose = server.Receive();
                        server.Close();
                        List<string> availbleScript = VarList(_variableXml.doc);
                        List<string> receivedScript = VarList(respose.Header["variables"]);
                        List<string> conflictName = GetConflictName(availbleScript, receivedScript);
                        if (conflictName.Count > 0)
                        {
                            StringBuilder msg = new StringBuilder();
                            msg.AppendLine("Following varialble(s) are available on your local machine.");
                            foreach (string name in conflictName)
                            {
                                msg.AppendLine(name);
                            }
                            msg.AppendLine().AppendLine("Are you sure you want to replace?");
                            if (MessageBox.Show(msg.ToString(), "Warning", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            {
                                return;
                            }
                        }

                        server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                        server.Send(new TrasportData("GETVARIABLES", string.Empty, header));
                        respose= DownloadFile(server, tempFilePath);
                        server.Close();
                        header.Clear();
                        XmlDocument doc = new XmlDocument();
                        doc.Load(tempFilePath);
                        Marge(_variableXml.doc, doc, folderBrowserDialog.SelectedPath);
                        _variableXml.Save();
                        LoadTree();
                        MessageBox.Show("Downloaded succesfully.");
                        if (File.Exists(tempFilePath))
                        {
                            try
                            {
                                File.Delete(tempFilePath);
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Download.Please try again");
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private TrasportData DownloadFile(Trasport server, string filePath)
        {
            TrasportData respose = null;
            long totalByte = 0;
            long recivedByte = 0;
            bool Success = true;

            new Thread(() =>
            {
                try
                {
                    respose = server.Receive(filePath, ref totalByte, ref recivedByte, ref Success);
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

                    frmDownloadProgress frm = new frmDownloadProgress(true);
                    new Thread(() =>
                    {
                        frm.UpdateStatus(ref totalByte, ref recivedByte, ref Success);
                    }).Start();
                    frm.ShowDialog();
                }
                if (Success == false) break;
                Thread.Sleep(1000);
            }
            if(Success==false)
            {
                throw new Exception("Download Faild");
            }
            return respose;
        }

        private void UploadFile(Trasport server, TrasportData data)
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
                    frm.Text = "Uploading...";
                    new Thread(() =>
                    {
                        frm.UpdateStatus(ref totalByte, ref recivedByte, ref Success);
                    }).Start();
                    frm.ShowDialog();
                }
                if (Success == false) break;
                Thread.Sleep(1000);
            }
            if (Success == false)
            {
                throw new Exception("Upload Faild");
            }
        }

        private void Marge(XmlDocument available, XmlDocument received, string fileLocationPath)
        {
            foreach (XmlNode receivedVar in received.SelectNodes("//variable"))
            {
                XmlNode existingVar = available.SelectSingleNode("//variable[@name='" + receivedVar.Attributes["name"].Value + "']");
                if (existingVar != null)
                {
                    available.SelectSingleNode("//variables").RemoveChild(existingVar);
                }
                XmlNode newNode = available.ImportNode(receivedVar, true);
                if (newNode.SelectSingleNode(".//content") != null)
                {
                    StroreVarialeFile(newNode, fileLocationPath);
                }
                available.SelectSingleNode("//variables").AppendChild(newNode);
            }
        }

        private List<string> GetConflictName(List<string> available, List<string> received)
        {
            List<string> result = new List<string>();
            foreach (string receivedName in received)
            {
                if (available.Contains(receivedName)) result.Add(receivedName);
            }
            return result;
        }

        private void StroreVarialeFile(XmlNode var, string selectedPath)
        {
            string[] filename = var.Attributes["location"].Value.Split('\\');
            string location = selectedPath + "\\" + filename[filename.Length - 1];
            string virtualLoacationFilePath = Constants.GetInstance().ExecutingAssemblyLocation + var.Attributes["vituallocation"].Value;

            if (File.Exists(virtualLoacationFilePath)) File.Delete(virtualLoacationFilePath);
            if (File.Exists(location)) File.Delete(location);

            using (StreamWriter steam = new StreamWriter(new FileStream(location, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                steam.Write(var.InnerText);
            }
            using (StreamWriter steam = new StreamWriter(new FileStream(virtualLoacationFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                steam.Write(var.InnerText);
            }
            var.Attributes["modified"].Value = DateTime.Now.Ticks.ToString();
            var.Attributes["location"].Value = location;

        }
    }
}
