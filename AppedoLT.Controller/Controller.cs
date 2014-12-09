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
        public int CreatedUser = 0;
        public int CompletedUser = 0;
        public string ScriptWiseStatus { get; set; }
        Constants _constants = Constants.GetInstance();

        public string ReportName = string.Empty;

        ControllerStatus _staus = ControllerStatus.Idle;
        public ControllerStatus Status { get { return _staus; } }

        public Controller(string soureIP, string reportName)
        {
            ReportName = reportName;
        }

        public void Start()
        {
            _CollectorThread = new Thread(new ThreadStart(DoWork));
            _CollectorThread.Start();
            _staus = ControllerStatus.Running;
        }

        public void Stop()
        {
            foreach (XmlNode loadGen in ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + ReportName + "']").SelectNodes("loadgen"))
            {
                try
                {
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
                }
            }
        }

        private void DoWork()
        {
            Regex log = new Regex("createduser: ([0-9]*)\r\ncompleteduser: ([0-9]*)\r\niscompleted: ([0-9]*)");
            int totalCreated = 0;
            int totalCompleted = 0;
            int runcompleted = 0;
            XmlNodeList loadGens;
            StringBuilder scriptwisestatus = null;

            while (true)
            {
                try
                {
                    loadGens = ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + ReportName + "']").SelectNodes("loadgen");
                    Thread.Sleep(5000);
                    scriptwisestatus = new StringBuilder();
                    foreach (XmlNode loadGen in loadGens)
                    {
                        try
                        {
                            #region Retrive Created & Completed UserCount
                            Trasport loadGenConnection = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889", 10000);
                            loadGenConnection.Send(new TrasportData("status", string.Empty, null));
                            TrasportData data = loadGenConnection.Receive();
                            string dataStr = data.DataStr;
                            totalCreated += Convert.ToInt32(log.Match(dataStr).Groups[1].Value);
                            totalCompleted += Convert.ToInt32(log.Match(dataStr).Groups[2].Value);
                            loadGenConnection.Close();
                            runcompleted += Convert.ToInt32(log.Match(dataStr).Groups[3].Value);

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
                    CreatedUser = totalCreated;
                    CompletedUser = totalCompleted;
                    if (runcompleted == loadGens.Count)
                    {
                        Thread.Sleep(10000);
                        _staus = ControllerStatus.RunCompleted;
                        new ResultFileGenerator(ReportName).Genarate();
                        _staus = ControllerStatus.ReportGenerateCompleted;
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
                        new ResultFileGenerator(ReportName).Genarate();
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

    }
    public enum ControllerStatus { Idle = 0, Running, RunCompleted, ReportGenerating, ReportGenerateCompleted }

}
