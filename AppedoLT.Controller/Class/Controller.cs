using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace AppedoLTController
{
    /// <summary>
    /// This used to get scriptwise data from all loadgen and send to appedo server.
    /// 
    /// prerequisites: 
    ///  scriptWiserIP: Ip address where we need to send script wise report
    ///  scriptWiserPort: port  where we need to send script wise report
    ///  runid: Current runid received from appedo
    ///  runNode: run details
    ///  loadgens: list of load gen with comma separated value
    ///  appedoFailedUrl: Controller has to call this url when appedo server is unavailable
    /// 
    /// Author: Rasith
    ///
    /// </summary>
    class Controller
    {
        public static Dictionary<string, Controller> Controllers = new Dictionary<string, Controller>();

        private int _reportDataCount = 0;
        private Thread _CollectorThread = null;
        private Constants _constants = Constants.GetInstance();
        private XmlNode _runNode = null;
        private string _scriptWiserIP = null;
        private string _scriptWiserPort = null;
        private ControllerStatus _staus = ControllerStatus.Idle;
        private int _totalUserCreated = 0;
        private int _totalUserCompleted = 0;
        private string _appedoFailedUrl = string.Empty;
        private int _failedScriptWaseSend = 0;

        public string ScriptWiseStatus { get; set; }
        public string RunId = string.Empty;
        public ControllerStatus Status { get { return _staus; } }
        public string LastSentStatus = string.Empty;
        public int CreatedUser { get { return _totalUserCreated; } private set { } }
        public int CompletedUser { get { return _totalUserCompleted; } private set { } }
        public int IsCompleted { get { return (_staus == ControllerStatus.ReportGenerateCompleted ? 1 : 0); } private set { } }

        public int ReportDataCount
        {
            get { return _reportDataCount; }
            set { _reportDataCount = value; }
        }

        public Controller(string scriptWiserIP, string scriptWiserPort, string runid, XmlNode runNode, string loadgens, string appedoFailedUrl)
        {
            RunId = runid;
            _scriptWiserIP = scriptWiserIP;
            _scriptWiserPort = scriptWiserPort;
            _runNode = runNode;
            _appedoFailedUrl = appedoFailedUrl;
        }

        public void Start()
        {
            _CollectorThread = new Thread(new ThreadStart(DoWork));
            _CollectorThread.Start();
            _staus = ControllerStatus.Running;
        }

        //To stop run
        public void Stop()
        {
            ExceptionHandler.LogRunDetail(RunId, "Stop request received ");
            foreach (XmlNode loadGen in _runNode.SelectNodes("loadgen"))
            {
                for (int index = 0; index < 20; index++)
                {
                    try
                    {
                        Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889");
                        loadGenConnection.Send(new TrasportData("stop", string.Empty, null));
                        TrasportData data = loadGenConnection.Receive();
                        loadGenConnection.Close();
                        ExceptionHandler.LogRunDetail(RunId, "Stop request sent to loadgen " + loadGen.Attributes["ipaddress"].Value);
                        break;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        //Old logic
        private string DeleteReportFolder(string reportname)
        {
            string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    ExceptionHandler.LogRunDetail(reportname, "Directory deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return folderPath;
        }

        //It act as service. it will  collect result from all loadgen and accumulate and send to appedo
        private void DoWork()
        {

            int totalCreated = 0;
            int totalCompleted = 0;
            int runcompleted = 0;
            XmlNodeList loadGens;
            StringBuilder scriptwisestatus = null;
            Dictionary<string, int> failedCount = new Dictionary<string, int>();
            bool isEnd = false;

            while (true)
            {
                try
                {
                    loadGens = _runNode.SelectNodes("loadgen");
                    if (isEnd == false) Thread.Sleep(5000);
                    scriptwisestatus = new StringBuilder();

                    //Get all loadgen one by one
                    foreach (XmlNode loadGen in loadGens)
                    {
                        try
                        {
                            #region Retrive Created & Completed UserCount
                            Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889");
                            loadGenConnection.Send(new TrasportData("scriptwisestatus", string.Empty, null));
                            TrasportData data = loadGenConnection.Receive();
                            totalCreated += Convert.ToInt32(data.Header["createduser"]);
                            totalCompleted += Convert.ToInt32(data.Header["completeduser"]);
                            runcompleted += Convert.ToInt32(data.Header["iscompleted"]);
                            scriptwisestatus.Append(data.DataStr);
                            //Successfully send means we need to remove from failed list
                            if (failedCount.ContainsKey(loadGen.Attributes["ipaddress"].Value) == true)
                            {
                                failedCount.Remove(loadGen.Attributes["ipaddress"].Value);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.Message);
                            Thread.Sleep(1000);
                            if (failedCount.ContainsKey(loadGen.Attributes["ipaddress"].Value) == false)
                            {
                                failedCount.Add(loadGen.Attributes["ipaddress"].Value, 1);
                            }
                            else
                            {
                                failedCount[loadGen.Attributes["ipaddress"].Value]++;
                            }
                            //If send attempt failed more than 2 times it will call failed url
                            if (failedCount[loadGen.Attributes["ipaddress"].Value] > 2)
                            {
                                runcompleted++;
                                ExceptionHandler.LogRunDetail(RunId, "Unable to connect " + loadGen.Attributes["ipaddress"].Value + " " + ex.Message);
                                Dictionary<string, string> header = new Dictionary<string, string>();
                                Trasport server = null;
                                try
                                {
                                    header["runid"] = RunId;
                                    header["ipaddress"] = loadGen.Attributes["ipaddress"].Value;
                                    server = new Trasport(_scriptWiserIP, Constants.GetInstance().AppedoPort, 20000);
                                    server.Send(new TrasportData("loadgenfailed", string.Empty, header));
                                }
                                catch (Exception ex1)
                                {
                                    ExceptionHandler.WritetoEventLog(ex1.StackTrace + ex1.Message);
                                }
                            }
                        }
                    }

                    _totalUserCreated = totalCreated;
                    _totalUserCompleted = totalCompleted;
                    //If run completed
                    if (runcompleted == loadGens.Count)
                    {
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        isEnd = true;
                    }
                    ScriptWiseStatus = GetStatusconcatenate(scriptwisestatus.ToString());
                    SendScriptWiseStatus(ScriptWiseStatus);
                    //If run completed
                    if (isEnd == true)
                    {
                        try
                        {
                            if (ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + RunId + "']") != null)
                            {
                                ControllerXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild(ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + RunId + "']"));
                                ControllerXml.GetInstance().Save();
                                ExceptionHandler.LogRunDetail(RunId, "node deleted successfully.");
                            }
                            DeleteReportFolder(RunId);
                            ExceptionHandler.RunDetaillog.Remove(RunId);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }
                        break;
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);   
                    try
                    {
                        Thread.Sleep(10000);
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        break;
                    }
                    catch (Exception ex1)
                    {
                        ExceptionHandler.WritetoEventLog(ex1.StackTrace + ex1.Message);                     
                    }

                }
                finally
                {
                    totalCreated = 0;
                    totalCompleted = 0;
                    runcompleted = 0;
                }
            }
        }

        //Consolidate result from different loadgens
        private string GetStatusconcatenate(string allStatus)
        {
            Dictionary<string, string> scriptWiseResult = new Dictionary<string, string>();
            StringBuilder summary = new StringBuilder();
            string[] result = null;
            try
            {
                foreach (string str in allStatus.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    if (str != string.Empty)
                    {
                        result = str.Split(',');
                        if (scriptWiseResult.ContainsKey(result[0]) == false)
                        {
                            scriptWiseResult.Add(result[0], str);
                        }
                        else
                        {
                            string[] source = scriptWiseResult[result[0]].Split(',');
                            string isComplete =
                            scriptWiseResult[result[0]] = new StringBuilder().AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n",
                                                                                            source[0],
                                                                                            Convert.ToInt32(source[1]) + Convert.ToInt32(result[1]),
                                                                                            Convert.ToInt32(source[2]) + Convert.ToInt32(result[2]),
                                                                                            Convert.ToInt32(source[3]) + Convert.ToInt32(result[3]),
                                                                                            Convert.ToInt32(source[4]) + Convert.ToInt32(result[4]),
                                                                                            Convert.ToInt32(source[5]) + Convert.ToInt32(result[5]),
                                                                                            Convert.ToInt32(source[6]) + Convert.ToInt32(result[6]),
                                                                                            source[7] == "1" && result[7] == "1" ? "1" : "0",
                                                                                            Convert.ToInt32(source[8]) + Convert.ToInt32(result[8]),
                                                                                            source[9]
                                                                                          ).ToString();
                        }
                    }
                }
                foreach (string key in scriptWiseResult.Keys)
                {
                    summary.AppendLine(scriptWiseResult[key]);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }

            return summary.ToString();
        }

        //Send consolidated result to appedo.
        private void SendScriptWiseStatus(string scriptWiseStatus)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            Trasport server = null;
            TrasportData data = null;
            try
            {
                header["runid"] = RunId;
                header["iscompleted"] = (_staus == ControllerStatus.ReportGenerateCompleted) ? "1" : "0";

                server = new Trasport(_scriptWiserIP, _scriptWiserPort, 20000);
                data = new TrasportData("scriptwisestatus", scriptWiseStatus, header);
                server.Send(data);
                data = server.Receive();
                LastSentStatus = scriptWiseStatus;
            }
            catch (Exception ex)
            {
                _failedScriptWaseSend++;
                if (_failedScriptWaseSend == 5)
                {
                    _failedScriptWaseSend = 0;
                    try
                    {
                        if (_appedoFailedUrl != string.Empty)
                        {
                            _constants.GetPageContent(_appedoFailedUrl);
                        }
                    }
                    catch (Exception ex1)
                    {
                        ExceptionHandler.WritetoEventLog(ex1.StackTrace + Environment.NewLine + ex1.Message);
                    }
                }
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                server = null;
                header = null;
                data = null;
            }
        }

    }

    //Current running scenarios status list
    public enum ControllerStatus { Idle = 0, Running, RunCompleted, ReportGenerating, ReportGenerateCompleted }

}
