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
using System.Configuration;
using System.Messaging;
using log4net;

namespace AppedoLTLoadGenerator
{
    public class RunScenario
    {
        private static readonly ILog logger = LogManager.GetLogger("LoadGenerator");
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

        bool _logResponseData;
        bool _logVariableData;
        bool _stopRunning;

        // Response data queue
        private Queue<ResponseDetail> _responseDetailQueue = new Queue<ResponseDetail>();
        private object _responseDetailSyncObj = new object();

        // Variable data queue
        private Queue<VariableDetail> _variableDetailQueue = new Queue<VariableDetail>();
        private object _variableDetailSyncObj = new object();

        private StatusData<Log> _logBuf = new StatusData<Log>();
        private StatusData<RequestException> _errorBuf = new StatusData<RequestException>();
        private StatusData<ReportData> _reportDataBuf = new StatusData<ReportData>();
        private StatusData<TransactionRunTimeDetail> _TransactionDataBuf = new StatusData<TransactionRunTimeDetail>();
        private StatusData<UserDetail> _userDetailBuf = new StatusData<UserDetail>();
        private StatusData<LoadGenMonitor> _loadGenMonitorBuf = new StatusData<LoadGenMonitor>();

        Dictionary<int, PerformanceCounter> CountersAllInstance = new Dictionary<int, PerformanceCounter>();

