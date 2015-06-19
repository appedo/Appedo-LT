using AppedoLT.BusinessLogic;
using AppedoLT.Core;
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
        private Queue<Log> _LogBuffer = new Queue<Log>();
        private Queue<RequestException> _ErrorBuffer = new Queue<RequestException>();
        private Queue<ReportData> _reportDataBuffer = new Queue<ReportData>();
        private Queue<TransactionRunTimeDetail> _TransactionDataBuffer = new Queue<TransactionRunTimeDetail>();
        private Queue<UserDetail> _UserDetailBuffer = new Queue<UserDetail>();
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

        public RunScenario(string runid, string appedoIP, string appedoPort, string scenarioXml, string distribution, string appedoFailedUrl)
        {
            _runid = runid;
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
                            if (_reportDataBuffer.Count > 0 || _UserDetailBuffer.Count > 0 || _TransactionDataBuffer.Count > 0 || _LogBuffer.Count > 0 || _ErrorBuffer.Count > 0)
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

        private void ClearData()
        {
            try
            {
                _LogBuffer.Clear();
                _ErrorBuffer.Clear();
                _reportDataBuffer.Clear();
                _TransactionDataBuffer.Clear();
                _UserDetailBuffer.Clear();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        void scr_OnLockUserDetail(UserDetail data)
        {
            lock (_UserDetailBuffer)
            {
                if (data != null)
                {
                    _UserDetailBuffer.Enqueue(data);
                }
            }
        }

        void scr_OnLockTransactions(TransactionRunTimeDetail data)
        {
            lock (_TransactionDataBuffer)
            {
                if (data != null)
                {
                    _TransactionDataBuffer.Enqueue(data);
                }
            }
        }

        void scr_OnLockLog(Log data)
        {
            lock (_LogBuffer)
            {
                if (data != null)
                {
                    _LogBuffer.Enqueue(data);
                }
            }
        }

        void scr_OnLockError(RequestException data)
        {
            lock (_ErrorBuffer)
            {
                if (data != null)
                {
                    _ErrorBuffer.Enqueue(data);
                }
            }
        }

        void scr_OnLockReportData(ReportData data)
        {
            lock (_reportDataBuffer)
            {
                if (data != null)
                {
                    _reportDataBuffer.Enqueue(data);
                }
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
                    status.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", scripts.StatusSummary.ScriptId, scripts.StatusSummary.TotalVUserCreated, scripts.StatusSummary.TotalVUserCompleted, scripts.StatusSummary.TotalTwoHundredStatusCodeCount, scripts.StatusSummary.TotalThreeHundredStatusCodeCount, scripts.StatusSummary.TotalFourHundredStatusCodeCount, scripts.StatusSummary.TotalFiveHundredStatusCodeCount, Convert.ToInt16(scripts.IsRunCompleted), scripts.StatusSummary.TotalErrorCount, scripts.StatusSummary.ScriptName));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return status.ToString();
        }

        private void GetLog(List<Log> logList)
        {
            try
            {
               lock (_LogBuffer)
                {
                    int count = _LogBuffer.Count;
                    for (; count > 0; count--)
                    {
                        logList.Add(_LogBuffer.Dequeue());
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void GetError(List<RequestException> errorList)
        {
            try
            {
                int count = _ErrorBuffer.Count;
                lock (_ErrorBuffer)
                {
                    for (; count > 0; count--)
                    {
                        errorList.Add(_ErrorBuffer.Dequeue());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void GetReportData(List<ReportData> reportDataList)
        {
            try
            {
                int count = _reportDataBuffer.Count;
                lock (_reportDataBuffer)
                {
                    for (; count > 0; count--)
                    {
                        reportDataList.Add(_reportDataBuffer.Dequeue());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void GetTransactions(List<TransactionRunTimeDetail> transactionsList)
        {
            try
            {
                int count = _TransactionDataBuffer.Count;
                lock (_TransactionDataBuffer)
                {
                    for (; count > 0; count--)
                    {
                        transactionsList.Add(_TransactionDataBuffer.Dequeue());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void GetUserDetail(List<UserDetail> userDetailsList)
        {
            try
            {
                int count = _UserDetailBuffer.Count;
               lock (_UserDetailBuffer)
                {
                    for (; count > 0; count--)
                    {
                        userDetailsList.Add(_UserDetailBuffer.Dequeue());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void SendData()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_LogBuffer.Count != 0
                              || _ErrorBuffer.Count != 0
                              || _reportDataBuffer.Count != 0
                              || _TransactionDataBuffer.Count != 0
                              || _UserDetailBuffer.Count != 0 || _faildData != null)
                        {
                            LoadGenRunningStatusData data = new LoadGenRunningStatusData();
                            data.Runid = _runid;
                            GetLog(data.Log);
                            GetError(data.Error);
                            GetReportData(data.ReportData);
                            GetTransactions(data.Transactions);
                            GetUserDetail(data.UserDetailData);

                            if (_faildData != null)
                            {
                                data.Log.AddRange(_faildData.Log);
                                data.Error.AddRange(_faildData.Error);
                                data.ReportData.AddRange(_faildData.ReportData);
                                data.Transactions.AddRange(_faildData.Transactions);
                                data.UserDetailData.AddRange(_faildData.UserDetailData);
                            }
                            _faildData = data;
                            try
                            {
                                Trasport trasport = new Trasport(_appedoIp, _appedoPort, 30000);
                                trasport.Send(new TrasportData("status", _constants.Serialise(data), _header));
                               
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

                                try
                                {
                                    string path = ExceptionHandler.WriteReportData(DateTime.Now.Ticks.ToString(), _constants.Serialise(data));
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
                        else if ((_scriptExecutorList.Count == 0
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
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    finally { Thread.Sleep(5000); }
                }

            }).Start();
        }
    }
}
