﻿using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Configuration;

namespace AppedoLT.BusinessLogic
{
    public class ScriptExecutor
    {

        #region The private fields

        private string _reportName = string.Empty;
        private string _scriptid { get; set; }
        private string _scriptname { get; set; }
        private int _startUserid;
        private int _endUserid;
        private int _createdUserCount = 0;
        private int _completedUserCount = 0;
        private object _userCreationLock = new object();
        private object _userCompletedLock = new object();
        private List<VUser> _usersList = new List<VUser>();
        private Stopwatch _durationTimer = new System.Diagnostics.Stopwatch();
        private XmlNode _vuScript;
        private ManualResetEvent _threadRun = new ManualResetEvent(false);
        private System.Timers.Timer _tmrRun = new System.Timers.Timer();
        private System.Timers.Timer _tmrVUCreator = new System.Timers.Timer();
        private VUScriptSetting _setting = new VUScriptSetting();
        private bool _isStop = false;
        private Constants _constant = Constants.GetInstance();
        List<int> excludeLogList = null;

        #endregion

        #region The public property

        public BackgroundWorker Worker = new BackgroundWorker();
        public VUScriptStatus StatusSummary = new VUScriptStatus();
        public bool IsRunCompleted = false;
        public System.Diagnostics.Stopwatch elapsedTime = new System.Diagnostics.Stopwatch();
        public int StartUserId { get { return _startUserid; } private set { } }
        public List<VUser> UserList
        {
            get
            {
                return _usersList;
            }
        }
        ExecutionReport Status = ExecutionReport.GetInstance();

        #endregion

        #region The event

        public event LockReportData OnLockReportData;
        public event LockLog OnLockLog;
        public event LockError OnLockError;
        public event LockTransactions OnLockTransactions;
        public event LockUserDetail OnLockUserDetail;
        public event IterationCompleted OnIterationStarted;
        public event VUserRunCompleted OnVUserRunCompleted;
        public event VUserCreated OnVUserCreated;
        public event LockVariable OnVariableCreated;
        public event LockResponse OnResponse;
        #endregion

        #region The constructor