        public RunScenario(string runid, string appedoIP, string appedoPort, string scenarioXml, string distribution, string appedoFailedUrl, string monitorCounter = null)
        {
            _runid = runid;
            _loadGenMonitorBuf.Runid = _logBuf.Runid = _errorBuf.Runid = _reportDataBuf.Runid = _TransactionDataBuf.Runid = _userDetailBuf.Runid = runid;

            _loadGenMonitorBuf.Type = "loadgenstatus";
            _logBuf.Type = "log";
            _errorBuf.Type = "error";
            _reportDataBuf.Type = "reporddata";
            _TransactionDataBuf.Type = "transactions";
            _userDetailBuf.Type = "userdetail";
            _header.Add("runid", runid);
            _header.Add("queuename", "ltreport");
            _appedoFailedUrl = appedoFailedUrl;
            _appedoIp = appedoIP;
            _appedoPort = appedoPort;
            _scenarioXml = scenarioXml;
            _distribution = distribution;

            _logResponseData = false;
            _logVariableData = false;

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogResponseData"]))
            {
                bool.TryParse(ConfigurationManager.AppSettings["LogResponseData"].ToString(), out _logResponseData);
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogVariableData"]))
            {
                bool.TryParse(ConfigurationManager.AppSettings["LogVariableData"].ToString(), out _logVariableData);
            }

            if (_logResponseData || _logVariableData)
            {
                Thread logResponseThread = new Thread(new ThreadStart(ResponseMessageWriter));
                logResponseThread.Start();
            }

            _statusUpdateTimer = new System.Timers.Timer(1000);
            _statusUpdateTimer.Enabled = true;
            _statusUpdateTimer.Elapsed += new ElapsedEventHandler(StatusUpdateTimer_Tick);
            if (monitorCounter != null)
            {
                ConfigMoniter(monitorCounter);
            }
        }

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

                if (_scriptExecutorList.FindAll(f => f.IsRunCompleted).Count == _scriptExecutorList.Count && _tempCreatedUser != 0 && _tempCreatedUser == _tempCompletedUser)
                {
                    try
                    {
                        _statusUpdateTimer.Stop();
                        for (int index = 0; index < 9; index++)
                        {
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
                logger.Debug("scriptid = " + scriptid);
                XmlNode setting = script.SelectNodes("//script[@id='" + scriptid + "']//setting")[0];
                XmlNode vuscript = script.SelectNodes("//script[@id='" + scriptid + "']//vuscript")[0];
                ScriptExecutor scriptRunnerSce = new ScriptExecutor(setting, vuscript, executionReport.ReportName, _distribution);
                if (scriptRunnerSce.StartUserId > 0)
                {
                    _scriptExecutorList.Add(scriptRunnerSce);
                }
            }

            executionReport.StartTime = DateTime.Now;

            foreach (ScriptExecutor scr in _scriptExecutorList)
            {
                scr.OnLockReportData += scr_OnLockReportData;
                scr.OnLockError += scr_OnLockError;
                scr.OnLockLog += scr_OnLockLog;
                scr.OnLockTransactions += scr_OnLockTransactions;
                scr.OnLockUserDetail += scr_OnLockUserDetail;
                if (_logVariableData)
                {
                    scr.OnVariableCreated += scr_OnVariableCreated;
                }

                if (_logResponseData)
                {
                    scr.OnResponse += scr_OnResponse;
                }
                scr.Run();
            }
            if (_scriptExecutorList.Count > 0)
            {
                _statusUpdateTimer.Start();
                SendData();
                return true;
            }
            else
            {
                executionReport.ExecutionStatus = Status.Completed;
                return false;
            }
        }

        void scr_OnResponse(ResponseDetail data)
        {
            lock (_responseDetailSyncObj)
            {
                _responseDetailQueue.Enqueue(data);
            }
        }

        void scr_OnVariableCreated(VariableDetail data)
        {
            lock (_variableDetailSyncObj)
            {
                _variableDetailQueue.Enqueue(data);
            }
        }

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

        public void Stop()
        {
            try
            {
                _stopRunning = true;
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
                    status.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", scripts.StatusSummary.ScriptId, scripts.StatusSummary.TotalVUserCreated, scripts.StatusSummary.TotalVUserCompleted, scripts.StatusSummary.TotalTwoHundredStatusCodeCount, scripts.StatusSummary.TotalThreeHundredStatusCodeCount, scripts.StatusSummary.TotalFourHundredStatusCodeCount, scripts.StatusSummary.TotalFiveHundredStatusCodeCount, Convert.ToInt16(scripts.IsRunCompleted), scripts.StatusSummary.TotalErrorCount, scripts.StatusSummary.ScriptName));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return status.ToString();
        }

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

        private void Send(byte[] dataObj)
        {
            TrasportData data = null;
            try
            {
                data = new TrasportData("status", dataObj, _header);
                //logger.Debug("Sending data to Appedo Server.");
                //logger.Debug("Header : ");
                //foreach(KeyValuePair<string, string> header in data.Header)
                //{
                //    logger.Debug(header.Key + " : " + header.Value);
                //}
                //logger.Debug("DataString = " + data.DataStr);
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
                //try
                //{
                //    string path = ExceptionHandler.WriteReportData(DateTime.Now.Ticks.ToString(), data.DataBytes);
                //    if (path != string.Empty)
                //    {
                //        _dataXml.doc.SelectSingleNode("root").AppendChild(_dataXml.CreateData(_runid, _appedoIp, _appedoPort, path));
                //        _faildData = null;
                //        _dataXml.Save();
                //    }
                //}
                //catch (Exception ex3)
                //{
                //    ExceptionHandler.WritetoEventLog(ex3.StackTrace + Environment.NewLine + ex3.Message);
                //}

                //_dataSendFailedCount++;
                //if (_dataSendFailedCount == 3)
                //{
                //    _dataSendFailedCount = 0;
                //    try
                //    {
                //        if (_appedoFailedUrl != string.Empty)
                //        {
                //            _constants.GetPageContent(_appedoFailedUrl);
                //        }
                //    }
                //    catch (Exception ex2)
                //    {
                //        ExceptionHandler.WritetoEventLog(ex2.StackTrace + Environment.NewLine + ex2.Message);
                //    }
                //}
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
                        mon.counter_value = Convert.ToDecimal(CountersAllInstance[key].NextValue());
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

        private void ResponseMessageWriter()
        {
            MessageQueue queue = GetMSMQ(string.Format("FormatName:Direct=TCP:{0}\\private$\\appedo_logs", _appedoIp));
            if (queue == null)
            {
                // Don't run the thread when there is no MSMQ active service
                return;
            }

            while (!_stopRunning)
            {
                try
                {
                    if (_responseDetailQueue.Count == 0 || _variableDetailQueue.Count == 0)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    #region Write Response Data
                    while (_responseDetailQueue.Count > 0)
                    {
                        ResponseDetail detail = null;
                        lock (_responseDetailSyncObj)
                        {
                            detail = _responseDetailQueue.Dequeue();
                        }

                        if (detail != null)
                        {
                            queue.Send(detail, "ResponseData");
                        }
                    }
                    #endregion

                    #region Write Variable Data
                    while (_variableDetailQueue.Count > 0)
                    {
                        VariableDetail detail = null;
                        lock (_variableDetailSyncObj)
                        {
                            detail = _variableDetailQueue.Dequeue();
                        }

                        if (detail != null)
                        {
                            queue.Send(detail, "VariableData");
                        }
                    }
                    #endregion

                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(10000);
                }
            }

            if (queue != null)
            {
                queue.Close();
            }
        }

        private MessageQueue GetMSMQ(string queueName)
        {
            MessageQueue msmq = null;
            try
            {
                msmq = new MessageQueue(queueName, false);
            }
            catch (Exception excp)
            {
                ExceptionHandler.WritetoEventLog("Error while opening the MSMQ for log response messages. " + Environment.NewLine + excp.StackTrace + Environment.NewLine + excp.Message);
            }
            return msmq;
        }
    }
}
