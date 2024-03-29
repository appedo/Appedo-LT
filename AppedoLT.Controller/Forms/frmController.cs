﻿using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLTController
{
    /// <summary>
    /// This is act like service. Every 20 sec it will communicate with appedo and get run detail.
    /// 
    /// prerequisites:
    ///  Appedo ip address
    ///  Appedo port
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class frmController : Form
    {
        NotifyIcon ni = new NotifyIcon();
        TcpListener serverSocket = new TcpListener(8888);
        TcpClient clientSocket = default(TcpClient);

        BackgroundWorker worker = new BackgroundWorker();
        Constants constants = Constants.GetInstance();
        ControllerXml _ControllerXml = ControllerXml.GetInstance();

        Thread DoWorkThread = null;
        Dictionary<string, Controller> Controllers = Controller.Controllers;

        object test = new object();
        string AppedoServer = string.Empty;
        string AppedoFailedUrl = string.Empty;
        bool isClientRunning = false;

        public frmController()
        {
            InitializeComponent();
            this.Visible = false;
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (!Directory.Exists(".\\Data")) Directory.CreateDirectory(".\\Data");
                if (!Directory.Exists(".\\Upload")) Directory.CreateDirectory(".\\Upload");
                if (!Directory.Exists(".\\Variables")) Directory.CreateDirectory(".\\Variables");

                AppedoServer = ControllerXml.GetInstance().doc.SelectSingleNode("//runs").Attributes["appedoipaddress"].Value;
                AppedoFailedUrl = ControllerXml.GetInstance().doc.SelectSingleNode("//runs").Attributes["failedurl"].Value;
                if (AppedoServer == string.Empty)
                {
                    if ((new frmAppedoIP().ShowDialog() == DialogResult.OK))
                    {
                        AppedoServer = ControllerXml.GetInstance().doc.SelectSingleNode("//runs").Attributes["appedoipaddress"].Value;
                        AppedoFailedUrl = ControllerXml.GetInstance().doc.SelectSingleNode("//runs").Attributes["failedurl"].Value;
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
                serverSocket.Start();
                DoWorkThread = new Thread(new ThreadStart(DoWork));
                DoWorkThread.Start();
                ni.Icon = new Form().Icon;
                ni.Text = "AppedoLT Controller.";
                ni.Visible = true;
                ni.ContextMenuStrip = new ContextMenus().Create();
                ni.BalloonTipText = "AppedoLT Controller started";
                ni.ShowBalloonTip(1000);
                ni.ContextMenuStrip = contextMenuStrip1;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Old logic
        void DoWork()
        {
            try
            {
                CollectFailedTest(ControllerXml.GetInstance().doc.SelectSingleNode("//runs").CloneNode(true));
                StartClient();
                while ((true))
                {
                    try
                    {
                        if (serverSocket == null) serverSocket = new TcpListener(8888);
                        Trasport UIclient = new Trasport(serverSocket.AcceptTcpClient());

                        new Thread(() =>
                         {
                             string runid = string.Empty;
                             try
                             {
                                 TrasportData data = UIclient.Receive();

                                 switch (data.Operation.ToLower())
                                 {

                                     case "run":

                                         RunOperation(UIclient, data);
                                         if (isClientRunning == false)
                                         {
                                             StartClient();
                                         }

                                         break;

                                     case "test":

                                         UIclient.Send(new TrasportData("ok", string.Empty, null));

                                         break;

                                     case "stop":
                                         runid = data.Header["runid"];
                                         if (Controllers.ContainsKey(runid) == true) Controllers[runid].Stop();
                                         UIclient.Send(new TrasportData("response", "ok", null));
                                         break;

                                     case "runstatus":
                                         UIclient.Send(new TrasportData("runstatus", ExceptionHandler.GetLog(), null));
                                         break;

                                     case "resultfilejmeter":
                                         {
                                             string reportid = data.Header["reportid"];
                                             GenerateReportFolderJmeter(reportid);
                                             File.Delete(constants.ExecutingAssemblyLocation + "\\result.csv");
                                             using (FileStream file = new FileStream(constants.ExecutingAssemblyLocation + "\\result.csv", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                                             {
                                                 file.Write(data.DataStream.ToArray(), 0, Convert.ToInt32(data.DataStream.Length));
                                             }
                                             Constants.GetInstance().ExecuteBat(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat");
                                             CreateSummaryReport(data.Header["reportid"]);

                                             string chartSummaryFile = constants.ExecutingAssemblyLocation + "\\Data\\" + data.Header["reportid"] + "\\Report\\chart_ summary.csv";
                                             string reportSummaryFile = constants.ExecutingAssemblyLocation + "\\Data\\" + data.Header["reportid"] + "\\Report\\summary.xml";
                                             TrasportData response = null;
                                             if (File.Exists(chartSummaryFile) == true && File.Exists(reportSummaryFile) == true)
                                             {
                                                 data = new TrasportData("RESULT", null, chartSummaryFile);
                                                 UIclient.Send(data);
                                                 response = UIclient.Receive();

                                                 data = new TrasportData("SUMMARYREPORT", null, reportSummaryFile);
                                                 UIclient.Send(data);
                                                 response = UIclient.Receive();

                                                 string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportid;
                                                 try
                                                 {
                                                     if (Directory.Exists(folderPath))
                                                     {
                                                         Directory.Delete(folderPath, true);
                                                     }
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                                 }
                                             }
                                             else
                                             {
                                                 data = new TrasportData("ERROR", "Unable to get report", null);
                                                 UIclient.Send(data);
                                             }

                                             break;

                                         }
                                     case "resultfile":
                                         {
                                             lock (test)
                                             {
                                                 runid = data.Header["runid"];
                                                 string chartSummaryFile = constants.ExecutingAssemblyLocation + "\\Data\\" + runid + "\\Report\\chart_ summary.csv";
                                                 string reportSummaryFile = constants.ExecutingAssemblyLocation + "\\Data\\" + runid + "\\Report\\summary.xml";
                                                 TrasportData response = null;

                                                 if (Controllers.ContainsKey(runid) == true && Controllers[runid].Status < ControllerStatus.ReportGenerateCompleted)
                                                 {
                                                     UIclient.Send(new TrasportData("REPORTGENERATIONG", string.Empty, null));
                                                     response = UIclient.Receive();
                                                 }
                                                 else if (File.Exists(chartSummaryFile) == true && File.Exists(reportSummaryFile) == true)
                                                 {
                                                     data = new TrasportData("RESULT", null, chartSummaryFile);
                                                     UIclient.Send(data);
                                                     response = UIclient.Receive();

                                                     data = new TrasportData("SUMMARYREPORT", null, reportSummaryFile);
                                                     UIclient.Send(data);
                                                     response = UIclient.Receive();
                                                 }
                                                 else
                                                 {
                                                     data = new TrasportData("ERROR", "Unable to get report", null);
                                                     UIclient.Send(data);
                                                 }
                                             }
                                         }
                                         break;
                                 }
                                 UIclient.Close();
                             }
                             catch (Exception ex)
                             {
                                 ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                 Thread.Sleep(10000);
                             }
                         }).Start();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        Thread.Sleep(10000);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                Thread.Sleep(10000);
            }
        }

        //It act as service. Every 20 sec it will communicate with appedo and get run detail.
        void StartClient()
        {
            new Thread(() =>
               {
                   isClientRunning = true;
                   try
                   {
                       while (true)
                       {
                           try
                           {
                               if (AppedoServer != string.Empty)
                               {
                                   TrasportData data = new TrasportData("isqueueavailable", string.Empty, null);
                                   Trasport server = new Trasport(AppedoServer, Constants.GetInstance().AppedoPort);
                                   server.Send(data);
                                   data = server.Receive();
                                   //If there is item in queue 
                                   if (data.Header["status"] == "1")
                                   {
                                       server = new Trasport(AppedoServer, Constants.GetInstance().AppedoPort);
                                       data = new TrasportData("getrundetail", string.Empty, null);
                                       server.Send(data);
                                       data = server.Receive();
                                     
                                       ExceptionHandler.LogRunDetail(data.Header["runid"], "Received Rundetail for runid " + data.Header["runid"]);
                                       server = new Trasport(AppedoServer, Constants.GetInstance().AppedoPort);
                                       server.Send(new TrasportData("ok", string.Empty, null));
                                       //Call separate thread to do run script
                                       new Thread(() => { RunOperation(server, data); }).Start();
                                   }
                                   else
                                   {
                                       Thread.Sleep(20000);
                                   }
                               }
                               else
                               {
                                   Thread.Sleep(20000);
                                   ExceptionHandler.WritetoEventLog("AppedoServer is empty");
                               }
                           }
                           catch (Exception ex)
                           {
                               Thread.Sleep(10000);
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                       Thread.Sleep(10000);
                   }
               }).Start();
        }

        void CollectFailedTest(XmlNode faildRunNode)
        {
            new Thread(() =>
                {
                    Trasport server = null;
                    List<string> removedRunids = new List<string>();
                    try
                    {
                        foreach (XmlNode runnode in faildRunNode.SelectNodes("./run"))
                        {
                            try
                            {
                                if (runnode.Attributes["sourceip"] != null && Controllers.ContainsKey(runnode.Attributes["reportname"].Value) == false)
                                {
                                    foreach (XmlNode lodgen in runnode.SelectNodes(".//loadgen"))
                                    {
                                        try
                                        {
                                            server = new Trasport(lodgen.Attributes["ipaddress"].Value, "8889");
                                            server.Send(new TrasportData("stop", string.Empty, null));
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                        }
                                    }
                                    Dictionary<string, string> runid = new Dictionary<string, string>();
                                    runid.Add("runid", runnode.Attributes["reportname"].Value);
                                    server = new Trasport(runnode.Attributes["sourceip"].Value, Constants.GetInstance().AppedoPort);
                                    server.Send(new TrasportData("updaterunidstatus", string.Empty, runid));
                                    if (ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runnode.Attributes["reportname"].Value + "']") != null)
                                    {
                                        ControllerXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild(ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runnode.Attributes["reportname"].Value + "']"));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Thread.Sleep(10000);
                                ControllerXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild(ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runnode.Attributes["reportname"].Value + "']"));
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                            }

                        }
                        lock (ControllerXml.GetInstance())
                        {
                            ControllerXml.GetInstance().Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(10000);
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }).Start();
        }

        //Used to send data to all loadgen and initiate run
        void RunOperation(Trasport server, TrasportData data)
        {
            #region Run
            try
            {
                string runid = data.Header["runid"];
                //Negative scenario
                if (Controllers.ContainsKey(runid) == true)
                {
                    if (Controllers[runid].Status < ControllerStatus.ReportGenerateCompleted)
                    {
                        server.Send(new TrasportData("ERROR", "Already scenario running", null));
                        ExceptionHandler.LogRunDetail(runid, "Already scenario running");
                    }
                    else
                    {
                        Controllers.Remove(runid);
                    }
                }
                //If received empty scenario xml
                if (data.DataStr.EndsWith("?><root/>") == true)
                {
                    ExceptionHandler.LogRunDetail(runid, "Empty scenario received");
                    Dictionary<string, string> headerrunid = new Dictionary<string, string>();
                    headerrunid.Add("runid", runid);
                    ExceptionHandler.LogRunDetail(runid, "Empty scenario received for run id " + runid);
                    for (int index = 0; index < 20; index++)
                    {
                        try
                        {
                            Trasport Appedoserver = new Trasport(AppedoServer, Constants.GetInstance().AppedoPort);
                            //Send frailer notification to appedo
                            Appedoserver.Send(new TrasportData("failedrunidstatus", ExceptionHandler.GetLog(runid), headerrunid));
                            Appedoserver.Receive();
                            ExceptionHandler.RunDetaillog.Remove(runid);
                            break;
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }
                    }

                }
                else if (Controllers.ContainsKey(runid) == false)
                {
                    // GenerateReportFolder(runid);

                    List<string> unAvailableLoadGens = new List<string>();
                    #region SendData
                    XmlNode runDetail = null;
                    string loadGensStr = data.Header["loadgens"];
                    string scriptWiseStatusIp = string.Empty;
                    string scriptWiseStatusPort = string.Empty;
                    string statusIp = string.Empty;
                    string statusPort = string.Empty;
                    try
                    {
                        //Get ip and port to send script wise data
                        scriptWiseStatusIp = data.Header["scriptwisedataconnection"].Split(',')[0].Trim();
                        scriptWiseStatusPort = data.Header["scriptwisedataconnection"].Split(',')[1].Trim();
                        //Get ip and port to send status data
                        statusIp = data.Header["scriptdataconnection"].Split(',')[0].Trim();
                        statusPort = data.Header["scriptdataconnection"].Split(',')[1].Trim();

                        ExceptionHandler.WritetoEventLog("scriptwisedataconnection: " + scriptWiseStatusIp + " scriptWiseStatusPort: " + scriptWiseStatusPort + "scriptdataconnection: " + statusIp + "scriptdataconnection: " + statusPort);
                        data.Operation = "savescenario";
                        GenerateReportFolder(runid);
                        XmlDocument runXml = new XmlDocument();
                        runXml.LoadXml(data.DataStr);

                        runDetail = _ControllerXml.CreateRun(runid, runXml.SelectSingleNode("root"), ((IPEndPoint)server.tcpClient.Client.RemoteEndPoint).Address.ToString());

                        List<TcpClient> loadGens = new List<TcpClient>();
                        string[] loadGenIps = loadGensStr.Split(',');
                        int loadGenId = 1;
                        //Get all loadgen one by one to send data
                        foreach (string ip in loadGenIps)
                        {
                            try
                            {
                                unAvailableLoadGens.Add(ip);
                                if (data.Header.ContainsKey("totalloadgen") == false)
                                {
                                    data.Header.Add("totalloadgen", loadGenIps.Length.ToString());
                                    data.Header.Add("currentloadgenid", loadGenId++.ToString());
                                    data.Header.Add("loadgenname", ip);
                                    data.Header.Add("appedoip", statusIp);
                                    data.Header.Add("appedoport", statusPort);
                                    data.Header.Add("appedofailedurl", AppedoFailedUrl);
                                }
                                else
                                {
                                    data.Header["totalloadgen"] = loadGenIps.Length.ToString();
                                    data.Header["currentloadgenid"] = loadGenId++.ToString();
                                    data.Header["loadgenname"] = ip;
                                    data.Header["appedoip"] = statusIp;
                                    data.Header["appedoport"] = statusPort;
                                    data.Header["appedofailedurl"] = AppedoFailedUrl;
                                }
                                try
                                {
                                    TrasportData response = null;
                                    for (int index = 0; index < 20; index++)
                                    {
                                        try
                                        {
                                            Trasport loadGen = new Trasport(ip, "8889");

                                            loadGen.Send(data);
                                            response = loadGen.Receive();
                                            if (response.Operation == "ok")
                                            {
                                                ExceptionHandler.LogRunDetail(runid, "Rundetail saved on " + ip);
                                                runDetail.AppendChild(_ControllerXml.CreadLoadGen(ip, ip));
                                                loadGens.Add(loadGen.tcpClient);
                                                if (unAvailableLoadGens.Contains(ip) == true) unAvailableLoadGens.Remove(ip);
                                                break;
                                            }
                                            loadGen.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            Thread.Sleep(30000);
                                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                }

                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogRunDetail(runid, ex.Message);
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }

                    #endregion
                    //If any load gen is unavailable
                    if (unAvailableLoadGens.Count > 0)
                    {
                        Dictionary<string, string> headerrunid = new Dictionary<string, string>();
                        headerrunid.Add("runid", runid);
                        foreach (string gen in unAvailableLoadGens)
                        {
                            ExceptionHandler.LogRunDetail(runid, gen + " Loadgens not available. Run Failed ");
                        }
                        for (int index = 0; index < 20; index++)
                        {
                            try
                            {
                                Trasport Appedoserver = new Trasport(AppedoServer, Constants.GetInstance().AppedoPort);
                                Appedoserver.Send(new TrasportData("failedrunidstatus", ExceptionHandler.GetLog(runid), headerrunid));
                                Appedoserver.Receive();
                                ExceptionHandler.RunDetaillog.Remove(runid);
                                break;
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                            }
                        }
                        return;
                    }
                    //If all loadgens are available
                    else
                    {

                        data.Operation = "run";
                        data.Header.Clear();
                        data.Header.Add("runid", runid);

                        int runningThread = 0;
                        // XmlNode runNode = ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runid + "']");
                        if (runDetail != null)
                        {
                            //Get all load gen one by one
                            foreach (XmlNode loadGenNode in runDetail.SelectNodes("loadgen"))
                            {
                                try
                                {
                                    runningThread++;
                                    new Thread(() =>
                                    {
                                        for (int index = 0; index < 20; index++)
                                        {
                                            try
                                            {
                                                Trasport loadGen = new Trasport(loadGenNode.Attributes["ipaddress"].Value, "8889");
                                                //Send run command to loadgen
                                                loadGen.Send(data);
                                                loadGen.Receive();
                                                loadGen.Close();
                                                loadGenNode.Attributes["runstarted"].Value = true.ToString();
                                                ExceptionHandler.LogRunDetail(runid, "Run started on " + loadGenNode.Attributes["ipaddress"].Value);
                                                break;
                                            }
                                            catch (Exception ex)
                                            {
                                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                            }
                                        }
                                        runningThread--;

                                    }).Start();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                }
                            }
                            while (runningThread != 0)
                            {
                                Thread.Sleep(2000);
                            }
                            if (runDetail != null)
                            {
                                ExceptionHandler.LogRunDetail(runid, "Run started ");
                                //Create controller object to update status to appedo until run complete
                                Controller controller = new Controller(scriptWiseStatusIp, scriptWiseStatusPort, runid, runDetail, loadGensStr, AppedoFailedUrl);
                                Controllers.Add(runid, controller);
                                _ControllerXml.doc.SelectSingleNode("root/runs").AppendChild(runDetail);
                                _ControllerXml.Save();
                                controller.Start();
                            }
                        }
                        server.Send(new TrasportData("OK", string.Empty, null));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                Thread.Sleep(2000);
            }

            #endregion
        }

        public List<string> GetStatusList()
        {
            List<string> statusList = new List<string>();
            try
            {
                Controller ctrl = null;

                foreach (string users in Controllers.Keys)
                {
                    ctrl = Controllers[users];
                    statusList.Add(string.Format("{0},{1},{2},{3}", ctrl.RunId, ctrl.CreatedUser, ctrl.CompletedUser, ctrl.IsCompleted));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return statusList;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                try
                {
                    serverSocket.Stop();
                    Application.Exit();
                    Environment.Exit(1);
                    this.Close();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
        }

        private string GenerateReportFolder(string reportname)
        {
            string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
            try
            {
                if (Directory.Exists(folderPath))
                {
                    reportname = reportname + DateTime.Now.ToString("_dd_MM_yy_hh_mm_ss");
                    folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
                }
                Directory.CreateDirectory(folderPath);
                Directory.CreateDirectory(folderPath + "\\Report");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return folderPath;
        }

        private string DeleteReportFolder(string reportname)
        {
            string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    if (Directory.Exists(folderPath)) Directory.Delete(folderPath);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return folderPath;
        }

        private string GenerateReportFolderJmeter(string reportname)
        {
            try
            {
                string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
                if (Directory.Exists(folderPath))
                {
                    reportname = reportname + DateTime.Now.ToString("_dd_MM_yy_hh_mm_ss");
                    folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
                }
                Directory.CreateDirectory(folderPath);
                Directory.CreateDirectory(folderPath + "\\Report");

                File.Copy(Constants.GetInstance().ExecutingAssemblyLocation + "\\database_jmeter.db", folderPath + "\\database.db");
                File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat");
                File.WriteAllText(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat", @"sqlite3 " + "\"" + Constants.GetInstance().ExecutingAssemblyLocation + @"\Data\" + reportname + "\\database.db\"" + " < \"" + Constants.GetInstance().ExecutingAssemblyLocation + "\\commands.txt\"");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return reportname;
        }

        private void SendVariableFileToLoadGen(XmlDocument souce, string ipaddress, string port)
        {
            try
            {
                TrasportData respose = null;

                Trasport server = new Trasport(ipaddress, port);
                server.Send(new TrasportData("VARIABLES", souce.SelectSingleNode("//variables").OuterXml, null));
                respose = server.Receive();

                XmlDocument variableFiles = new XmlDocument();
                variableFiles.LoadXml(respose.DataStr);
                Dictionary<string, string> header = new Dictionary<string, string>();
                foreach (XmlNode varFile in variableFiles.SelectNodes("//variable"))
                {
                    string filepath = Constants.GetInstance().ExecutingAssemblyLocation + "\\" + varFile.Attributes["vituallocation"].Value;
                    if (File.Exists(filepath))
                    {
                        header.Clear();
                        header.Add("filename", new FileInfo(filepath).Name);
                        server.Send(new TrasportData("VARIABLEFILE", header, filepath));
                        respose = server.Receive();
                    }
                }
                server.Send(new TrasportData("STOP", string.Empty, null));
                respose = server.Receive();
                server.Close();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void CreateSummaryReport(string reportName)
        {
            try
            {
                JmeterReportMaster mas = new JmeterReportMaster(reportName);
                DataTable scriptlist = mas.GetJMeterScriptList();
                string query = GetQueryJMeter(reportName, scriptlist);
                mas.Executequery(reportName, query);
                Result.GetInstance().GetSummaryReportJmeterByScript(reportName, scriptlist);
                mas.GenerateReports();

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private string GetQueryJMeter(string reportName, DataTable scriptList)
        {
            StringBuilder result = new StringBuilder();

            if (scriptList.Rows.Count > 0)
            {

                foreach (DataRow script in scriptList.Rows)
                {
                    #region Script Query
                    string rampuptime = "2013-12-16 20:05:00";
                    result.AppendFormat(@"

                                           CREATE TABLE requests_{0} (                                                                                            
                                                                   address       VARCHAR 
                                                                  );

                                           CREATE TABLE requestresponse_{0} (                                                              
                                                                   address       VARCHAR,
                                                                   min           DOUBLE,
                                                                   max           DOUBLE,
                                                                   avg           DOUBLE 
                                                                   );

                                           CREATE TABLE requestresponse90_{0} ( 
                                                                   address       VARCHAR,
                                                                   min           DOUBLE,
                                                                   max           DOUBLE,
                                                                   avg           DOUBLE 
                                                                   );

                                           CREATE TABLE containerresponse_{0} ( 
                                                                   containername VARCHAR,
                                                                   min           DOUBLE,
                                                                   max           DOUBLE,
                                                                   avg           DOUBLE 
                                                                   );

                                           CREATE TABLE containerresponse90_{0} ( 
                                                                   containername VARCHAR,
                                                                   min           DOUBLE,
                                                                   max           DOUBLE,
                                                                   avg           DOUBLE 
                                                                   );

                                            CREATE TABLE throughput_{0} ( 
                                                                   address       VARCHAR,
                                                                   total         DOUBLE 
                                                                   );

                                            CREATE TABLE throughput90_{0} ( 
                                                                   address       VARCHAR,
                                                                   total         DOUBLE
                                                                   );

                                           CREATE TABLE hitcount_{0} ( 
                                                                   address       VARCHAR,
                                                                   count         INT
                                                                   );
                                      

                                           CREATE TABLE errorcount_{0} ( 
                                                                   address       VARCHAR,
                                                                   count         INT 
                                                                   );

                                           CREATE TABLE errorcode_{0} ( 
                                                                   message     VARCHAR,
                                                                   count       INT
                                                                   );

                                           insert into requests_{0} select address from jmeterdata where scriptid={0} group by address;
                                           insert into requestresponse_{0} select address,min(diff),max(diff),avg(diff) from jmeterdata where scriptid={0} group by address;
                                           insert into requestresponse90_{0} select address,min(diff),max(diff),avg(diff) from jmeterdata where scriptid={0} and starttime>'{2}' group by address;
                                           insert into containerresponse_{0} select containername,min(diff) AS min,max(diff) AS max,avg(diff) AS avg from containerresponse where scriptid={0} group by containername;
                                           insert into containerresponse90_{0} select containername,min(diff) AS min,max(diff) AS max,avg(diff) AS avg from containerresponse where scriptid={0} and starttime>'{2}' group by containername;
                                           insert into throughput_{0} select address,sum(responsesize) from jmeterdata where scriptid={0} group by address;
                                           insert into hitcount_{0} select address,count(responsesize) from jmeterdata where scriptid={0} group by address;
                                           insert into errorcount_{0} select address,count(*) from jmeterdata where scriptid={0} and success='FALSE' or success='false' group by address;
                                           insert into errorcode_{0} select httpresponsemessage,count(*) from jmeterdata where scriptid={0} and success='FALSE' or success='false' group by httpresponsemessage;", script["id"], script["scriptname"], rampuptime).AppendLine();
                    #endregion
                }
                result.Append(@" insert into summaryreport SELECT 
                                                                         MIN(starttime) AS start_time,
                                                                         MAX(endtime) AS end_time,
                                                                         (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) as duration_sec,
                                                                         (SELECT COUNT(*) AS user_count FROM (SELECT userid,scriptid FROM jmeterdata  GROUP BY userid,scriptid) AS a) AS user_count,
                                                                          count(*) AS total_hits,
                                                                          IFNULL(AVG(diff)*1.0/1000,0) AS avg_response,
                                                                          CASE WHEN (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) = 0 THEN 0
                                                                               ELSE count(*)*1.0/(strftime('%s',max(starttime)) - strftime('%s',min(starttime))) END AS avg_hits,
                                                                          IFNULL(ROUND((SUM(responsesize)*1.0/1024)/1024,3),0) AS total_throughput,
                                                                          CASE WHEN (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) = 0 THEN 0
                                                                               ELSE ((SUM(responsesize)*8.0)/(strftime('%s',max(starttime)) - strftime('%s',min(starttime))))/1024/1024 END AS avg_throughput,
                                                                          (select count(*)from jmeterdata where success='FALSE' or success='false' ) AS total_errors,
                                                                          (select count(*) from (SELECT containername from containerresponse group by containername))as total_page,
                                                                          (select IFNULL(AVG(diff)*1.0/1000,0) from (select sum(diff)as pageresponse from containerresponse  group by containername))AS avg_page_response,
                                                                          (select count(*)from jmeterdata where responsecode like '2%') AS reponse_200,
                                                                          (select count(*)from jmeterdata where responsecode like '3%' ) AS reponse_300,
                                                                          (select count(*)from jmeterdata where responsecode like '4%') AS reponse_400,
                                                                          (select count(*)from jmeterdata where responsecode like '5%') AS reponse_500
                                                                      FROM 
                                                                          jmeterdata;");
            }
            return result.ToString();
        }

        private void generateReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportGenerate().ShowDialog();
        }

        private void trimQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (_ControllerXml.doc.SelectSingleNode("root/runs").Attributes["lastrequestsourceip"].Value != string.Empty)
            //    {
            //        AppedoServer = _ControllerXml.doc.SelectSingleNode("root/runs").Attributes["lastrequestsourceip"].Value;
            //        if (isClientRunning == false)
            //        {
            //            StartClient();
            //            MessageBox.Show("Started");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            //}
        }

    }

    /// <summary>
    /// 
    /// </summary>
    class ContextMenus
    {
        /// <summary>
        /// Is the About box displayed?
        /// </summary>
        bool isAboutLoaded = false;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create()
        {
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            // Windows Explorer.
            item = new ToolStripMenuItem();
            item.Text = "Explorer";
            item.Click += new EventHandler(Explorer_Click);

            menu.Items.Add(item);

            // About.
            item = new ToolStripMenuItem();
            item.Text = "About";
            item.Click += new EventHandler(About_Click);

            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);

            menu.Items.Add(item);

            return menu;
        }

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Explorer_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", null);
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;

                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }
    }
}
