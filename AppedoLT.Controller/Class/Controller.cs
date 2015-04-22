using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace AppedoLTController
{
    class Controller
    {
        public static Dictionary<string, Controller> Controllers = new Dictionary<string, Controller>();

        Thread _CollectorThread = null;
        Constants _constants = Constants.GetInstance();
        private XmlNode _RunNode = null;
        private string _SourceIp = null;
        ControllerStatus _staus = ControllerStatus.Idle;
        public string ScriptWiseStatus { get; set; }
        public string RunId = string.Empty;
        public ControllerStatus Status { get { return _staus; } }
        public string LastSentStatus = string.Empty;
        public LoadGenRunningStatusData _runningStatusData = new LoadGenRunningStatusData();
        int _reportDataCount = 0;

        public int ReportDataCount
        {
            get { return _reportDataCount; }
            set { _reportDataCount = value; }
        }

        public LoadGenRunningStatusData RunningStatusData
        {
            get
            {
                _runningStatusData.IsCompleted = (_staus == ControllerStatus.ReportGenerateCompleted) ? 1 : 0;
                return _runningStatusData;
            }
        }

        public Controller(string soureIP, string runid, XmlNode runNode, string loadgens)
        {
            RunId = runid;
            _SourceIp = soureIP;
            _RunNode = runNode;
            _runningStatusData.LoadGens = loadgens;
        }

        public void Start()
        {
            _CollectorThread = new Thread(new ThreadStart(DoWork));
            _CollectorThread.Start();
            _staus = ControllerStatus.Running;
        }
        public void Stop()
        {
            ExceptionHandler.LogRunDetail(RunId, "Stop request received ");
            foreach (XmlNode loadGen in _RunNode.SelectNodes("loadgen"))
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
                    loadGens = _RunNode.SelectNodes("loadgen");
                    if (isEnd == false) Thread.Sleep(5000);
                    scriptwisestatus = new StringBuilder();
                    foreach (XmlNode loadGen in loadGens)
                    {
                        try
                        {
                            #region Retrive Created & Completed UserCount
                            Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889", 20000);
                            loadGenConnection.Send(new TrasportData("status", string.Empty, null));
                            TrasportData data = loadGenConnection.Receive();
                            LoadGenRunningStatusData status = Constants.GetInstance().Deserialise<LoadGenRunningStatusData>(data.DataStr);
                            totalCreated += status.CreatedUser;
                            totalCompleted += status.CompletedUser;
                            runcompleted += status.IsCompleted;
                            _runningStatusData.Log.AddRange(status.Log);
                            _runningStatusData.Error.AddRange(status.Error);
                            _reportDataCount += status.ReportData.Count;
                            _runningStatusData.ReportData.AddRange(status.ReportData);
                            _runningStatusData.Transactions.AddRange(status.Transactions);
                            _runningStatusData.UserDetailData.AddRange(status.UserDetailData);
                            loadGenConnection.Send(new TrasportData("ok", string.Empty, null));
                            loadGenConnection.Close();
                            loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889");
                            loadGenConnection.Send(new TrasportData("scriptwisestatus", string.Empty, null));
                            scriptwisestatus.Append(loadGenConnection.Receive().DataStr);

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
                            if (failedCount[loadGen.Attributes["ipaddress"].Value] > 5)
                            {
                                runcompleted++;
                                ExceptionHandler.LogRunDetail(RunId, "Unable to connect " + loadGen.Attributes["ipaddress"].Value + " " + ex.Message);
                            }
                        }
                    }
                    _runningStatusData.Time = DateTime.Now;
                    _runningStatusData.CreatedUser = totalCreated;
                    _runningStatusData.CompletedUser = totalCompleted;
                    ScriptWiseStatus = GetStatusconcatenate(scriptwisestatus.ToString());
                    SendScriptWiseStatus(ScriptWiseStatus);
                    SendStatus();

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
                    if (runcompleted == loadGens.Count)
                    {
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        isEnd = true;
                    }

                }
                catch (Exception ex)
                {
                    try
                    {
                        Thread.Sleep(10000);
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        SendStatus();
                        break;
                    }
                    catch (Exception ex1)
                    {
                        ExceptionHandler.WritetoEventLog(ex1.StackTrace + ex1.Message);
                        //break;
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
        private string GetStatusconcatenate(string allStatus)
        {
            Dictionary<string, string> scriptWiseResult = new Dictionary<string, string>();
            string[] result = null;
            StringBuilder summary = new StringBuilder();
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
                            scriptWiseResult[result[0]] = new StringBuilder().AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n", source[0],
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
        private void SendScriptWiseStatus(string scriptWiseStatus)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            Trasport server = null;
            TrasportData data = null;
            try
            {
                header["runid"] = RunId;
                server = new Trasport(_SourceIp, Constants.GetInstance().AppedoPort);
                data = new TrasportData("scriptwisestatus", scriptWiseStatus, header);
                server.Send(data);
                data = server.Receive();
                LastSentStatus = scriptWiseStatus;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                server = null;
                header = null;
                data = null;
            }
        }
        private void SendStatus()
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            Trasport server = null;
            TrasportData data = null;
            try
            {
                header["runid"] = RunId;
                server = new Trasport(_SourceIp, Constants.GetInstance().AppedoPort);
                data = new TrasportData("status", ASCIIEncoding.Default.GetString(Constants.GetInstance().Serialise(RunningStatusData)), header);
                server.Send(data);
                data = server.Receive();
                if (data.Operation == "ok")
                {
                    
                    _runningStatusData.Log.Clear();
                    _runningStatusData.Error.Clear();
                    _runningStatusData.ReportData.Clear();
                    _runningStatusData.Transactions.Clear();
                    _runningStatusData.UserDetailData.Clear();
                }
            }
            catch (Exception ex)
            {
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
    public enum ControllerStatus { Idle = 0, Running, RunCompleted, ReportGenerating, ReportGenerateCompleted }

}
