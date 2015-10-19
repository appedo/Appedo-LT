using AppedoLT.Core;
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

        TcpListener serverSocket = new TcpListener(8889);
        RunScenario run;
        BackgroundWorker worker = new BackgroundWorker();
        Constants constants = Constants.GetInstance();
        ExecutionReport executionReport = ExecutionReport.GetInstance();
        Dictionary<string, Dictionary<string, string>> runScripts = new Dictionary<string, Dictionary<string, string>>();
        StringBuilder logMsg = new StringBuilder();
        string _lastRunAppedoIP = string.Empty;
        string _lastRunAppedoPort = string.Empty;
        DataXml _dataXml = DataXml.GetInstance();

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

                ni.Icon = new Form().Icon;
                DataService();
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
                                                Dictionary<string, string> runDetail = new Dictionary<string, string>();
                                                runDetail.Add("data", data.DataStr);
                                                runDetail.Add("reportfoldername", reportFolder);
                                                runDetail.Add("scenarioname", data.Header["scenarioname"]);
                                                runDetail.Add("runid", data.Header["runid"]);
                                                runDetail.Add("appedoip", data.Header["appedoip"]);
                                                runDetail.Add("appedoport", data.Header["appedoport"]);
                                                runDetail.Add("appedofailedurl", data.Header["appedofailedurl"]);
                                                runDetail.Add("totalloadgenused", data.Header["totalloadgen"] == null ? "1" : data.Header["totalloadgen"]);
                                                runDetail.Add("currentloadgenid", data.Header["currentloadgenid"] == null ? "1" : data.Header["currentloadgenid"]);
                                                runDetail.Add("souceip", ((IPEndPoint)controller.tcpClient.Client.RemoteEndPoint).Address.ToString());
                                                runDetail.Add("loadgenname", data.Header["loadgenname"] == null ? string.Empty : data.Header["loadgenname"]);
                                                runDetail.Add("distribution", data.Header["distribution"] == null ? string.Empty : data.Header["distribution"]);
                                                runDetail.Add("loadgencounters", data.Header["loadgencounters"]);

                                                if (runScripts.ContainsKey(data.Header["runid"]) == true)
                                                {
                                                    runScripts[data.Header["runid"]] = runDetail;
                                                }
                                                else
                                                {
                                                    runScripts.Add(data.Header["runid"], runDetail);
                                                }
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
                                                if (runScripts.ContainsKey(data.Header["runid"]) == true)
                                                {
                                                    Dictionary<string, string> runDetail = runScripts[data.Header["runid"]];
                                                    executionReport.ReportName = runDetail["reportfoldername"];
                                                    executionReport.ScenarioName = runDetail["scenarioname"];
                                                    executionReport.TotalLoadGenUsed = Convert.ToInt16(runDetail["totalloadgenused"]);
                                                    executionReport.CurrentLoadGenid = Convert.ToInt16(runDetail["currentloadgenid"]);
                                                    executionReport.LoadGenName = runDetail["loadgenname"];
                                                    run = new AppedoLTLoadGenerator.RunScenario(data.Header["runid"], runDetail["appedoip"], runDetail["appedoport"], runDetail["data"], runDetail["distribution"], runDetail["appedofailedurl"], runDetail["loadgencounters"]);

                                                    if (run.Start() == true)
                                                    {
                                                        _lastRunAppedoIP = runDetail["appedoip"];
                                                        _lastRunAppedoPort = runDetail["appedoport"];
                                                        ni.Text = "Running...";
                                                        ni.BalloonTipText = "Running...";
                                                        ni.ShowBalloonTip(2000);
                                                        if (runScripts.ContainsKey(data.Header["runid"]) == true) runScripts.Remove(data.Header["runid"]);
                                                        UpdateStatus();
                                                    }
                                                }
                                            }
                                        }
                                        break;


                                    case "scriptwisestatus":
                                        {

                                            Dictionary<string, string> headers = new Dictionary<string, string>();
                                            headers.Add("createduser", run.TotalCreatedUser.ToString());
                                            headers.Add("completeduser", run.TotalCompletedUser.ToString());
                                            headers.Add("iscompleted", run.IsCompleted.ToString());

                                            controller.Send(new TrasportData("scriptwisestatus", run.GetStatus(), headers));
                                        }
                                        break;

                                    case "resultfile":
                                        {
                                            while (executionReport.ExecutionStatus == Status.Running)
                                            {
                                                Thread.Sleep(5000);
                                            }
                                            string reportName = data.Header["reportname"];
                                            string directoryPath = string.Empty;
                                            DirectoryInfo dicinfo = new DirectoryInfo(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data");
                                            string filePath = string.Empty;
                                            foreach (DirectoryInfo info in dicinfo.GetDirectories())
                                            {
                                                if (new Regex(reportName + "_[0-9]*_[0-9]*_[0-9]*_[0-9]*").Match(info.Name).Success)
                                                {
                                                    directoryPath = info.Name;
                                                    foreach (FileInfo fileInfo in info.GetFiles("database.db"))
                                                    {
                                                        filePath = fileInfo.FullName;
                                                    }
                                                    break;
                                                }
                                            }
                                            controller.Send(new TrasportData("file", null, filePath));
                                            TrasportData agn = controller.Receive();
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
                            if (run.TotalCreatedUser != 0 && run.TotalCreatedUser == run.TotalCompletedUser && run.IsCompleted == 1)
                            {

                                ni.Text = "Run completed" + System.Environment.NewLine + "Created: " + run.TotalCreatedUser.ToString() + Environment.NewLine + "Completed: " + run.TotalCompletedUser.ToString() + Environment.NewLine + timer.Elapsed.ToString(@"dd\.hh\:mm\:ss")+Environment.NewLine;
                                break;
                            }
                            else
                            {
                                ni.Text = "Running.." + System.Environment.NewLine + "Created: " + run.TotalCreatedUser.ToString() + Environment.NewLine + "Completed: " + run.TotalCompletedUser.ToString() + Environment.NewLine + timer.Elapsed.ToString(@"dd\.hh\:mm\:ss");
                            }
                        }
                    }
                }).Start();
        }

        private void DataService()
        {
            new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            foreach (XmlNode data in _dataXml.doc.SelectNodes("/root/data"))
                            {
                                try
                                {
                                    {
                                        if (File.Exists(data.Attributes["filePath"].Value) == true)
                                        {
                                            Trasport trasport = new Trasport(data.Attributes["ipadddress"].Value, data.Attributes["port"].Value, 30000);
                                            Dictionary<string, string> header = new Dictionary<string, string>();
                                            header.Add("runid", data.Attributes["runid"].Value);
                                            header.Add("queuename", "ltreport");
                                            trasport.Send(new TrasportData("status", header, data.Attributes["filePath"].Value));
                                            TrasportData ack = trasport.Receive();
                                            if (ack.Operation == "ok")
                                            {
                                                _dataXml.doc.SelectSingleNode("/root").RemoveChild(data);
                                                _dataXml.Save();
                                                File.Delete(data.Attributes["filePath"].Value);
                                            }
                                            trasport.Close();
                                            trasport = null;
                                        }
                                        else
                                        {
                                            _dataXml.doc.SelectSingleNode("/root").RemoveChild(data);
                                            _dataXml.Save();
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(5000);
                        }
                        finally
                        {
                            Thread.Sleep(5000);
                        }
                    }
                }).Start();
        }
    }
}
