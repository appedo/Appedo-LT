using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
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
                    catch(Exception ex)
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
                if (var.Attributes["type"].Value=="file")
                {
                    if(var.Attributes["vituallocation"]!=null&&File.Exists(Constants.GetInstance().ExecutingAssemblyLocation+var.Attributes["vituallocation"].Value))
                    {
                        File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + var.Attributes["vituallocation"].Value);
                    }
                }
                var.Attributes.RemoveAll();
                variable = var;
            }
            variable.Attributes.Append(_variableXml.GetAttribute("from", "variablemanager"));
            variable.Attributes.Append(_variableXml.GetAttribute("modified", DateTime.Now.Ticks.ToString()));
            if (rbtnGlobal.Checked == true)  variable.Attributes.Append(_variableXml.GetAttribute("policy","global"));
            else if (rbtnEachUser.Checked == true)  variable.Attributes.Append(_variableXml.GetAttribute("policy","eachuser"));
            else if (rbtnEachIteration.Checked == true)  variable.Attributes.Append(_variableXml.GetAttribute("policy","eachiteration"));

            switch (ddlVariableType.Text)
            {
                case "File":
                    string vituallocation = CopyFileToVariableFolder(ucFileTypeVariable.txtFileName.Text, variable.Attributes["modified"].Value);
                    variable.Attributes.Append(_variableXml.GetAttribute("name",txtVariableName.Text));
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
                    variable.Attributes.Append(_variableXml.GetAttribute("columns",columns));
                    break;

                case "String":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type","string"));
                    variable.Attributes.Append(_variableXml.GetAttribute("value", txtVariableValue.Text));
                    break;

                case "Number":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "number"));
                    variable.Attributes.Append(_variableXml.GetAttribute("value",txtNumber.Text));
                    break;

                case "Current Date":
                    variable.Attributes.Append(_variableXml.GetAttribute("name", txtVariableName.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("type", "currentdate"));
                    variable.Attributes.Append(_variableXml.GetAttribute("index", ddlDateFormat.SelectedIndex.ToString()));
                    variable.Attributes.Append(_variableXml.GetAttribute("value", txtNumber.Text));
                    variable.Attributes.Append(_variableXml.GetAttribute("dateformat",ddlDateFormat.Text.ToString()));
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

        public string CopyFileToVariableFolder(string source,string modified)
        {
            string destination = string.Empty;
            fileCopyException = string.Empty;
            try
            {
                if (File.Exists(source))
                {
                    FileInfo fileinfo = new FileInfo(source);
                    destination = Constants.GetInstance().ExecutingAssemblyLocation + "\\Variables\\" +modified+"_"+fileinfo.Name;
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
            if (!(char.IsControl(e.KeyChar) == true || char.IsLetterOrDigit(e.KeyChar) == true || e.KeyChar == '_'))
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
                    server.Send(new TrasportData("VARIABLES", GetVariableXmlWithContent(), header));
                    respose = server.Receive();
                    server.Close();
                    header.Clear();

                    MessageBox.Show("Uploaded succesfully.");
                }
            }
            catch(Exception ex)
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
                   // ValidateVariableVersion();

                    TrasportData respose = null;
                    Dictionary<string, string> header = new Dictionary<string, string>();

                    header.Add("userid", Constants.GetInstance().UserId);
                    Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                    server.Send(new TrasportData("GETVARIABLES", string.Empty, header));
                    respose = server.Receive();
                    server.Close();
                    header.Clear();

                    MessageBox.Show("Downloaded succesfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to upload");
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
