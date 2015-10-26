using AppedoLT.BusinessLogic;
using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;

namespace AppedoLTLoadGenerator
{
    /// <summary>
    /// 
    /// It is used to run scenario, collect data and send to appedo server.
    /// 
    /// prerequisites: 
    ///  runid- Runid 
    ///  appedoIP- To send data
    ///  appedoPort- To send data
    ///  scenarioXml- Scenario need to be execute
    ///  distribution- Distribution detail.
    ///  appedoFailedUrl- Send failed notification to this url
    ///  monitorCounter- Loadgen monitor counters
    /// 
    /// Author: Rasith
    /// </summary>
    public class RunScenario
    {
        private List<ScriptExecutor> _scriptExecutorList = new List<ScriptExecutor>();
        private string _scenarioXml;
        private int _tempCreatedUser = 0;
        private int _tempCompletedUser = 0;
        private string _distribution = string.Empty;
        private System.Timers.Timer _statusUpdateTimer;
        private int _totalCreatedUser = 0;
        private int _totalCompleted = 0;
        private int _isCompleted = 0;
        private LoadGenRunningStatusData _runningStatusData = new LoadGenRunningStatusData();
        private ExecutionReport executionReport = ExecutionReport.GetInstance();
        private Constants _constants = Constants.GetInstance();

        private LoadGenRunningStatusData _faildData = null;
        private string _runid = string.Empty;
        private string _appedoIp = string.Empty;
        private string _appedoPort = string.Empty;
        private string _appedoFailedUrl = string.Empty;
        private int _dataSendFailedCount = 0;
        public int TotalCreatedUser { get { return _totalCreatedUser; } private set { } }
        public int TotalCompletedUser { get { return _totalCompleted; } private set { } }
        public int IsCompleted { get { return _isCompleted; } private set { } }
        private DataXml _dataXml = DataXml.GetInstance();
        private Dictionary<string, string> _header = new Dictionary<string, string>();
        private int _currentUserMonierId = 0;
        private StatusData<Log> _logBuf = new StatusData<Log>();
        private StatusData<RequestException> _errorBuf = new StatusData<RequestException>();
        private StatusData<ReportData> _reportDataBuf = new StatusData<ReportData>();
        private StatusData<TransactionRunTimeDetail> _TransactionDataBuf = new StatusData<TransactionRunTimeDetail>();
        private StatusData<UserDetail> _userDetailBuf = new StatusData<UserDetail>();
        private StatusData<LoadGenMonitor> _loadGenMonitorBuf = new StatusData<LoadGenMonitor>();

        Dictionary<int, PerformanceCounter> CountersAllInstance = new Dictionary<int, PerformanceCounter>();

        public RunScenario(string runid, string appedoIP, string appedoPort, string scenarioXml, string distribution, string appedoFailedUrl, string monitorCounter)
        {
            _runid = runid;
            _loadGenMonitorBuf.Runid = _logBuf.Runid = _errorBuf.Runid = _reportDataBuf.Runid = _TransactionDataBuf.Runid = _userDetailBuf.Runid = runid;
            _logBuf.Type = "log";
            _errorBuf.Type = "error";
            _reportDataBuf.Type = "reporddata";
            _TransactionDataBuf.Type = "transactions";
            _userDetailBuf.Type = "userdetail";
            _loadGenMonitorBuf.Type = "loadgenstatus";
            _header.Add("runid", runid);
            _header.Add("queuename", "ltreport");
            _appedoFailedUrl = appedoFailedUrl;
            _appedoIp = appedoIP;
            _appedoPort = appedoPort;
            _scenarioXml = scenarioXml;
            _distribution = distribution;
            _statusUpdateTimer = new System.Timers.Timer(1000);
            _statusUpdateTimer.Enabled = true;
            _statusUpdateTimer.Elapsed += new ElapsedEventHandler(StatusUpdateTimer_Tick);
            ConfigMoniter(monitorCounter);
        }

