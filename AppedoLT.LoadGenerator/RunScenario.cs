using AppedoLT.BusinessLogic;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;

namespace AppedoLTLoadGenerator
{
    public class RunScenario
    {
        List<ScriptExecutor> _scriptExecutorList = new List<ScriptExecutor>();
        string _scenarioXml;
        public int TotalUserCreated { get; set; }
        public int TotalUserComplted { get; set; }

        private System.Timers.Timer _statusUpdateTimer;
        private Constants _constants = Constants.GetInstance();
        private ExecutionReport executionReport = ExecutionReport.GetInstance();
        private int _tempCreatedUser = 0;
        private int _tempCompletedUser = 0;
        private DataServer _dataServer = DataServer.GetInstance();
        private string _distribution = string.Empty;

        public int IsRunCompleted
        {
            get
            {
                if ((_scriptExecutorList.Count == 0 
                      || _scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count) 
                    && executionReport.ExecutionStatus == Status.Completed)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public RunScenario(string scenarioXml, string distribution)
        {
            _scenarioXml = scenarioXml;
            _distribution = distribution;
            _statusUpdateTimer = new System.Timers.Timer(1000);
            _statusUpdateTimer.Enabled = true;
            _statusUpdateTimer.Elapsed += new ElapsedEventHandler(StatusUpdateTimer_Tick);
        }

        void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(1000);
                _tempCreatedUser = 0;
                _tempCompletedUser = 0;
                foreach (ScriptExecutor scripts in _scriptExecutorList)
                {
                    _tempCreatedUser += scripts.StatusSummary.TotalVUserCreated;
                    _tempCompletedUser += scripts.StatusSummary.TotalVUserCompleted;
                }

                TotalUserCreated = _tempCreatedUser;
                TotalUserComplted = _tempCompletedUser;

                if (_scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count && _tempCreatedUser != 0 && _tempCreatedUser == _tempCompletedUser)
                {
                    try
                    {
                        _statusUpdateTimer.Stop();
                        for (int index = 0; index < 9; index++)
                        {
                            if ((_dataServer.reportDataFile != null
                                     && _dataServer.reportDataFile.BaseStream != null
                                     && _dataServer.reportDataFile.BaseStream.Length > 0)

                                || (_dataServer.transactionFile != null
                                     && _dataServer.transactionFile.BaseStream != null
                                     && _dataServer.transactionFile.BaseStream.Length > 0)

                                || (_dataServer.errorFile != null
                                    && _dataServer.errorFile.BaseStream != null
                                    && _dataServer.errorFile.BaseStream.Length > 0)

                                || (_dataServer.reportDT.Count > 0
                                    || _dataServer.transcations.Count > 0
                                    || _dataServer.errors.Count > 0
                                    || _dataServer.logs.Count > 0))
                            {
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                break;
                            }
                        }
                        Thread.Sleep(7000);
                        lock (_dataServer.DataBaseLock)
                        {
                            ReportMaster rm = new ReportMaster(executionReport.ReportName, executionReport.StartTime, executionReport.LoadGenName);
                            rm.SetUserRunTime();
                            executionReport.ExecutionStatus = Status.Completed;
                            rm.SetChartSummary();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                    finally
                    {
                        executionReport.ExecutionStatus = Status.Completed;
                    }
                }

            }
            catch (SocketException ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public bool Start()
        {
            XmlDocument scenario = new XmlDocument();
            scenario.LoadXml(_scenarioXml);
            Request.IPSpoofingEnabled = Convert.ToBoolean(scenario.SelectSingleNode("//root/scenario").Attributes["enableipspoofing"].Value);
            VariableManager.dataCenter = new VariableManager(scenario.SelectSingleNode("//root/variables"));
            DataServer.GetInstance().ClearData();

            foreach (XmlNode script in scenario.SelectNodes("//script"))
            {
                string scriptid = script.Attributes["id"].Value;
                XmlNode setting = script.SelectNodes("//script[@id='" + scriptid + "']//setting")[0];
                XmlNode vuscript = script.SelectNodes("//script[@id='" + scriptid + "']//vuscript")[0];
                ScriptExecutor scriptRunnerSce = new ScriptExecutor(setting, vuscript, executionReport.ReportName,_distribution);
                if (scriptRunnerSce.StartUserId > 0)
                {
                    _scriptExecutorList.Add(scriptRunnerSce);
                }
            }
            executionReport.StartTime = DateTime.Now;

            foreach (ScriptExecutor scr in _scriptExecutorList)
            {
                scr.Run();
            }
            if (_scriptExecutorList.Count > 0)
            {
                _statusUpdateTimer.Start();
                return true;
            }
            else
            {
                executionReport.ExecutionStatus = Status.Completed;
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                executionReport.ExecutionStatus = Status.Completed;
                foreach (ScriptExecutor scr in _scriptExecutorList)
                {
                    new Thread(() => { scr.Stop(); }).Start();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public string GetStatus()
        {
            StringBuilder status = new StringBuilder();
            try
            {
                foreach (ScriptExecutor scripts in _scriptExecutorList)
                {
                    status.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", scripts.Scriptid, scripts.StatusSummary.TotalVUserCreated, scripts.StatusSummary.TotalVUserCompleted, scripts.StatusSummary.TotalTwoHundredStatusCodeCount, scripts.StatusSummary.TotalThreeHundredStatusCodeCount, scripts.StatusSummary.TotalFourHundredStatusCodeCount, scripts.StatusSummary.TotalFiveHundredStatusCodeCount, Convert.ToInt16(scripts.IsRunCompleted), scripts.StatusSummary.TotalErrorCount, scripts.Scriptname));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return status.ToString();
        }
    }
}
