using AppedoLT.BusinessLogic;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using System.Windows.Forms;

namespace AppedoLT
{
    /// <summary>
    /// Application starting form.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class Design : Telerik.WinControls.UI.RadForm
    {
        public static Telerik.WinControls.UI.RadMenuItem mnuiLogin;
        private List<string> _loadGeneratorips = new List<string>();
        private Dictionary<string, string> loadGenUserDetail = new Dictionary<string, string>();
        private Stopwatch runTime = new Stopwatch();
        private List<ScriptExecutor> _scriptExecutorList = new List<ScriptExecutor>();
        private RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        private Constants _constants = Constants.GetInstance();
        private Boolean _isUseLoadGen = false;
        private ucDesign _ucDesignObj = null;
        private ExecutionReport executionReport = ExecutionReport.GetInstance();
        private DataServer _dataServer = DataServer.GetInstance();
        private int _hitCount = 0;
        private BindingList<VUScriptStatus> _vususerStatus = new BindingList<VUScriptStatus>();

        public Design()
        {
            try
            {
                InitializeComponent();

                comboBrowserVersion.SelectedIndex = 0;
                mnuiLogin = new Telerik.WinControls.UI.RadMenuItem();
                radMenu1.Items.Add(mnuiLogin);
                mnuiLogin.Name = "mnuiLogin";
                mnuiLogin.Text = "&Login";
                mnuiLogin.Click += new System.EventHandler(this.mnuiLogin_Click);

                XmlNode vuscripts = RepositoryXml.GetInstance().Doc.SelectSingleNode("//vuscripts");
                if (vuscripts != null && vuscripts.ChildNodes.Count > 0)
                {
                    long totalByte = vuscripts.ChildNodes.Count;
                    long recivedByte = 0;
                    bool Success = true;

                    //Upgrade old scripts
                    new Thread(() =>
                    {
                        try
                        {
                            Upgrade(ref totalByte, ref recivedByte, ref Success);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                            Success = false;
                        }
                    }).Start();

                    //Show upgrade status
                    while (((totalByte == 0 && recivedByte == 0) || recivedByte < totalByte))
                    {
                        if (totalByte > 0)
                        {

                            frmDownloadProgress frm = new frmDownloadProgress("Converted / Total");
                            frm.Text = "Upgrading script...";
                            new Thread(() =>
                            {
                                frm.UpdateStatusForScript(ref totalByte, ref recivedByte, ref Success);
                            }).Start();
                            frm.ShowDialog();
                        }
                        if (Success == false) break;
                        Thread.Sleep(1000);
                    }

                }
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                //Create required folders
                if (!Directory.Exists(".\\Data")) Directory.CreateDirectory(".\\Data");
                if (!Directory.Exists(".\\DataMonitor")) Directory.CreateDirectory(".\\DataMonitor");
                if (!Directory.Exists(".\\Exported Charts")) Directory.CreateDirectory(".\\Exported Charts");
                if (!Directory.Exists(".\\Exported Reports")) Directory.CreateDirectory(".\\Exported Reports");
                if (!Directory.Exists(".\\MonitorData")) Directory.CreateDirectory(".\\MonitorData");
                if (!Directory.Exists(".\\Scripts")) Directory.CreateDirectory(".\\Scripts");
                if (!Directory.Exists(".\\Upload")) Directory.CreateDirectory(".\\Upload");
                if (!Directory.Exists(".\\Variables")) Directory.CreateDirectory(".\\Variables");
                _ucDesignObj = ucDesign.GetInstance();
                tabiVUscript.ContentPanel.Controls.Add(_ucDesignObj);
                //To allow cross thread access
                ListView.CheckForIllegalCrossThreadCalls = false;
                tabiVUscript.Select();
                LoadScenarioTree();
                LoadReportName(string.Empty);
                lblUserCompleted.Text = "0";
                lblUserCreated.Text = "0";
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }
        }

        #region Design

        //To form script tree view. 
        public void GetTreeNode(XmlNode ContainerNode, RadTreeNode parentNode)
        {
            foreach (XmlNode action in ContainerNode.ChildNodes)
            {
                switch (action.Name)
                {
                    case "request":
                        RadTreeNode childContainerNode = new RadTreeNode();
                        childContainerNode.Text = action.Attributes["name"].Value;
                        childContainerNode.Tag = action;
                        if (action.Attributes["HasErrorResponse"] != null && Convert.ToBoolean(action.Attributes["HasErrorResponse"].Value) == true)
                        {
                            childContainerNode.BackColor = Color.Red;
                        }
                        parentNode.Nodes.Add(childContainerNode);
                        break;
                    case "container":
                    case "if":
                    case "then":
                    case "else":
                    case "page":
                        RadTreeNode container = new RadTreeNode();
                        if (action.Name == "container" || action.Name == "page") container.Text = action.Attributes["name"].Value;
                        else if (action.Name == "if") container.Text = "If(Condition)";
                        else if (action.Name == "then") container.Text = "Then";
                        else if (action.Name == "else") container.Text = "else";
                        container.Tag = action;
                        GetTreeNode(action, container);
                        parentNode.Nodes.Add(container);
                        break;
                    case "loop":
                        RadTreeNode loop = new RadTreeNode();
                        loop.Text = "Loop";
                        loop.Tag = action;
                        GetTreeNode(action, loop);
                        parentNode.Nodes.Add(loop);
                        break;
                    case "whileloop":
                        RadTreeNode whileloop = new RadTreeNode();
                        whileloop.Text = "WhileLoop";
                        whileloop.Tag = action;
                        GetTreeNode(action, whileloop);
                        parentNode.Nodes.Add(whileloop);
                        break;
                    case "delay":
                        RadTreeNode delayNode = new RadTreeNode();
                        delayNode.Text = "Delay";
                        delayNode.Tag = action;
                        parentNode.Nodes.Add(delayNode);
                        break;
                    case "starttransaction":
                        RadTreeNode transactionNode = new RadTreeNode();
                        transactionNode.Text = "StartTransaction";
                        transactionNode.Tag = action;
                        parentNode.Nodes.Add(transactionNode);
                        break;
                    case "endtransaction":
                        RadTreeNode endTransactionNode = new RadTreeNode();
                        endTransactionNode.Text = "EndTransaction";
                        endTransactionNode.Tag = action;
                        parentNode.Nodes.Add(endTransactionNode);
                        break;
                    case "javascript":
                        RadTreeNode javascriptNode = new RadTreeNode();
                        javascriptNode.Text = "JavaScript";
                        javascriptNode.Tag = action;
                        parentNode.Nodes.Add(javascriptNode);
                        break;
                }
            }
        }

        //Load list of scenario in right side tree view in run screen
        public void LoadScenarioTree()
        {
            try
            {
                tvScenarios.Nodes.Clear();
                //Get all scenarios
                foreach (XmlNode scenario in _repositoryXml.Doc.SelectNodes("//scenarios//scenario"))
                {
                    if (scenario.Attributes.Count > 0)
                    {
                        
                        RadTreeNode scenarioNode = new RadTreeNode();
                        scenarioNode.Text = scenario.Attributes["name"].Value;
                        scenarioNode.Tag = scenario;
                        //Set scenario icon
                        scenarioNode.ImageKey = "scenarios.gif";
                        //Get all scripts
                        foreach (XmlNode script in scenario.SelectNodes("script"))
                        {
                            RadTreeNode scenarioScriptNode = new RadTreeNode();
                            scenarioScriptNode.Text = script.Attributes["name"].Value;
                            scenarioScriptNode.Tag = script;
                            scenarioScriptNode.ImageKey = "scripts.gif";
                            scenarioNode.Nodes.Add(scenarioScriptNode);
                        }
                        tvScenarios.Nodes.Add(scenarioNode);
                   }
                    
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }


        private static void DeleteXmlNode(string path, string tagname, string searchconditionAttributename, string searchconditionAttributevalue)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList nodes = doc.GetElementsByTagName(tagname);
            //XmlNodeList nodes = doc.GetElementsByTagName("user");
            foreach (XmlNode node in nodes)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if ((attribute.Name == searchconditionAttributename) && (attribute.Value == searchconditionAttributevalue))
                    //if ((attribute.Name == "name") && (attribute.Value == "aaa"))
                    {
                        //delete.
                        node.RemoveAll();
                        break;
                    }
                }
            }
            //save xml file.
            doc.Save(path);
        }  
        //To get list of report name
        public void LoadReportName(string repoerName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = Result.GetInstance().GetReportNameList(repoerName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        #region Events

        //Fire when user open application
        private void Design_Load(object sender, EventArgs e)
        {
            try
            {
                //If out.txt file not exist, we need to create to store report data.
                if (File.Exists(Constants.GetInstance().ExecutingAssemblyLocation + "\\out.txt") == false) File.Create(Constants.GetInstance().ExecutingAssemblyLocation + "\\out.txt");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Fire when user resize form
        private void tabsDesign_Resize(object sender, EventArgs e)
        {
            tabsDesign.ItemsOffset = (tabsDesign.Width / 3) + 5;
        }

        //Fire when user click Variable Manager option
        private void mnuiVariableManager_Click(object sender, EventArgs e)
        {
            frmVariableManager vm = new frmVariableManager();
            vm.ShowDialog();
        }

        //Fire when user click Record http option
        private void mnuiHttp_Click(object sender, EventArgs e)
        {
            try
            {
                frmVUScriptNameHttp frm = new frmVUScriptNameHttp();
                frm.ShowDialog();
                if (frm.vuscriptXml != null)
                {
                    XmlNode vuscriptNode = frm.vuscriptXml.Doc.SelectSingleNode("//vuscript");
                    frmRecord rd = new frmRecord(this, frm.name, vuscriptNode, frm.ddlParentContainer.SelectedIndex);
                    this.Visible = false;
                    rd.ShowDialog();
                    frm.vuscriptXml.Save();
                    RadTreeNode vuScriptNode = new RadTreeNode();
                    vuScriptNode.Text = vuscriptNode.Attributes["name"].Value;
                    vuScriptNode.Tag = frm.vuscriptXml;
                    //Change icon for script node
                    vuScriptNode.ImageKey = "scripts.gif";
                    //To form script tree view
                    foreach (XmlNode container in vuscriptNode.ChildNodes)
                    {
                        RadTreeNode containerNode = new RadTreeNode();
                        containerNode.Text = container.Attributes["name"].Value;
                        containerNode.Tag = container;
                        GetTreeNode(container, containerNode);
                        vuScriptNode.Nodes.Add(containerNode);
                    }
                    _ucDesignObj.tvRequest.Nodes.Add(vuScriptNode);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Fire when user click Record tcp ip option
        private void mnuiTcpip_Click(object sender, EventArgs e)
        {
            try
            {
                frmVUScriptName frm = new frmVUScriptName("tcp");
                frm.ShowDialog();
                if (frm.vuscriptXml != null)
                {
                    XmlNode vuscriptNode = frm.vuscriptXml.Doc.SelectSingleNode("//vuscript");
                    frmTCPIPRecord frmTcpRecord = new frmTCPIPRecord((Design)this.Parent.Parent.Parent, vuscriptNode);
                    this.Parent.Parent.Parent.Visible = false;
                    frmTcpRecord.ShowDialog();

                    RadTreeNode vuScriptNode = new RadTreeNode();
                    vuScriptNode.Text = vuscriptNode.Attributes["name"].Value;
                    vuScriptNode.Tag = frm.vuscriptXml;
                    //Change icon for script node
                    vuScriptNode.ImageKey = "scripts.gif";

                    //To form script tree view
                    foreach (XmlNode container in vuscriptNode.ChildNodes)
                    {
                        RadTreeNode containerNode = new RadTreeNode();
                        containerNode.Text = container.Attributes["name"].Value;
                        containerNode.Tag = container;
                        GetTreeNode(container, containerNode);
                        vuScriptNode.Nodes.Add(containerNode);
                    }
                    _ucDesignObj.tvRequest.Nodes.Add(vuScriptNode);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Fire when user click NewScenario
        private void mnuiNewScenario_Click(object sender, EventArgs e)
        {
            tabiRun.Select();
            btnAddNewScenario_Click(null, null);
        }

        //Fire when user click run menu
        private void mnuiRun_Click(object sender, EventArgs e)
        {
            tabiRun.Select();
            btnRun_Click(null, null);
        }

        //Fire when user click Stop menu
        private void mnuiStop_Click(object sender, EventArgs e)
        {
            tabiRun.Select();
            btnStop_Click(null, null);
        }

        //Fire when user click about menu
        private void radMenuItem5_Click(object sender, EventArgs e)
        {
            new About_FloodGates().ShowDialog();
        }

        //Fire when user click run option
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnRun_Click(null, null);
        }

        //user click login option
        private void mnuiLogin_Click(object sender, EventArgs e)
        {
            if (mnuiLogin.Text == "&Login")
            {
                if (Session.Login())
                {
                    mnuiLogin.Text = "&Logout";
                }
            }
            else
            {
                Session.LogOut();
                mnuiLogin.Text = "&Login";
            }
        }

        //Old logic. To upgrade vuscripts.
        void Upgrade(ref long totalByte, ref long recivedByte, ref bool Success)
        {
            XmlNode vuscripts = RepositoryXml.GetInstance().Doc.SelectSingleNode("//vuscripts");
            //  File.Copy(RepositoryXml.GetInstance().doc.)
            string sorceRequestFolderPath = ".\\Request\\";
            string sorceResponseFolderPath = ".\\Response\\";
            if (vuscripts != null && vuscripts.ChildNodes.Count > 0)
            {
                foreach (XmlNode script in vuscripts.ChildNodes)
                {
                    try
                    {
                        string scriptid = script.Attributes["id"].Value;

                        string desFolderPath = ".\\Scripts\\" + scriptid + "\\";
                        if (Directory.Exists(desFolderPath)) Directory.Delete(desFolderPath, true);
                        VuscriptXml vuscriptxml = new VuscriptXml(scriptid, script.OuterXml);
                        vuscriptxml.Save();

                        foreach (XmlNode request in vuscriptxml.Doc.SelectNodes("//request"))
                        {
                            if (File.Exists(sorceRequestFolderPath + request.Attributes["reqFilename"].Value)) File.Copy(sorceRequestFolderPath + request.Attributes["reqFilename"].Value, desFolderPath + request.Attributes["reqFilename"].Value);
                            if (File.Exists(sorceResponseFolderPath + request.Attributes["resFilename"].Value)) File.Copy(sorceResponseFolderPath + request.Attributes["resFilename"].Value, desFolderPath + request.Attributes["resFilename"].Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    recivedByte += 1;
                }
            }
            RepositoryXml.GetInstance().Doc.SelectSingleNode(".//root").RemoveChild(vuscripts);
            RepositoryXml.GetInstance().Save();
        }

        //During the form closing
        private void Design_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Do you want to save changes?", "Save", MessageBoxButtons.YesNoCancel);

                //Cancel form close
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    Session.LogOut();
                    _ucDesignObj.btnScriptSave_Click(null, null);
                }
                Process.GetCurrentProcess().Kill();
                AppedoLT.Core.Constants.GetInstance().ReSetFirefoxProxy();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        #endregion

        #endregion

        #region Run

        // validate license info. Restrict user count
        private bool ValidateLicence(XmlNode scenario)
        {
            int userCount = 0;

            //Getting total user count given by user
            foreach (XmlNode script in scenario.ChildNodes)
            {
                userCount += int.Parse(script.ChildNodes[0].Attributes["maxuser"].Value);
            }
            //Check Maxuser count
            if (userCount > _constants.MaxUserCount)
            {
                //Login to get user count for given user
                if(Session.Login())
                {
                    //If license valid
                    if (Session.IsLicenseValid == true)
                    {
                        //User given user count is greater than max user count in license
                        if (userCount > Session.UserCount)
                        {
                            MessageBox.Show("Current license only allow max " + Session.UserCount + " users.");
                            return false;
                        }
                        else
                        {
                            //Lock license to avoid multiple machine use same license
                            if (Session.MachineUniqueID == string.Empty && Session.RegisterMachine() == false)
                            {
                                MessageBox.Show("Unable to register machine. Please contact Appedo support.");
                                return false;
                            }
                            else
                            {
                                //Allow user to run
                                if (Session.MachineUniqueID == _constants.MachineUniqueID)
                                {
                                    return true;
                                }
                                else
                                {
                                    MessageBox.Show("AppedoLT already registered in another device. Please contact Appedo support.");
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("License has expired. Please contact Appedo support.");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Current license only allow max " + _constants.MaxUserCount + " users.");
                    return false;
                }
               
            }
            else
            {
                return true;
            }
        }

        //Create scenario for run. It is temp scenario xml for run.
        public XmlDocument GetScenarioForRun(string scenarioid, string reportName, int totalLoadGen, int currentLoadGenid, bool enableipspoofing)
        {
            XmlDocument scenarioDoc = new XmlDocument();
            try
            {

                scenarioDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><scenario id='" + scenarioid + "' reportname='" + reportName + "' enableipspoofing='" + enableipspoofing + "'></scenario></root>");

                XmlNode root = scenarioDoc.SelectSingleNode("//root/scenario");
                XmlNode setting = scenarioDoc.SelectSingleNode("//root/setting");
                XmlNode scenario = _repositoryXml.Doc.SelectSingleNode("//scenario[@id='" + scenarioid + "']").Clone();

                //Getting script one by one
                foreach (XmlNode script in scenario.ChildNodes)
                {
                    VuscriptXml vuscript = new VuscriptXml(script.Attributes["id"].Value);
                    XmlNode scriptNode = vuscript.Doc.SelectSingleNode("//vuscript");
                    if (scriptNode.HasChildNodes == true)
                    {
                        XmlNode tempScript = scriptNode.Clone();
                        tempScript = _repositoryXml.Doc.ImportNode(tempScript, true);
                        script.AppendChild(tempScript);
                        XmlNode importNode = scenarioDoc.ImportNode(script, true);
                        XmlAttribute attTotalMachine = scenarioDoc.CreateAttribute("totalloadgen");
                        XmlAttribute attCurrentMacineid = scenarioDoc.CreateAttribute("currentloadgenid");
                        attTotalMachine.Value = totalLoadGen.ToString();
                        attCurrentMacineid.Value = currentLoadGenid.ToString();
                        importNode.ChildNodes[0].Attributes.Append(attTotalMachine);
                        importNode.ChildNodes[0].Attributes.Append(attCurrentMacineid);
                        root.AppendChild(importNode);
                    }
                }
                root = scenarioDoc.SelectSingleNode("//root");
                //scenarioDoc.Save(reportName + ".xml");

            }
            catch (Exception ex)
            {

            }
            //fo doc.SelectSingleNode("\\scenario[name='scenarioName']").ChildNodes;
            return scenarioDoc;
        }

        //Export DataGridView data into html file
        private void Export_TO_Excel(DataGridView grdView, String grdName)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Exported Reports";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                int cols;
                string fileName = directoryPath + "\\" + grdName + "_" + DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss") + ".html";
                using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    StreamWriter wr = new StreamWriter(stream);
                    cols = grdView.Columns.Count;
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(wr))
                    {
                        hw.Write("<table border=1>");
                        hw.Write("<tr>");
                        for (int i = 0; i < cols; i++)
                        {
                            hw.Write("<td>");
                            hw.Write("<b>" + grdView.Columns[i].HeaderText.ToString().ToUpper() + "</b>");
                            hw.Write("</td>");
                        }
                        hw.Write("</tr>");
                        for (int i = 0; i < (grdView.Rows.Count); i++)
                        {
                            hw.Write("<tr>");
                            for (int j = 0; j < cols; j++)
                            {
                                hw.Write("<td>");
                                if (grdView.Rows[i].Cells[j].Value != null)
                                    hw.Write(grdView.Rows[i].Cells[j].Value);
                                hw.Write("</td>");
                            }
                            hw.Write("</tr>");
                        }
                        hw.Write("</table>");
                    }
                    wr.Close();
                }
                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //Export RadGridView data into html file
        private void Export_TO_Excel(RadGridView grdView, String grdName)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Exported Reports";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                int cols;
                string fileName = directoryPath + "\\" + grdName + "_" + DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss") + ".html";
                using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    StreamWriter wr = new StreamWriter(stream);
                    cols = grdView.Columns.Count;
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(wr))
                    {
                        hw.Write("<table border=1>");
                        hw.Write("<tr>");
                        for (int i = 0; i < cols; i++)
                        {
                            hw.Write("<td>");
                            hw.Write("<b>" + grdView.Columns[i].HeaderText.ToString().ToUpper() + "</b>");
                            hw.Write("</td>");
                        }
                        hw.Write("</tr>");
                        for (int i = 0; i < (grdView.Rows.Count); i++)
                        {
                            hw.Write("<tr>");
                            for (int j = 0; j < cols; j++)
                            {
                                hw.Write("<td>");
                                if (grdView.Rows[i].Cells[j].Value != null)
                                    hw.Write(grdView.Rows[i].Cells[j].Value);
                                hw.Write("</td>");
                            }
                            hw.Write("</tr>");
                        }
                        hw.Write("</table>");
                    }
                    wr.Close();
                }
                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //Export list view data into html file
        private void Export_TO_Excel(ListView grdView, String grdName)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Exported Reports";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string fileName = directoryPath + "\\" + grdName + "_" + DateTime.Now.ToString().Replace(":", "_") + ".html";

                int cols;
                using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {

                    StreamWriter wr = new StreamWriter(stream);
                    cols = grdView.Columns.Count;
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(wr))
                    {
                        hw.Write("<table border=1>");
                        hw.Write("<tr>");
                        for (int i = 0; i < cols; i++)
                        {
                            try
                            {
                                hw.Write("<td>");
                                hw.Write("<b>" + grdView.Columns[i].Text.ToUpper() + "</b>");
                                hw.Write("</td>");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        hw.Write("</tr>");
                        for (int i = 0; i < (grdView.Items.Count); i++)
                        {
                            try
                            {
                                hw.Write("<tr>");
                                hw.Write("<td>");
                                hw.Write(grdView.Items[i].Text);
                                hw.Write("</td>");
                                for (int j = 1; j < cols; j++)
                                {
                                    hw.Write("<td>");
                                    hw.Write(grdView.Items[i].SubItems[j].Text);
                                    hw.Write("</td>");
                                }
                                hw.Write("</tr>");
                            }
                            catch
                            {

                            }
                        }
                        hw.Write("</table>");
                    }
                    wr.Close();
                    Process.Start(fileName);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //To update report generation status to user
        private void UpdateReportStatus()
        {
            new Thread(() =>
            {
                try
                {
                    lblStatus.Text = "Report Generating";
                    double totalPer = ReportMaster.Status.Count * 100;
                    double competedPer = 0;
                    double temp = 0;
                    while (totalPer > competedPer)
                    {
                        competedPer = 0;
                        foreach (string key in ReportMaster.Status.Keys)
                        {
                            competedPer += ReportMaster.Status[key].Percentage;
                        }
                        try
                        {
                            lblStatus.Text = string.Format("Report Generating. {0}% Completed.", Convert.ToInt32((competedPer / totalPer) * 100));
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                    btnRun.Visible = true;
                    lblStatus.Text = "Completed";
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }

            }).Start();
        }

        #region events
        //Delete a scenario
        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvScenarios.SelectedNode != null)
                {
                    if (MessageBox.Show("Are you sure you want to delete selected scenario?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ((XmlNode)tvScenarios.SelectedNode.Tag).ParentNode.RemoveChild((XmlNode)tvScenarios.SelectedNode.Tag);
                        string strScenarioFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + "VUScripts.xml";
                        DeleteXmlNode(strScenarioFilePath, "scenario", "name", tvScenarios.SelectedNode.Text);
                        tvScenarios.SelectedNode.Remove();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //This function will be called when user click run button.
        private void btnRun_Click(object sender, EventArgs e)
        {
            AppedoLT.Core.Constants.GetInstance().btnExecutionType = "Run";
            //If script selected, We try to select scenario
            if (tvScenarios.SelectedNode != null && tvScenarios.SelectedNode.Level != 0)
            {
                if (tvScenarios.SelectedNode.Level == 1)
                {
                    tvScenarios.SelectedNode.Parent.Selected = true;
                }
            }

            //Checking license
            if (tvScenarios.SelectedNode != null && ValidateLicence((XmlNode)tvScenarios.SelectedNode.Tag) == true)
            {
                try
                {
                    MessageBox.Show("Please save any changes done, before validating or running scripts.");
                    _scriptExecutorList.Clear();
                    tmrExecution.Stop();

                    frmRun objFrmRun = new frmRun();
                    if (objFrmRun.ShowDialog() == DialogResult.OK)
                    {
                        lsvErrors.Items.Clear();
                        lblErrorCount.Text = "0";
                        lblHitCount.Text = "0";
                        _hitCount = 0;
                        _repositoryXml.Save();
                        //Global setting to store data
                        executionReport.ReportName = objFrmRun.strReportName;
                        executionReport.ScenarioName = tvScenarios.SelectedNode.Text;
                        executionReport.ExecutionStatus = Status.Running;

                        Request.IPSpoofingEnabled = Convert.ToBoolean(((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["enableipspoofing"].Value);
                        lblStatus.Text = "Running";
                        //Update new report name 
                        LoadReportName(executionReport.ReportName);
                       // userControlCharts1.LoadReportName(executionReport.ReportName);

                        lblUserCompleted.Text = "0";
                        listView1.Items.Clear();
                        XmlNode run = _repositoryXml.Doc.CreateElement("run");
                        run.Attributes.Append(_repositoryXml.GetAttribute("reportname", executionReport.ReportName));

                        //if Local run
                        if (objUCLoadGen.IsLoadGeneratorSelected() == false)
                        {
                            #region Without Loadgen
                            //Creating scenario xml
                            XmlDocument scenario = GetScenarioForRun(((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["id"].Value, executionReport.ReportName, 1, 1, Convert.ToBoolean(((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["enableipspoofing"].Value));
                            string strSelectedUserAgent = ((KeyValuePair<string, string>)comboBrowserVersion.SelectedItem).Value.Trim();
                            if (strSelectedUserAgent != null && strSelectedUserAgent != "Recorded Agent")
                            {
                                string strScenario = scenario.InnerXml.ToString();
                                string pattern = "<header name=\"User-Agent\" value=\"(.*?)\"";

                                Match m = Regex.Match(strScenario, pattern);
                                while (m.Success)
                                {
                                    Console.WriteLine("'{0}' found at position {1}", m.Value, m.Index);
                                    Group g = m.Groups[1];
                                    strScenario = strScenario.Replace(g.Value, strSelectedUserAgent);
                                    m = m.NextMatch();
                                }
                                scenario = new XmlDocument();
                                scenario.LoadXml(strScenario);
                                //XmlDocument aa = new XmlDocument();
                                //aa.LoadXml(strScenario);
                                //scenario = aa;
                            }
                            run.AppendChild(GetRuntimeScriptDetail(scenario));
                            //Update varialble details
                            VariableManager.dataCenter = new VariableManager();
                            foreach (XmlNode script in scenario.SelectNodes("//script"))
                            {
                                string scriptid = script.Attributes["id"].Value;
                                XmlNode setting = script.SelectNodes("//script[@id='" + scriptid + "']//setting")[0];
                                XmlNode vuscript = script.SelectNodes("//script[@id='" + scriptid + "']//vuscript")[0];
                                ScriptExecutor scriptRunnerSce = new ScriptExecutor(setting, vuscript, executionReport.ReportName);

                                // Write Settings
                                try
                                {
                                    ReportMaster mas = new ReportMaster(executionReport.ReportName);
                                    mas.Executequery(executionReport.ReportName, _constants.GetSettingsQuery(executionReport.ReportName, scriptid, setting));
                                }
                                catch (Exception excp)
                                {
                                }

                                //_scriptExecutorList used to store list of script selected by user
                                _scriptExecutorList.Add(scriptRunnerSce);
                            }

                            #region Run detail
                            // Run detail stored in xml for further ref.
                            run.Attributes.Append(_repositoryXml.GetAttribute("starttime", DateTime.Now.ToString()));
                            run.Attributes.Append(_repositoryXml.GetAttribute("loadgenused", false.ToString()));
                            XmlNode runs = _repositoryXml.Doc.SelectSingleNode("//runs");
                            if (runs != null)
                            {
                                runs.AppendChild(run);
                                _repositoryXml.Save();
                            }
                            #endregion

                            foreach (ScriptExecutor scr in _scriptExecutorList)
                            {
                                //Mapping function
                                //scr_OnLockReportData function will be called when vuser has reportdata
                                scr.OnLockReportData += scr_OnLockReportData;
                                //scr_OnLockError function will be called when vuser has error
                                scr.OnLockError += scr_OnLockError;
                                //scr_OnLockLog function will be called when vuser has log
                                scr.OnLockLog += scr_OnLockLog;
                                //scr_OnLockTransactions function will be called when vuser has Transactions
                                scr.OnLockTransactions += scr_OnLockTransactions;
                                //scr_OnIterationCompleted function will be called when iteration complete
                                scr.OnIterationStarted += scr_OnIterationCompleted;
                                //scr_OnVUserRunCompleted function will be called when vuser is completed
                                scr.OnVUserRunCompleted += scr_OnVUserRunCompleted;
                                //scr_OnVUserCreated function will be called when vuser is created
                                scr.OnVUserCreated += scr_OnVUserCreated;
                                scr.Run();
                            }
                            //Start timers to update status
                            runTime.Reset();
                            runTime.Start();
                            tmrExecution.Start();
                            #endregion
                            _isUseLoadGen = false;
                        }
                        //Old log
                        else
                        {
                            _loadGeneratorips.Clear();
                            #region Loadgen
                            bool isAllLoadgenConnected = true;
                            StringBuilder disconnectedHost = new StringBuilder();
                            foreach (XmlNode loadgen in objUCLoadGen.GetLoadGenerators())
                            {
                                System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                                try
                                {
                                    Trasport controller = new Trasport(loadgen.Attributes["ipaddress"].Value, "8888");
                                    controller.Send(new TrasportData("TEST", string.Empty, null));
                                    controller.Receive();
                                    controller.Close();
                                }
                                catch (Exception)
                                {
                                    isAllLoadgenConnected = false;
                                    disconnectedHost.AppendLine(loadgen.Attributes["hostname"].Value);
                                }
                            }
                            if (isAllLoadgenConnected == false)
                            {
                                MessageBox.Show("Please connect following host \n" + disconnectedHost.ToString());
                            }
                            else
                            {
                                int loadGenId = 0;
                                List<XmlNode> loadGens = objUCLoadGen.GetLoadGenerators();

                                foreach (XmlNode loadgen in loadGens)
                                {
                                    loadGenId++;
                                    XmlDocument scenario = GetScenarioForRun(((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["id"].Value, executionReport.ReportName, loadGens.Count, loadGenId, Convert.ToBoolean(((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["enableipspoofing"].Value));
                                    run.AppendChild(GetRuntimeScriptDetail(scenario));
                                    //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                                    try
                                    {
                                        Trasport controller = new Trasport(loadgen.Attributes["ipaddress"].Value, "8888");
                                        Dictionary<string, string> header = new Dictionary<string, string>();
                                        header.Add("reportname", executionReport.ReportName);
                                        header.Add("scenarioname", executionReport.ScenarioName);
                                        header.Add("loadgens", "192.168.1.70");
                                        controller.Send(new TrasportData("run", scenario.InnerXml, header));
                                        _loadGeneratorips.Add(loadgen.Attributes["ipaddress"].Value);

                                        #region Run detail
                                        XmlNode loadGenDetail = loadgen.Clone();
                                        loadGenDetail.Attributes.Append(_repositoryXml.GetAttribute("resultfilereceived", false.ToString()));
                                        run.AppendChild(loadGenDetail);
                                        run.Attributes.Append(_repositoryXml.GetAttribute("starttime", DateTime.Now.ToString()));
                                        run.Attributes.Append(_repositoryXml.GetAttribute("loadgenused", true.ToString()));
                                        XmlNode runs = _repositoryXml.Doc.SelectSingleNode("//runs");
                                        if (runs != null)
                                        {
                                            runs.AppendChild(run);
                                            _repositoryXml.Save();
                                        }
                                        controller.Receive();
                                        #endregion
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    runTime.Reset();
                                    runTime.Start();
                                    tmrExecution.Start();
                                }
                            }
                            #endregion
                            _isUseLoadGen = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }

        }

        //This function will be called when new vuser creates.
        void scr_OnVUserCreated(string scriptname,int userid)
        {
            lock (listView1)
            {
                //To show user creation details to user
                ListViewItem newItem = new ListViewItem(scriptname+"_"+userid.ToString());
                newItem.SubItems.AddRange(new string[] { "0".ToString(), "Running" });
                listView1.Items.Add(newItem);
            }
        }

        //This function will be called when vuser complete.
        void scr_OnVUserRunCompleted(string scriptname, int userid)
        {
            lock (listView1)
            {
                //To show user completed details to user
                ListViewItem newItem = listView1.FindItemWithText(scriptname + "_" + userid.ToString());
                if (newItem != null)
                {
                    newItem.SubItems[2].Text = "Completed";
                }
            }
        }

        //This function will be called when user complete iteration
        void scr_OnIterationCompleted(string scriptname, int userid, int iterationid)
        {
            lock (listView1)
            {
                //To show iteration completed details to user
                ListViewItem newItem = listView1.FindItemWithText(scriptname + "_" + userid.ToString());
                if (newItem != null)
                {
                    newItem.SubItems[1].Text = iterationid.ToString();
                }
            }
        }


        //Store Transactions during the run
        void scr_OnLockTransactions(TransactionRunTimeDetail data)
        {
            lock (DataServer.GetInstance().transcations)
            {
                _dataServer.transcations.Enqueue(data);
            }
        }

        //Store errors during the run
        void scr_OnLockLog(Log data)
        {
            lock (DataServer.GetInstance().logs)
            {
                _dataServer.logs.Enqueue(data);
            }
        }

        //Store errors during the run
        void scr_OnLockError(RequestException data)
        {
            lock (DataServer.GetInstance().errors)
            {
                try
                {
                    data.message = data.message.Replace("\r\n", " ");
                    //To show errors to user
                    ListViewItem newItem = new ListViewItem(data.requestexceptionid.ToString());
                    newItem.SubItems.AddRange(new string[] {  data.loadGen, 
                                                              data.reportname,
                                                              data.scenarioname, 
                                                              data.scriptname, 
                                                              data.containerid,
                                                              data.containername,
                                                              data.requestid,
                                                              data.userid, 
                                                              data.iterationid,
                                                              data.time.ToString("yyyy-MM-dd HH:mm:ss"), 
                                                              data.message.Replace("\"", "\"\""),
                                                              data.request.Replace("\"", "\"\""),
                                                              data.errorcode });
                    lsvErrors.Items.Add(newItem);
                    _dataServer.errors.Enqueue(data);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
        }

        //Store report data during the run
        void scr_OnLockReportData(ReportData data)
        {
            _hitCount++;
            _dataServer.LogResult(data);
        }

        //Stop run
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                //Stop status update timers
                runTime.Stop();
                tmrExecution.Stop();
                //Local run
                if (_isUseLoadGen == false)
                {
                    foreach (ScriptExecutor thread in _scriptExecutorList)
                    {
                        if (thread != null)
                        {
                            thread.Stop();
                        }
                    }
                    executionReport.ExecutionStatus = Status.Completed;
                    WaitUntillExecutionComplete();
                    //Create summary report
                    CreateSummaryReport(executionReport.ReportName);
                    ReportMaster reportMaster = new ReportMaster(executionReport.ReportName);
                    reportMaster.GenerateReports();
                    UpdateReportStatus();
                    userControlReports2.LoadReportName(executionReport.ReportName);

                }
                //Old logic
                else
                {
                    foreach (string objClient in _loadGeneratorips)
                    {
                        try
                        {
                            Trasport controller = new Trasport(objClient, "8888");
                            controller.Send(new TrasportData("stop", string.Empty, null));
                            controller.Receive();
                        }
                        catch (Exception ex)
                        {
                        }

                    }

                    executionReport.ExecutionStatus = Status.Completed;
                    runTime.Stop();
                    tmrExecution.Stop();
                    WaitUntillExecutionComplete();
                    if (executionReport.ReportName != null)
                    {
                        CreateSummaryReport(executionReport.ReportName);
                        ReportMaster reportMaster = new ReportMaster(executionReport.ReportName);
                        reportMaster.GenerateReports();
                        UpdateReportStatus();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Sava script
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tvScenarios.SelectedNode != null && tvScenarios.SelectedNode.Level == 1)
            {
                ((UCScriptSetting)pnlScriptSettings.Controls["UCScriptSetting"]).Save();
            }
        }

        //Add new scenario
        private void btnAddNewScenario_Click(object sender, EventArgs e)
        {
            try
            {
                frmScenario frmscenario = new frmScenario(VuscriptXml.GetScriptidAndName());
                if (frmscenario.ShowDialog() == DialogResult.OK)
                {
                    LoadScenarioTree();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Edit scenario
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvScenarios.SelectedNode != null && tvScenarios.SelectedNode.Level == 0)
                {
                    frmScenario frmscenario = new frmScenario(VuscriptXml.GetScriptidAndName(), (XmlNode)tvScenarios.SelectedNode.Tag);
                    if (frmscenario.ShowDialog() == DialogResult.OK)
                    {
                        LoadScenarioTree();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Scenario context menu opening
        private void cmnuScenario_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (tvScenarios.SelectedNode.Level == 0)
                {
                    editToolStripMenuItem.Visible = deleteToolStripMenuItem1.Visible = runToolStripMenuItem.Visible = true;
                }
                else
                {
                    editToolStripMenuItem.Visible = deleteToolStripMenuItem1.Visible = runToolStripMenuItem.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //User click scenario node
        private void tvScenarios_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                Control temp = pnlScriptSettings.Controls["objUCLoadGen"];
                pnlScriptSettings.Controls.Clear();
                pnlScriptSettings.Controls.Add(temp);

                if (tvScenarios.SelectedNode.Level == 0)
                {
                    //  objUCLoadGen.Visible = true;
                }
                //User select script
                else if (tvScenarios.SelectedNode.Level == 1)
                {
                    objUCLoadGen.Visible = false;
                    try
                    {
                        XmlNode node = ((XmlNode)tvScenarios.SelectedNode.Tag);
                        //To display script setting UI
                        UCScriptSetting objUCScript = new UCScriptSetting();
                        objUCScript.Tag = tvScenarios.SelectedNode.Tag;
                        objUCScript.vUScriptSetting = (XmlNode)tvScenarios.SelectedNode.Tag;
                        objUCScript.strScriptName = ((XmlNode)tvScenarios.SelectedNode.Tag).Attributes["name"].Value;
                        objUCScript.SetLoadScenario(((XmlNode)tvScenarios.SelectedNode.Tag).SelectNodes("setting")[0]);
                        objUCScript.Location = new System.Drawing.Point(0, 0);
                        pnlScriptSettings.Controls.Add(objUCScript);

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

        private void btnExport_Click(object sender, EventArgs e)
        {
            Export_TO_Excel(lsvErrors, "Error_Data");
        }


        #endregion

        #region While script running

        private void tmrExecution_Tick(object sender, EventArgs e)
        {
            try
            {
                btnRun.Visible = false;
                int isCompleted = 0;

                if (_isUseLoadGen == true)
                {
                    Regex log = new Regex("createduser: ([0-9]*)\r\ncompleteduser: ([0-9]*)\r\niscompleted: ([0-9]*)");
                    int loadGenCreatedUser = 0;
                    int loadGenCompetedUser = 0;
                    int tempIsCompleted = 0;
                    foreach (string objClient in _loadGeneratorips)
                    {
                        try
                        {
                            #region Retrive Created & Completed UserCount
                            Trasport controller = new Trasport(objClient, "8888");
                            controller.Send(new TrasportData("status", string.Empty, null));
                            TrasportData data = controller.Receive();

                            string dataStr = data.DataStr;

                            loadGenCreatedUser = Convert.ToInt32(log.Match(dataStr).Groups[1].Value);
                            loadGenCompetedUser = Convert.ToInt32(log.Match(dataStr).Groups[2].Value);
                            tempIsCompleted = Convert.ToInt32(log.Match(dataStr).Groups[3].Value);
                            #endregion

                            #region Store info into list
                            string address = ((System.Net.IPEndPoint)(controller.tcpClient.Client.RemoteEndPoint)).Address.ToString();
                            if (loadGenUserDetail.ContainsKey(address) == false)
                            {
                                loadGenUserDetail.Add(address, loadGenCreatedUser + "," + loadGenCompetedUser + "," + tempIsCompleted);
                            }
                            else
                            {
                                loadGenUserDetail[address] = loadGenCreatedUser + "," + loadGenCompetedUser + "," + tempIsCompleted;
                            }
                            executionReport.CreatedUser = 0;
                            executionReport.CompletedUser = 0;
                            #endregion

                            #region Consolidate Createted and completed info
                            foreach (string keys in loadGenUserDetail.Keys)
                            {
                                executionReport.CreatedUser += Convert.ToInt32(loadGenUserDetail[keys].Split(',')[0]);
                                executionReport.CompletedUser += Convert.ToInt32(loadGenUserDetail[keys].Split(',')[1]);
                                isCompleted += Convert.ToInt32(loadGenUserDetail[keys].Split(',')[2]);
                            }

                            controller.Close();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            isCompleted++;
                        }
                        finally
                        {
                            loadGenCreatedUser = 0;
                            loadGenCompetedUser = 0;
                        }
                    }
                }

                if (executionReport.CreatedUser > 0) lblUserCreated.Text = executionReport.CreatedUser.ToString();
                lblUserCompleted.Text = executionReport.CompletedUser.ToString();
                lblErrorCount.Text = lsvErrors.Items.Count.ToString();
                lblHitCount.Text = _hitCount.ToString();
                //If running update status
                if (runTime.IsRunning == true)
                {
                    lblElapsedTime.Text = string.Format("{0}:{1}:{2}", runTime.Elapsed.Hours.ToString("00"), runTime.Elapsed.Minutes.ToString("00"), runTime.Elapsed.Seconds.ToString("00"));
                }
                else
                {
                    lblElapsedTime.Text = "0";
                }

                //If loadgen used.
                if (_isUseLoadGen)
                {
                    #region loadgen
                    //If run completed. It is old logic
                    //if (lblUserCreated.Text != "0" && lblUserCreated.Text == lblUserCompleted.Text && _loadGeneratorips.Count == isCompleted)
                    //{
                    //    lblElapsedTime.Text = string.Format("{0}:{1}:{2}", runTime.Elapsed.Hours.ToString("00"), runTime.Elapsed.Minutes.ToString("00"), runTime.Elapsed.Seconds.ToString("00"));
                    //    executionReport.ExecutionStatus = Status.Completed;
                    //    //Stop status update timers
                    //    runTime.Stop();
                    //    tmrExecution.Stop();
                    //    Thread.Sleep(6000);
                    //    //Receive all data file
                    //    ReceiveAllLoadGenDatafiles(executionReport.ReportName);
                    //    WaitUntillExecutionComplete();
                    //    if (executionReport.ReportName != null)
                    //    {
                    //        CreateSummaryReport(executionReport.ReportName);
                    //        ReportMaster reportMaster = new ReportMaster(executionReport.ReportName);
                    //        reportMaster.GenerateReports();
                    //        UpdateReportStatus();
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    // if (_scriptExecutorList.Count > 0 && _scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count && executionReport.CreatedUser != 0 && executionReport.CreatedUser == executionReport.CompletedUser)
                    int tempCreatedUser = 0;
                    int tempCompletedUser = 0;
                    //To calculate total user created and total user completed
                    foreach (ScriptExecutor scripts in _scriptExecutorList)
                    {
                        tempCreatedUser += scripts.StatusSummary.TotalVUserCreated;
                        tempCompletedUser += scripts.StatusSummary.TotalVUserCompleted;
                    }
                    //If run completed
                    if (_scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count && tempCreatedUser != 0 && tempCreatedUser == tempCompletedUser)
                    {
                        //Stop status update timers
                        runTime.Stop();
                        tmrExecution.Stop();
                        executionReport.ExecutionStatus = Status.Completed;
                        _scriptExecutorList.Clear();
                        lblUserCreated.Text = tempCreatedUser.ToString();
                        lblUserCompleted.Text = tempCompletedUser.ToString();
                        //lblHitCount.Text = _hitCount.ToString();
                        lblHitCount.Text = Convert.ToString(RequestCountHandler._ReqCount);
                        // reset request count to zero                        
                        RequestCountHandler._ReqCount = 0;
                        WaitUntillExecutionComplete();
                        Thread.Sleep(5000);
                        if (executionReport.ReportName != null)
                        {
                            //To create summmary report
                            CreateSummaryReport(executionReport.ReportName);
                            ReportMaster reportMaster = new ReportMaster(executionReport.ReportName);
                            reportMaster.GenerateReports();
                            UpdateReportStatus();
                            userControlReports2.LoadReportName(executionReport.ReportName);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);

            }
        }

        //Delay until run complete
        private void WaitUntillExecutionComplete()
        {
            ReceiveAllLoadGenDatafiles(executionReport.ReportName);
            DataServer resultLog = DataServer.GetInstance();
            int time = 4;

            while (true && time-- > 0)
            {
                if (resultLog.reportDT.Count > 0 || resultLog.transcations.Count > 0 || resultLog.errors.Count > 0)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    break;
                }
            }
            time = 4;
            while (true && time-- > 0)
            {
                if ((resultLog.reportDataFile != null && resultLog.reportDataFile.BaseStream != null && resultLog.reportDataFile.BaseStream.Length > 0) || (resultLog.transactionFile != null && resultLog.transactionFile.BaseStream != null && resultLog.transactionFile.BaseStream.Length > 0) || (resultLog.errorFile.BaseStream != null && resultLog.errorFile.BaseStream.Length > 0))
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }

        }

        //Old logic
        private void ReceiveAllLoadGenDatafiles(string reportName)
        {
            try
            {
                if (_isUseLoadGen == true)
                {
                    foreach (string objClient in _loadGeneratorips)
                    {
                        string ipAddress = objClient;
                        string filePath;
                        int readCount = 0;
                        foreach (XmlNode loadGen in _repositoryXml.Doc.SelectSingleNode("//runs/run[@reportname='" + reportName + "' and @loadgenused='True']").SelectNodes("loadgen"))
                        {
                            if (loadGen.Attributes["resultfilereceived"].Value == "False")
                            {
                                ipAddress = loadGen.Attributes["ipaddress"].Value;
                                filePath = _constants.ExecutingAssemblyLocation + "\\Data\\" + reportName + "\\database_" + ipAddress.Replace('.', '_') + ".db";
                                readCount = 0;
                                try
                                {
                                    TcpClient clt = new TcpClient();
                                    clt.Connect(IPAddress.Parse(ipAddress), 8888);
                                    clt.Client.Send(Encoding.ASCII.GetBytes("resultfile: 0\r\nreportid= " + reportName + "\r\n\r\n"));
                                    NetworkStream stream = clt.GetStream();
                                    string header = _constants.ReadHeader(stream);
                                    string operation = new Regex("(.*): ([0-9]*)").Match(header).Groups[1].Value;
                                    int streamLength = Convert.ToInt32(new Regex("(.*): ([0-9]*)").Match(header).Groups[2].Value);

                                    byte[] buffer = new byte[8192];


                                    if (File.Exists(filePath)) File.Delete(filePath);
                                    using (FileStream file = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                                    {
                                        while (streamLength > 0)
                                        {

                                            if (streamLength < buffer.Length)
                                            {
                                                readCount = stream.Read(buffer, 0, streamLength);
                                            }
                                            else
                                            {
                                                readCount = stream.Read(buffer, 0, buffer.Length);
                                            }

                                            streamLength = streamLength - readCount;
                                            file.Write(buffer, 0, readCount);
                                        }
                                    }
                                    loadGen.Attributes["resultfilereceived"].Value = true.ToString();
                                    clt.Client.Close();
                                    buffer = null;
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //It will return list of scripts name and corresponding id
        private XmlNode GetRuntimeScriptDetail(XmlNode scenario)
        {

            XmlNode scripts = _repositoryXml.Doc.CreateElement("scripts");
            foreach (XmlNode script in scenario.SelectNodes("//script"))
            {
                XmlNode scrt = _repositoryXml.Doc.CreateElement("script");
                scrt.Attributes.Append(_repositoryXml.GetAttribute("id", script.Attributes["id"].Value));
                scrt.Attributes.Append(_repositoryXml.GetAttribute("name", script.Attributes["name"].Value));
                scripts.AppendChild(scrt);
            }

            return scripts;
        }

        #endregion

        //To create summary report
        private void CreateSummaryReport(string reportName)
        {
            try
            {
                ReportMaster mas = new ReportMaster(reportName);
                mas.Executequery(reportName, _constants.GetQuery(reportName, _repositoryXml.Doc));
                XmlNode runNode = _repositoryXml.Doc.SelectSingleNode("//run[@reportname='" + reportName + "']");
                Result.GetInstance().GetSummaryReportByScript(reportName, runNode);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        private void proxySettings_Click(object sender, EventArgs e)
        {
            AppedoLT.Forms.frmProxyConfiguration vm = new AppedoLT.Forms.frmProxyConfiguration();
            vm.ShowDialog();
        }

       
    }
}