using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLTLoadGenerator
{
    public partial class LoadGenerator : Form
    {
        NotifyIcon ni = new NotifyIcon();
        DataServer resultLog = DataServer.GetInstance();
        TcpListener serverSocket = new TcpListener(8889);
        RunScenario run;
        BackgroundWorker worker = new BackgroundWorker();
        Constants constants = Constants.GetInstance();
        ExecutionReport executionReport = ExecutionReport.GetInstance();
        LoadTestAgentXml _loadTestAgent;
        Dictionary<string, string> runScripts = new Dictionary<string, string>();
        StringBuilder logMsg = new StringBuilder();

        public LoadGenerator()
        {
            InitializeComponent();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (!Directory.Exists(".\\Data")) Directory.CreateDirectory(".\\Data");
                if (!Directory.Exists(".\\Upload")) Directory.CreateDirectory(".\\Upload");
                if (!Directory.Exists(".\\Variables")) Directory.CreateDirectory(".\\Variables");
                serverSocket.Start();
                _loadTestAgent = LoadTestAgentXml.GetInstance();
                ni.Icon = new Form().Icon;
                ni.Text = "AppedoLT Loadgenerator.";
                ni.Visible = true;
                ni.ContextMenuStrip = new AppedoLTLoadGenerator.ContextMenus().Create();
                ni.BalloonTipText = "AppedoLT Loadgenerator started";
                ni.ShowBalloonTip(1000);
                ni.ContextMenuStrip = contextMenuStrip1;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while ((true))
                {
                    Trasport controller = new Trasport(serverSocket.AcceptTcpClient());
                    new Thread(() =>
                        {
                            try
                            {
                                TrasportData data = controller.Receive();
                                switch (data.Operation.ToLower())
                                {
                                    case "savescenario":
                                        {
                                            try
                                            {
                                                string reportFolder = data.Header["runid"] + "_" + (data.Header["loadgenname"] == null ? string.Empty : data.Header["loadgenname"]).Replace('.', '_');
                                                GenerateReportFolder(reportFolder);
                                                if (runScripts.ContainsKey(data.Header["runid"]) == true)
                                                {
                                                    runScripts[data.Header["runid"]] = data.DataStr;
                                                }
                                                else
                                                {
                                                    runScripts.Add(data.Header["runid"], data.DataStr);
                                                }
                                                _loadTestAgent.doc.SelectSingleNode("runs").AppendChild(
                                                                    _loadTestAgent.CreadRun(data.Header["runid"],
                                                                                            reportFolder,
                                                                                            data.Header["scenarioname"],
                                                                                            data.Header["totalloadgen"] == null ? "1" : data.Header["totalloadgen"],
                                                                                            data.Header["currentloadgenid"] == null ? "1" : data.Header["currentloadgenid"],
                                                                                            ((IPEndPoint)controller.tcpClient.Client.RemoteEndPoint).Address.ToString(),
                                                                                            data.Header["loadgenname"] == null ? string.Empty : data.Header["loadgenname"],
                                                                                             data.Header["distribution"] == null ? string.Empty : data.Header["distribution"]
                                                                                           ));

                                                _loadTestAgent.Save();
                                                ni.BalloonTipText = "Saved " + data.Header["runid"];
                                                ni.ShowBalloonTip(2000);
                                                controller.Send(new TrasportData("ok", string.Empty, null));
                                            }
                                            catch (Exception ex)
                                            {
                                                controller.Send(GetErrorData("401", "Unable to Save.\r\n" + ex.Message));
                                            }
                                        }
                                        break;

                                    case "run":
                                        {
                                            controller.Send(new TrasportData("ok", string.Empty, null));
                                            if (executionReport.ExecutionStatus == Status.Completed)
                                            {
                                                executionReport.ExecutionStatus = Status.Running;
                                                logMsg = new StringBuilder();
                                                DataServer.GetInstance().logs.Clear();
                                                XmlNode runNode = _loadTestAgent.doc.SelectSingleNode("//runs/run[@runid='" + data.Header["runid"] + "']");
                                                executionReport.ReportName = runNode.Attributes["reportfoldername"].Value;
                                                executionReport.ScenarioName = runNode.Attributes["scenarioname"].Value;
                                                executionReport.TotalLoadGenUsed = Convert.ToInt16(runNode.Attributes["totalloadgenused"].Value);
                                                executionReport.CurrentLoadGenid = Convert.ToInt16(runNode.Attributes["currentloadgenid"].Value);
                                                executionReport.LoadGenName = runNode.Attributes["loadgenname"].Value;

                                                run = new AppedoLTLoadGenerator.RunScenario(runScripts[data.Header["runid"]], runNode.Attributes["distribution"].Value);
                                                if (run.Start() == true)
                                                {
                                                    ni.Text = "Running...";
                                                    ni.BalloonTipText = "Running...";
                                                    ni.ShowBalloonTip(2000);
                                                    if (runScripts.ContainsKey(data.Header["runid"]) == true) runScripts.Remove(data.Header["runid"]);
                                                    UpdateStatus();
                                                }
                                            }
                                        }
                                        break;

                                    case "status":
                                        {
                                            controller.Send(new TrasportData("status", ASCIIEncoding.Default.GetString(constants.Serialise(run.RunningStatusData)), null));
                                            TrasportData ack = controller.Receive();
                                            if (ack.Operation == "ok")
                                            {
                                                run.RunningStatusData.Log.Clear();
                                                run.RunningStatusData.Error.Clear();
                                                run.RunningStatusData.Transactions.Clear();
                                                run.RunningStatusData.ReportData.Clear();
                                                run.RunningStatusData.UserDetailData.Clear();
                                            }
                                        }
                                        break;

                                    case "scriptwisestatus":
                                        {
                                            controller.Send(new TrasportData("scriptwisestatus", run.GetStatus(), null));
                                        }
                                        break;

                                    case "resultfile":
                                        {
                                            while (executionReport.ExecutionStatus == Status.Running)
                                            {
                                                Thread.Sleep(5000);
                                            }
                                            string reportName = data.Header["reportname"];
                                            DirectoryInfo dicinfo = new DirectoryInfo(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data");
                                            string filePath = string.Empty;
                                            foreach (DirectoryInfo info in dicinfo.GetDirectories())
                                            {
                                                if (new Regex(reportName + "_[0-9]*_[0-9]*_[0-9]*_[0-9]*").Match(info.Name).Success)
                                                {
<<<<<<< HEAD
=======
                                                    directoryPath = info.Name;
>>>>>>> dev_master
                                                    foreach (FileInfo fileInfo in info.GetFiles("database.db"))
                                                    {
                                                        filePath = fileInfo.FullName;
                                                    }
                                                    break;
                                                }
                                            }
                                            controller.Send(new TrasportData("file", null, filePath));
                                            TrasportData agn = controller.Receive();
                                            if (agn.Operation == "ok")
                                            {
                                                if (agn.Header["receivedsize"] == (new FileInfo(filePath)).Length.ToString())
                                                {
<<<<<<< HEAD
                                                  LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild( LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs/run[@runid='" + reportName + "']"));
                                                  LoadTestAgentXml.GetInstance().Save();
=======
                                                    if (LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs/run[@runid='" + reportName + "']") != null)
                                                    {
                                                        LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild(LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs/run[@runid='" + reportName + "']"));
                                                        LoadTestAgentXml.GetInstance().Save();
                                                        // Directory.Delete(directoryPath,true);
                                                    }
>>>>>>> dev_master
                                                }
                                            }
                                        }
                                        break;

                                    case "chartsummary":
                                        {
                                            string reportName = data.Header["reportname"];
                                            DirectoryInfo dicinfo = new DirectoryInfo(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data");
                                            string filePath = string.Empty;
                                            foreach (DirectoryInfo info in dicinfo.GetDirectories())
                                            {
                                                if (new Regex(reportName + "_[0-9]*_[0-9]*_[0-9]*_[0-9]*").Match(info.Name).Success)
                                                {
                                                    filePath = info.FullName + "\\Report\\chart_ summary.csv";
                                                    break;
                                                }
                                            }
                                            if (File.Exists(filePath) == true)
                                                controller.Send(new TrasportData("file", null, filePath));
                                            else
                                            {
                                                XmlNode runNode = LoadTestAgentXml.GetInstance().doc.SelectSingleNode("//runs/run[@runid='" + reportName + "']");
                                                ReportMaster rm = new ReportMaster(runNode.Attributes["reportfoldername"].Value, Convert.ToDateTime(runNode.Attributes["scenariostarttime"].Value), runNode.Attributes["loadgenname"].Value);
                                                rm.SetUserRunTime();
                                                rm.SetChartSummary();
                                                if (File.Exists(filePath) == true)
                                                    controller.Send(new TrasportData("file", null, filePath));
                                            }
                                        }
                                        break;

                                    case "stop":
                                        {
                                            controller.Send(new TrasportData("ok", string.Empty, null));
                                            if (run != null) run.Stop();
                                        }
                                        break;

                                    case "test":
                                        {
                                            controller.Send(new TrasportData("ok", string.Empty, null));
                                        }
                                        break;

                                }
                                controller.Close();
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                            }
                        }).Start();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        /* 
         code:
         * 401:Unable to Save
         */
        private TrasportData GetErrorData(string code, string message)
        {
            System.Collections.Generic.Dictionary<string, string> header = new System.Collections.Generic.Dictionary<string, string>();
            header.Add("errorcode", code);
            return new TrasportData("error", message, header);
        }
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    serverSocket.Stop();
                    Application.Exit();
                    Environment.Exit(1);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        private string GenerateReportFolder(string reportname)
        {
            try
            {
                string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                Directory.CreateDirectory(folderPath);
                Directory.CreateDirectory(folderPath + "\\Report");

                File.Copy(Constants.GetInstance().ExecutingAssemblyLocation + "\\database.db", folderPath + "\\database.db");
                File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat");
                File.WriteAllText(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat", @"sqlite3 " + "\"" + Constants.GetInstance().ExecutingAssemblyLocation + @"\Data\" + reportname + "\\database.db\"" + " < \"" + Constants.GetInstance().ExecutingAssemblyLocation + "\\commands.txt\"");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return reportname;
        }
        private void UpdateStatus()
        {
            new Thread(() =>
                {
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    while (true)
                    {
                        Thread.Sleep(1000);
                        if (run != null)
                        {
                            if (run.DisplayStatusData.CreatedUser != 0 && run.DisplayStatusData.CreatedUser == run.DisplayStatusData.CompletedUser && run.DisplayStatusData.IsCompleted == 1)
                            {

                                ni.Text = "Run completed" + System.Environment.NewLine + "Created: " + run.DisplayStatusData.CreatedUser.ToString() + Environment.NewLine + "Completed: " + run.DisplayStatusData.CompletedUser.ToString() + Environment.NewLine + timer.Elapsed.ToString(@"dd\.hh\:mm\:ss");
                                break;
                            }
                            else
                            {
                                ni.Text = "Running.." + System.Environment.NewLine + "Created: " + run.DisplayStatusData.CreatedUser.ToString() + Environment.NewLine + "Completed: " + run.DisplayStatusData.CompletedUser.ToString() + Environment.NewLine + timer.Elapsed.ToString(@"dd\.hh\:mm\:ss");
                            }
                        }
                    }
                }).Start();
        }
    }
}