        public ScriptExecutor(XmlNode settingNode, XmlNode vuScript, string reportName)
        {
            try
            {
                StatusSummary .ScriptId=_scriptid = vuScript.Attributes["id"].Value;
                StatusSummary.ScriptName=_scriptname = vuScript.Attributes["name"].Value;
                VUScriptSetting setting = new VUScriptSetting();
                setting.Type = settingNode.Attributes["type"].Value;
                setting.BrowserCache = Convert.ToBoolean(settingNode.Attributes["browsercache"].Value);
                setting.DurationTime = settingNode.Attributes["durationtime"].Value;
                setting.IncrementUser = settingNode.Attributes["incrementuser"].Value;
                setting.IncrementTime = settingNode.Attributes["incrementtime"].Value;
                setting.Iterations = settingNode.Attributes["iterations"].Value;
                setting.MaxUser = settingNode.Attributes["maxuser"].Value;
                setting.StartUser = settingNode.Attributes["startuser"].Value;
                if (settingNode.Attributes["bandwidth"] != null)
                {
                    setting.Bandwidth = settingNode.Attributes["bandwidth"].Value;
                }
                else
                {
                    // Backward compatibality. If noe is not present then run at full speed without any thorttle
                    setting.Bandwidth = "-1";
                }

                if (settingNode.Attributes["replythinktime"] != null)
                {
                    setting.ReplyThinkTime = Convert.ToBoolean(settingNode.Attributes["replythinktime"].Value);
                }
                else
                {
                    setting.ReplyThinkTime = true;
                }

                if (settingNode.Attributes["parallelconnections"] != null)
                {
                    setting.numberOfParallelCon = settingNode.Attributes["parallelconnections"].Value;
                }
                else
                {
                    setting.numberOfParallelCon = "6";
                }

                _vuScript = vuScript;
                _setting = setting;
                _reportName = reportName;

                _createdUserCount = setting.StartUserId;

                _tmrRun.Interval = 100;

                if (int.Parse(_setting.GetVUCreationIntervel().ToString()) < 1)
                {
                    _tmrVUCreator.Interval = 1;
                }
                else
                {
                    _tmrVUCreator.Interval = int.Parse(_setting.GetVUCreationIntervel().ToString());
                }

                _tmrVUCreator.Elapsed += new ElapsedEventHandler(tmrVUCreator_Tick);

                #region Load Distrubution


                int maxUser = Convert.ToInt16(settingNode.Attributes["maxuser"].Value);
                Dictionary<int, int> userDistribution = new Dictionary<int, int>();
                
                int remainigUser = maxUser % Status.TotalLoadGenUsed;

                for (int index = 1; index <= Status.TotalLoadGenUsed; index++)
                {
                    if (index <= remainigUser)
                    {
                        userDistribution.Add(index, ((maxUser - remainigUser) / Status.TotalLoadGenUsed) + 1);
                    }
                    else
                    {
                        userDistribution.Add(index, ((maxUser - remainigUser) / Status.TotalLoadGenUsed));
                    }
                }
                int sumUser = 0;
                for (int index = 1; index <= Status.TotalLoadGenUsed; index++)
                {
                    if (Status.CurrentLoadGenid == index)
                    {
                        if (userDistribution[index] == 0)
                        {
                            _startUserid = 0;
                        }
                        else
                        {
                            _startUserid = sumUser + 1;
                            _endUserid = sumUser + userDistribution[index];
                        }
                        break;
                    }
                    else if (userDistribution[index] == 0)
                    {
                        _startUserid = 0;
                        _endUserid = 0;
                        break;
                    }
                    sumUser += userDistribution[index];
                }
                #endregion

                _tmrRun.Elapsed += new ElapsedEventHandler(tmrRun_Tick);
                Worker.DoWork += new DoWorkEventHandler(DoWork);

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public ScriptExecutor(XmlNode settingNode, XmlNode vuScript, string reportName, string distribution)
        {
            try
            {
                StatusSummary.ScriptId = _scriptid = vuScript.Attributes["id"].Value;
                StatusSummary.ScriptName = _scriptname = vuScript.Attributes["name"].Value;
                VUScriptSetting setting = new VUScriptSetting();
                setting.Type = settingNode.Attributes["type"].Value;
                setting.BrowserCache = Convert.ToBoolean(settingNode.Attributes["browsercache"].Value);
                setting.DurationTime = settingNode.Attributes["durationtime"].Value;
                setting.IncrementUser = settingNode.Attributes["incrementuser"].Value;
                setting.IncrementTime = settingNode.Attributes["incrementtime"].Value;
                setting.Iterations = settingNode.Attributes["iterations"].Value;
                setting.MaxUser = settingNode.Attributes["maxuser"].Value;
                setting.StartUser = settingNode.Attributes["startuser"].Value;
                if (settingNode.Attributes["bandwidth"] != null)
                {
                    setting.Bandwidth = settingNode.Attributes["bandwidth"].Value;
                }
                else
                {
                    setting.Bandwidth = "-1";
                }

                _vuScript = vuScript;
                _setting = setting;
                _reportName = reportName;

                _createdUserCount = setting.StartUserId;

                _tmrRun.Interval = 100;

                if (int.Parse(_setting.GetVUCreationIntervel().ToString()) < 1)
                {
                    _tmrVUCreator.Interval = 1;
                }
                else
                {
                    _tmrVUCreator.Interval = int.Parse(_setting.GetVUCreationIntervel().ToString());
                }

                _tmrVUCreator.Elapsed += new ElapsedEventHandler(tmrVUCreator_Tick);

                #region Load Distrubution


                int maxUser = Convert.ToInt16(settingNode.Attributes["maxuser"].Value);
                string[] distributionValues = distribution.Split(',');

                Dictionary<int, int> userDistribution = new Dictionary<int, int>();

                for (int index = 1; index <= Status.TotalLoadGenUsed; index++)
                {
                    userDistribution.Add(index, (int)Math.Round((Convert.ToInt16(distributionValues[index - 1]) / 100.0) * maxUser));
                }
                int userDistributionSum = userDistribution.Sum(v => v.Value);
                if (userDistributionSum != maxUser)
                {
                    if (userDistributionSum < maxUser)
                    {
                        int key = userDistribution.FirstOrDefault(x => x.Value == userDistribution.Min(y => y.Value)).Key;
                        userDistribution[key] += (maxUser - userDistributionSum);
                    }
                    else
                    {
                        int key = userDistribution.Last(x => x.Value == userDistribution.Max(y => y.Value)).Key;
                        userDistribution[key] -= (userDistributionSum - maxUser);
                    }
                }
                int sumUser = 0;
                for (int index = 1; index <= Status.TotalLoadGenUsed; index++)
                {
                    if (Status.CurrentLoadGenid == index)
                    {
                        if (userDistribution[index] == 0)
                        {
                            _startUserid = 0;
                        }
                        else
                        {
                            _startUserid = sumUser + 1;
                            _endUserid = sumUser + userDistribution[index];
                        }
                        break;
                    }
                    else if (userDistribution[index] == 0)
                    {
                        _startUserid = 0;
                        _endUserid = 0;
                        break;
                    }
                    sumUser += userDistribution[index];
                }
                #endregion

                _tmrRun.Elapsed += new ElapsedEventHandler(tmrRun_Tick);
                Worker.DoWork += new DoWorkEventHandler(DoWork);

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region The public methods

        public void Run()
        {
            try
            {
                if (_endUserid > 0)
                {
                    Worker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public void Stop()
        {
            _tmrVUCreator.Stop();
            _isStop = true;
            lock (_userCreationLock)
            {
                lock (_userCompletedLock)
                {
                    _tmrRun.Stop();
                    _tmrVUCreator.Stop();
                    foreach (VUser user in _usersList)
                    {
                        user.Break = true;
                    }
                    foreach (VUser user in _usersList)
                    {
                        try
                        {
                            user.Stop();
                            _completedUserCount++;

                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            StatusSummary.TotalVUserCompleted++;
                        }
                    }
                    _usersList.Clear();
                    _threadRun.Reset();
                    IsRunCompleted = true;
                }
            }
        }

        #endregion

        #region The private methods

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _durationTimer.Reset();
                elapsedTime.Reset();
                elapsedTime.Start();

                #region Users for which data needs to be logged
                if (OnResponse != null)
                {
                    // Default value is 10%
                    int percentageUserToLogResponseData = 10;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PercentageUserToLogResponseData"]))
                    {
                        if (!int.TryParse(ConfigurationManager.AppSettings["PercentageUserToLogResponseData"], out percentageUserToLogResponseData))
                        {
                            percentageUserToLogResponseData = 1;
                        }
                        if (percentageUserToLogResponseData == 0)
                        {
                            // Default value is 10%
                            percentageUserToLogResponseData = 10;
                        }
                    }

                    // Get the user list
                    int totalUsers = _endUserid - _startUserid + 1;
                    int numberOfUsersToLog = (int)Math.Ceiling((double)(totalUsers * percentageUserToLogResponseData) / 100);
                    if (numberOfUsersToLog == 0)
                        numberOfUsersToLog = 1;

                    excludeLogList = new List<int>();
                    if (numberOfUsersToLog < totalUsers)
                    {
                        int numberOfUsersToExclude = totalUsers - numberOfUsersToLog;
                        Random rand = new Random(Environment.TickCount);
                        for (int i = 0; i < numberOfUsersToExclude; i++)
                        {
                            // Generate random numbers for which the data need not to be logged
                            int randomNumber = rand.Next(0, totalUsers);
                            while (excludeLogList.Contains(randomNumber))
                            {
                                randomNumber = rand.Next(0, totalUsers);
                            }
                            excludeLogList.Add(randomNumber);
                        }
                    }
                }
                #endregion

                for (int index = 1; index <= int.Parse(_setting.StartUser); index++)
                {
                    _createdUserCount++;
                    if (_createdUserCount >= _startUserid && _createdUserCount <= _endUserid)
                    {
                        VUser user = GetVUser(_createdUserCount);
                        if (excludeLogList.Contains(_usersList.Count))
                        {
                            user.OnResponse -= OnResponse;
                        }
                        _usersList.Add(user);
                    }
                }

                foreach (VUser vuser in _usersList)
                {
                    vuser.Start();
                    StatusSummary.TotalVUserCreated++;
                }
                if (int.Parse(_setting.StartUser) < int.Parse(_setting.MaxUser))
                {
                    _tmrVUCreator.Enabled = true;
                    _tmrVUCreator.Start();
                }
                _durationTimer.Start();
                _tmrRun.Start();
                _threadRun.Set();
                _threadRun.WaitOne();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void tmrRun_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                UpdateStatus();
                if (_setting.Type == "1")
                {
                    _completedUserCount = _usersList.FindAll(f => f.WorkCompleted == true).Count;
                    StatusSummary.TotalVUserCompleted = _completedUserCount;
                    if ((_endUserid - (_startUserid - 1)) == _completedUserCount)
                    {
                        lock (_userCompletedLock)
                        {
                            _tmrRun.Stop();
                            ClearUsers();
                            elapsedTime.Stop();
                            _threadRun.Reset();
                            IsRunCompleted = true;
                            StatusSummary.TotalVUserCompleted = _completedUserCount;
                        }
                    }
                }
                else if (_setting.Type == "2")
                {
                    if (_durationTimer.ElapsedMilliseconds >= _setting.GetVUDutaionIntervel())
                    {
                        try
                        {
                            _tmrVUCreator.Stop();
                            elapsedTime.Stop();
                            _tmrRun.Stop();
                            _durationTimer.Stop();
                            lock (_userCompletedLock)
                            {
                                foreach (VUser thread in _usersList)
                                {
                                    thread.Break = true;
                                }
                                foreach (VUser thread in _usersList)
                                {
                                    try
                                    {
                                        thread.Stop();
                                        _completedUserCount++;
                                        StatusSummary.TotalVUserCompleted++;
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                    }
                                }
                                ClearUsers();
                                _threadRun.Reset();
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            IsRunCompleted = true;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void tmrVUCreator_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<VUser> VUCreatorUsers = new List<VUser>();
            try
            {
                lock (_userCreationLock)
                {
                    if (_isStop == false)
                    {
                        if (_setting.Type == "2")
                        {
                            foreach (VUser thread in _usersList)
                            {
                                if (thread.WorkCompleted == true) thread.Start();
                            }
                        }

                        for (int index = 1; index <= int.Parse(_setting.IncrementUser); index++)
                        {
                            if (_usersList.Count >= int.Parse(_setting.MaxUser) || _isStop == true) break;
                            _createdUserCount++;
                            if (_createdUserCount >= _startUserid && _createdUserCount <= _endUserid)
                            {
                                VUCreatorUsers.Add(GetVUser(_createdUserCount));
                            }
                        }

                        if (_createdUserCount >= int.Parse(_setting.MaxUser))
                        {
                            _tmrVUCreator.Stop();
                        }

                        foreach (VUser user in VUCreatorUsers)
                        {
                            if (excludeLogList.Contains(_usersList.Count))
                            {
                                user.OnResponse -= OnResponse;
                            }

                            user.Start();
                            StatusSummary.TotalVUserCreated++;
                            _usersList.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                VUCreatorUsers = null;
            }
        }
       
        private void ClearUsers()
        {
            _usersList.Clear();
        }

        private VUser GetVUser(int userid)
        {
            VUser user = new VUser(int.Parse(_setting.MaxUser), _reportName, _setting.Type, userid, int.Parse(_setting.Iterations), _vuScript, _setting.BrowserCache, Request.GetIPAddress(_createdUserCount), _setting.ReplyThinkTime, _setting.numberOfParallelCon, GetBandwidth());
            if(OnLockReportData!=null) user.OnLockReportData+=OnLockReportData;
            if(OnLockError != null) user.OnLockError += OnLockError;
            if(OnLockLog != null) user.OnLockLog += OnLockLog;
            if(OnLockTransactions != null) user.OnLockTransactions += OnLockTransactions;
            if(OnLockUserDetail != null) user.OnLockUserDetail += OnLockUserDetail;
            if (OnIterationStarted != null) user.OnIterationStart += OnIterationStarted;
            if (OnVUserRunCompleted != null) user.OnVUserRunCompleted += OnVUserRunCompleted;
            if (OnVUserCreated != null) user.OnVUserCreated += OnVUserCreated;
            if (OnVariableCreated != null) user.OnVariableCreated += OnVariableCreated;
            if (OnResponse != null) user.OnResponse += OnResponse;
                        
            return user;
        }

        private void UpdateStatus()
        {
            lock (StatusSummary)
            {
                StatusSummary.TotalErrorCount = _usersList.Sum((s) => s.VUserStatus.ErrorCount);
                StatusSummary.TotalTwoHundredStatusCodeCount = _usersList.Sum((s) => s.VUserStatus.TwoHundredStatusCodeCount);
                StatusSummary.TotalThreeHundredStatusCodeCount = _usersList.Sum((s) => s.VUserStatus.ThreeHundredStatusCodeCount);
                StatusSummary.TotalFourHundredStatusCodeCount = _usersList.Sum((s) => s.VUserStatus.FourHundredStatusCodeCount);
                StatusSummary.TotalFiveHundredStatusCodeCount = _usersList.Sum((s) => s.VUserStatus.FiveHundredStatusCodeCount);
            }
        }

        private int GetBandwidth()
        {
            int bandwidth = -1;
            if (!int.TryParse(_setting.Bandwidth, out bandwidth))
            {
                // If value is less than 0,then no thortling will be done
                bandwidth = -1;
            }
            return bandwidth;
        }
        #endregion

    }
}
