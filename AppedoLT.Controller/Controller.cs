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
        
        Thread _CollectorThread = null;
        Constants _constants = Constants.GetInstance();
        private XmlNode _RunNode = null;
        private string _SourceIp = null;
        ControllerStatus _staus = ControllerStatus.Idle;
<<<<<<< HEAD

        public int CreatedUser = 0;
        public int CompletedUser = 0;
        public string ScriptWiseStatus { get; set; }
        public string RunId = string.Empty;
        public ControllerStatus Status { get { return _staus; } }
        public static Dictionary<string, Controller> Controllers = new Dictionary<string, Controller>();
=======
             
        public string ScriptWiseStatus { get; set; }
        public string RunId = string.Empty;
        public ControllerStatus Status { get { return _staus; } }
        public string LastSentStatus = string.Empty;
        public LoadGenRunningStatusData _runningStatusData = new LoadGenRunningStatusData();
        public LoadGenRunningStatusData RunningStatusData
        {
            get
            {
                _runningStatusData.IsCompleted = (_staus== ControllerStatus.ReportGenerateCompleted) ? 1 : 0;
                return _runningStatusData;
            }
        }
>>>>>>> dev_master

        public Controller(string soureIP, string runid, XmlNode runNode)
        {
            RunId = runid;
            _SourceIp = soureIP;
            _RunNode = runNode;
        }

        public void Start()
        {
            _CollectorThread = new Thread(new ThreadStart(DoWork));
            _CollectorThread.Start();
            _staus = ControllerStatus.Running;
        }

        public void Stop()
        {
            foreach (XmlNode loadGen in _RunNode.SelectNodes("loadgen"))
            {
                try
                {
<<<<<<< HEAD
                    #region Retrive Created & Completed UserCount
                    Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889");
                    loadGenConnection.Send(new TrasportData("stop", string.Empty, null));
                    TrasportData data = loadGenConnection.Receive();
                    loadGenConnection.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
=======
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
>>>>>>> dev_master
                }
            }
        }

        private void DoWork()
        {
            
            int totalCreated = 0;
            int totalCompleted = 0;
            int runcompleted = 0;
            XmlNodeList loadGens;
            StringBuilder scriptwisestatus = null;
<<<<<<< HEAD

=======
            Dictionary<string, int> failedCount = new Dictionary<string, int>();
           
>>>>>>> dev_master
            while (true)
            {
                try
                {
                    loadGens = _RunNode.SelectNodes("loadgen");
                    Thread.Sleep(5000);
                   
                    scriptwisestatus = new StringBuilder();
                    foreach (XmlNode loadGen in loadGens)
                    {
                        try
                        {
                            #region Retrive Created & Completed UserCount
                            Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889", 60000);
                            loadGenConnection.Send(new TrasportData("status", string.Empty, null));
                            TrasportData data = loadGenConnection.Receive();
                            loadGenConnection.Send(new TrasportData("ok", string.Empty, null));
                            loadGenConnection.Close();
                            LoadGenRunningStatusData status= Constants.GetInstance().Deserialise<LoadGenRunningStatusData>(data.DataStr);
                            totalCreated += status.CreatedUser;
                            totalCompleted += status.CompletedUser;
                            runcompleted += status.IsCompleted;
                            _runningStatusData.Log.AddRange(status.Log);
                            _runningStatusData.Error.AddRange(status.Error);
                            _runningStatusData.ReportData.AddRange(status.ReportData);
                            _runningStatusData.Transactions.AddRange(status.Transactions);
                            loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889");
                            loadGenConnection.Send(new TrasportData("scriptwisestatus", string.Empty, null));
                            scriptwisestatus.Append(loadGenConnection.Receive().DataStr);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            runcompleted++;
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }
                    }
                    ScriptWiseStatus = GetStatusconcatenate(scriptwisestatus.ToString());
                    SendScriptWiseStatus(ScriptWiseStatus);
                    SendStatus();
                    _runningStatusData.CreatedUser=totalCreated;
                    _runningStatusData.CompletedUser=totalCompleted;
                    if (runcompleted == loadGens.Count)
                    {
                        Thread.Sleep(10000);
                        _staus = ControllerStatus.RunCompleted;
<<<<<<< HEAD
=======
                        _runningStatusData.IsCompleted = 1;
                        ExceptionHandler.LogRunDetail(RunId, "Report is generating");
>>>>>>> dev_master
                        new ResultFileGenerator(RunId).Genarate();
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        Controllers.Remove(RunId);
                        break;
                        
                    }

                }
                catch (Exception ex)
                {
                    try
                    {
                        Thread.Sleep(10000);
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message + Environment.NewLine + ControllerXml.GetInstance().doc.InnerText);
                        _staus = ControllerStatus.RunCompleted;
                        new ResultFileGenerator(RunId).Genarate();
                        _staus = ControllerStatus.ReportGenerateCompleted;
                        break;
                    }
                    catch (Exception ex1)
                    {
                        ExceptionHandler.WritetoEventLog(ex1.StackTrace + ex1.Message);
                        break;
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
                data = new TrasportData("status",ASCIIEncoding.Default.GetString(Constants.GetInstance().Serialise( _runningStatusData)), header);
                server.Send(data);
                data = server.Receive();
                _runningStatusData.Log.Clear();
                _runningStatusData.Error.Clear();
                _runningStatusData.ReportData.Clear();
                _runningStatusData.Transactions.Clear();
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