        //Config monitor conunters for loadgen.
        void ConfigMoniter(string json)
        {
            try
            {
                System.Collections.Generic.List<MonitorCounter> obj = new System.Collections.Generic.List<MonitorCounter>();
                obj = Constants.GetInstance().Deserialise<System.Collections.Generic.List<MonitorCounter>>(json);
                foreach (MonitorCounter counter in obj)
                {
                    try
                    {
                        if (counter.CounterName != "Current User count")
                        {
                            PerformanceCounter count = null;
                            //If Counters is parent counter. 
                            if (counter.HasInstance == true)
                            {
                                count = new PerformanceCounter(counter.CategoryName, counter.CounterName, counter.InstanceName);
                            }
                            else
                            {
                                count = new PerformanceCounter(counter.CategoryName, counter.CounterName);
                            }
                            count.NextValue();
                            CountersAllInstance.Add(counter.CounterId, count);
                        }
                        else
                        {
                            _currentUserMonierId = counter.CounterId;
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
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //It will calculate total user created and total user completed for every 5sec
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
                _totalCreatedUser = _tempCreatedUser;
                _totalCompleted = _tempCompletedUser;

                //If run completed
                if (_scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count && _tempCreatedUser != 0 && _tempCreatedUser == _tempCompletedUser)
                {
                    try
                    {
                        _statusUpdateTimer.Stop();
                        for (int index = 0; index < 9; index++)
                        {
                            //If there is any unsent data
                            if (_reportDataBuf.Data.Count > 0 || _TransactionDataBuf.Data.Count > 0 || _logBuf.Data.Count > 0 || _errorBuf.Data.Count > 0 || _userDetailBuf.Data.Count > 0)
                            {
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                break;
                            }
                        }
                        Thread.Sleep(7000);
                        executionReport.ExecutionStatus = Status.Completed;
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

        //To start run.
        public bool Start()
        {
            XmlDocument scenario = new XmlDocument();
            scenario.LoadXml(_scenarioXml);
            Request.IPSpoofingEnabled = Convert.ToBoolean(scenario.SelectSingleNode("//root/scenario").Attributes["enableipspoofing"].Value);
            VariableManager.dataCenter = new VariableManager(scenario.SelectSingleNode("//root/variables"));
            ClearData();

            foreach (XmlNode script in scenario.SelectNodes("//script"))
            {
                string scriptid = script.Attributes["id"].Value;
                XmlNode setting = script.SelectNodes("//script[@id='" + scriptid + "']//setting")[0];
                XmlNode vuscript = script.SelectNodes("//script[@id='" + scriptid + "']//vuscript")[0];
                ScriptExecutor scriptRunnerSce = new ScriptExecutor(setting, vuscript, executionReport.ReportName, _distribution);
                //After distribution calculated, If StartUserId is greater than 0.
                if (scriptRunnerSce.StartUserId > 0)
                {
                    _scriptExecutorList.Add(scriptRunnerSce);
                }
            }
            executionReport.StartTime = DateTime.Now;

            //We need to map each script result to corresponding method.
            foreach (ScriptExecutor scr in _scriptExecutorList)
            {
                //if script has report data, it will call scr_OnLockReportData
                scr.OnLockReportData += scr_OnLockReportData;
                //if script has error data, it will call scr_OnLockError
                scr.OnLockError += scr_OnLockError;
                //if script has log data, it will call scr_OnLockLog
                scr.OnLockLog += scr_OnLockLog;
                //if script has Transactions data, it will call scr_OnLockTransactions
                scr.OnLockTransactions += scr_OnLockTransactions;
                //if script has UserDetail data, it will call scr_OnLockUserDetail
                scr.OnLockUserDetail += scr_OnLockUserDetail;
                scr.Run();
            }
            if (_scriptExecutorList.Count > 0)
            {
                _statusUpdateTimer.Start();
                //To send status.
                SendData();
                return true;
            }
            else
            {
                executionReport.ExecutionStatus = Status.Completed;
                return false;
            }
        }

        //Clear all queue items
        private void ClearData()
        {
            try
            {
                _reportDataBuf.Data.Clear();
                _TransactionDataBuf.Data.Clear();
                _logBuf.Data.Clear();
                _errorBuf.Data.Clear();
                _userDetailBuf.Data.Clear();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //Store UserDetail into queue
        void scr_OnLockUserDetail(UserDetail data)
        {
            lock (_userDetailBuf)
            {
                if (data != null)
                {
                    _userDetailBuf.Data.Add(data);
                }
            }
        }

        //Store TransactionRunTimeDetail into queue
        void scr_OnLockTransactions(TransactionRunTimeDetail data)
        {
            lock (_TransactionDataBuf)
            {
                if (data != null)
                {
                    _TransactionDataBuf.Data.Add(data);
                }
            }
        }

        //Store Log into queue
        void scr_OnLockLog(Log data)
        {
            lock (_logBuf)
            {
                if (data != null)
                {
                    _logBuf.Data.Add(data);
                }
            }
        }

        //Store RequestException into queue
        void scr_OnLockError(RequestException data)
        {
            lock (_errorBuf)
            {
                if (data != null)
                {
                    _errorBuf.Data.Add(data);
                }
            }
        }

        //Store ReportData into queue
        void scr_OnLockReportData(ReportData data)
        {
            lock (_reportDataBuf)
            {
                if (data != null)
                {
                    _reportDataBuf.Data.Add(data);
                }
            }
        }

        //To stop execution.
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

        //Get script wise status
        public string GetStatus()
        {
            StringBuilder status = new StringBuilder();
            try
            {
                foreach (ScriptExecutor scripts in _scriptExecutorList)
                {
                    status.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", scripts.StatusSummary.ScriptId, scripts.StatusSummary.TotalVUserCreated, scripts.StatusSummary.TotalVUserCompleted, scripts.StatusSummary.TotalTwoHundredStatusCodeCount, scripts.StatusSummary.TotalThreeHundredStatusCodeCount, scripts.StatusSummary.TotalFourHundredStatusCodeCount, scripts.StatusSummary.TotalFiveHundredStatusCodeCount, Convert.ToInt16(scripts.IsRunCompleted), scripts.StatusSummary.TotalErrorCount, scripts.StatusSummary.ScriptName));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return status.ToString();
        }

        //To send all data to appedo server.
        private void SendData()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] dataBuf;
                        bool hasData = false;

                        #region Report data
                        try
                        {
                            if (_reportDataBuf.Data.Count > 0)
                            {
                                hasData = true;
                                lock (_reportDataBuf)
                                {
                                    dataBuf = _constants.Serialise(_reportDataBuf);
                                    _reportDataBuf.Data.Clear();
                                }
                                Send(dataBuf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region send Log
                        try
                        {
                            if (_logBuf.Data.Count > 0)
                            {
                                hasData = true;
                                lock (_logBuf)
                                {
                                    dataBuf = _constants.Serialise(_logBuf);
                                    _logBuf.Data.Clear();
                                }
                                Send(dataBuf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region Send error
                        try
                        {
                            if (_errorBuf.Data.Count > 0)
                            {
                                hasData = true;
                                lock (_errorBuf)
                                {
                                    dataBuf = _constants.Serialise(_errorBuf);
                                    _errorBuf.Data.Clear();
                                }
                                Send(dataBuf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region transaction
                        try
                        {
                            if (_TransactionDataBuf.Data.Count > 0)
                            {
                                hasData = true;
                                lock (_TransactionDataBuf)
                                {
                                    dataBuf = _constants.Serialise(_TransactionDataBuf);
                                    _TransactionDataBuf.Data.Clear();
                                }
                                Send(dataBuf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region Userdetail
                        try
                        {
                            if (_userDetailBuf.Data.Count > 0)
                            {
                                hasData = true;
                                lock (_userDetailBuf)
                                {
                                    dataBuf = _constants.Serialise(_userDetailBuf);
                                    _userDetailBuf.Data.Clear();
                                }
                                Send(dataBuf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region LoadGenMontor
                      
                        try
                        {
                            if (CountersAllInstance.Count > 0)
                            {
                                UpdateCounter();
                                if (_loadGenMonitorBuf.Data.Count > 0)
                                {
                                    lock (_loadGenMonitorBuf)
                                    {
                                        dataBuf = _constants.Serialise(_loadGenMonitorBuf);
                                        _loadGenMonitorBuf.Data.Clear();
                                    }
                                    Send(dataBuf);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        #endregion

                        #region After completed
                        if (hasData == false
                            && (_scriptExecutorList.Count == 0
                                || _scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count)
                            && executionReport.ExecutionStatus == Status.Completed)
                        {
                            int count = 0;
                            while (true)
                            {
                                if (count > 20) break;
                                if (_dataXml.doc.SelectSingleNode("/root/data[@runid='" + _runid + "']") == null)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                    Thread.Sleep(5000);
                                }
                            }
                            _isCompleted = 1;
                            break;
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    finally { Thread.Sleep(5000); }
                }

            }).Start();
        }

        //Send data to appedo server
        private void Send(byte[] dataObj)
        {
            TrasportData data = null;
            try
            {
                data = new TrasportData("status", dataObj, _header);
                Trasport trasport = new Trasport(_appedoIp, _appedoPort, 30000);
                trasport.Send(data);

                TrasportData ack = trasport.Receive();
                if (ack.Operation == "ok")
                {
                    _faildData = null;
                }
                trasport.Close();
                trasport = null;
            }
            catch (Exception ex1)
            {
                //Failed data. We keep failed data into file. Next time sending status, Failed data will be send.
                try
                {
                    string path = ExceptionHandler.WriteReportData(DateTime.Now.Ticks.ToString(), data.DataBytes);
                    if (path != string.Empty)
                    {
                        _dataXml.doc.SelectSingleNode("root").AppendChild(_dataXml.CreateData(_runid, _appedoIp, _appedoPort, path));
                        _faildData = null;
                        _dataXml.Save();
                    }
                }
                catch (Exception ex3)
                {
                    ExceptionHandler.WritetoEventLog(ex3.StackTrace + Environment.NewLine + ex3.Message);
                }

                _dataSendFailedCount++;
                if (_dataSendFailedCount == 3)
                {
                    _dataSendFailedCount = 0;
                    try
                    {
                        if (_appedoFailedUrl != string.Empty)
                        {
                            _constants.GetPageContent(_appedoFailedUrl);
                        }
                    }
                    catch (Exception ex2)
                    {
                        ExceptionHandler.WritetoEventLog(ex2.StackTrace + Environment.NewLine + ex2.Message);
                    }
                }
                ExceptionHandler.WritetoEventLog(ex1.StackTrace + Environment.NewLine + ex1.Message);
            }

        }

        private void UpdateCounter()
        {
            LoadGenMonitor mon = null;
            try
            {
                foreach (int key in CountersAllInstance.Keys)
                {
                    try
                    {
                        mon = _loadGenMonitorBuf.Data.Find(f => f.counter_id == key);
                        if (mon == null)
                        {
                            mon = new LoadGenMonitor();
                            mon.counter_id = key;
                            mon.loadgenanme = executionReport.LoadGenName;
                            _loadGenMonitorBuf.Data.Add(mon);
                        }
                        mon.counter_value =Convert.ToDecimal(CountersAllInstance[key].NextValue());
                        mon.received_on = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                }
                mon = _loadGenMonitorBuf.Data.Find(f => f.counter_id == _currentUserMonierId);
                if (mon == null)
                {
                    mon = new LoadGenMonitor();
                    mon.counter_id = _currentUserMonierId;
                    mon.loadgenanme = executionReport.LoadGenName;
                    _loadGenMonitorBuf.Data.Add(mon);
                }
                mon.counter_value = _totalCreatedUser - _totalCompleted;
                mon.received_on = DateTime.Now;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
