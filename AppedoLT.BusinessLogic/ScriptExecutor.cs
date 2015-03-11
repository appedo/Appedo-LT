using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml;

namespace AppedoLT.BusinessLogic
{
    public class ScriptExecutor
    {
        private XmlNode _vuScript;
        private int _startUserid;
        private int _endUserid;
        private int _createdUserCount = 0;
        private int _completedUserCount = 0;
        private ManualResetEvent _threadRun = new ManualResetEvent(false);
        private VUScriptSetting _setting = new VUScriptSetting();
        private Result _resultLog = Result.GetInstance();
        private string _reportName = string.Empty;
        private List<VUser> _usersList = new List<VUser>();
        private Stopwatch _durationTimer = new System.Diagnostics.Stopwatch();
        private System.Timers.Timer tmrRun = new System.Timers.Timer();
        private System.Timers.Timer tmrVUCreator = new System.Timers.Timer();
        private Constants _constant = Constants.GetInstance();
        public BackgroundWorker Worker = new BackgroundWorker();
        public VUScriptStatus StatusSummary = new VUScriptStatus();

        public Queue<Log> LogBuffer = new Queue<Log>();
        public Queue<RequestException> ErrorBuffer = new Queue<RequestException>();
        public Queue<ReportData> reportDataBuffer = new Queue<ReportData>();

        public bool IsRunCompleted = false;
        private string Scriptid { get; set; }
        private string Scriptname { get; set; }
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
        object userCreationLock = new object();
        object userCompletedLock = new object();

        public ScriptExecutor(XmlNode settingNode, XmlNode vuScript, string reportName)
        {
            try
            {
                StatusSummary .ScriptId=Scriptid = vuScript.Attributes["id"].Value;
                StatusSummary.ScriptName=Scriptname = vuScript.Attributes["name"].Value;
                VUScriptSetting setting = new VUScriptSetting();
                setting.Type = settingNode.Attributes["type"].Value;
                setting.BrowserCache = Convert.ToBoolean(settingNode.Attributes["browsercache"].Value);
                setting.DurationTime = settingNode.Attributes["durationtime"].Value;
                setting.IncrementUser = settingNode.Attributes["incrementuser"].Value;
                setting.IncrementTime = settingNode.Attributes["incrementtime"].Value;
                setting.Iterations = settingNode.Attributes["iterations"].Value;
                setting.MaxUser = settingNode.Attributes["maxuser"].Value;
                setting.StartUser = settingNode.Attributes["startuser"].Value;

                _vuScript = vuScript;
                _setting = setting;
                _reportName = reportName;

                _createdUserCount = setting.StartUserId;

                tmrRun.Interval = 100;

                if (int.Parse(_setting.GetVUCreationIntervel().ToString()) < 1)
                {
                    tmrVUCreator.Interval = 1;
                }
                else
                {
                    tmrVUCreator.Interval = int.Parse(_setting.GetVUCreationIntervel().ToString());
                }

                tmrVUCreator.Elapsed += new ElapsedEventHandler(tmrVUCreator_Tick);

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

                tmrRun.Elapsed += new ElapsedEventHandler(tmrRun_Tick);
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
                StatusSummary.ScriptId = Scriptid = vuScript.Attributes["id"].Value;
                StatusSummary.ScriptName = Scriptname = vuScript.Attributes["name"].Value;
                VUScriptSetting setting = new VUScriptSetting();
                setting.Type = settingNode.Attributes["type"].Value;
                setting.BrowserCache = Convert.ToBoolean(settingNode.Attributes["browsercache"].Value);
                setting.DurationTime = settingNode.Attributes["durationtime"].Value;
                setting.IncrementUser = settingNode.Attributes["incrementuser"].Value;
                setting.IncrementTime = settingNode.Attributes["incrementtime"].Value;
                setting.Iterations = settingNode.Attributes["iterations"].Value;
                setting.MaxUser = settingNode.Attributes["maxuser"].Value;
                setting.StartUser = settingNode.Attributes["startuser"].Value;

                _vuScript = vuScript;
                _setting = setting;
                _reportName = reportName;

                _createdUserCount = setting.StartUserId;

                tmrRun.Interval = 100;

                if (int.Parse(_setting.GetVUCreationIntervel().ToString()) < 1)
                {
                    tmrVUCreator.Interval = 1;
                }
                else
                {
                    tmrVUCreator.Interval = int.Parse(_setting.GetVUCreationIntervel().ToString());
                }

                tmrVUCreator.Elapsed += new ElapsedEventHandler(tmrVUCreator_Tick);

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

                tmrRun.Elapsed += new ElapsedEventHandler(tmrRun_Tick);
                Worker.DoWork += new DoWorkEventHandler(DoWork);

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

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

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _durationTimer.Reset();
                elapsedTime.Reset();
                elapsedTime.Start();

                for (int index = 1; index <= int.Parse(_setting.StartUser); index++)
                {
                    _createdUserCount++;
                    if (_createdUserCount >= _startUserid && _createdUserCount <= _endUserid)
                    {
                        _usersList.Add(GetVUser(_createdUserCount));
                    }
                }
                foreach (VUser vuser in _usersList)
                {
                    vuser.Start();
                    StatusSummary.TotalVUserCreated++;
                }
                if (int.Parse(_setting.StartUser) < int.Parse(_setting.MaxUser))
                {
                    tmrVUCreator.Enabled = true;
                    tmrVUCreator.Start();
                }
                _durationTimer.Start();
                tmrRun.Start();
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
                        lock (userCompletedLock)
                        {
                            tmrRun.Stop();
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
                            tmrVUCreator.Stop();
                            elapsedTime.Stop();
                            tmrRun.Stop();
                            _durationTimer.Stop();
                            lock (userCompletedLock)
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
                lock (userCreationLock)
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
                        if (_usersList.Count >= int.Parse(_setting.MaxUser)) break;
                        _createdUserCount++;
                        if (_createdUserCount >= _startUserid && _createdUserCount <= _endUserid)
                        {
                            VUCreatorUsers.Add(GetVUser(_createdUserCount));
                        }
                    }

                    if (_createdUserCount >= int.Parse(_setting.MaxUser))
                    {
                        tmrVUCreator.Stop();
                    }

                    foreach (VUser user in VUCreatorUsers)
                    {
                        user.Start();
                        StatusSummary.TotalVUserCreated++;
                        _usersList.Add(user);
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

        public void Stop()
        {
            lock (userCreationLock)
            {
                lock (userCompletedLock)
                {
                    tmrRun.Stop();
                    tmrVUCreator.Stop();
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

        private void ClearUsers()
        {
            _usersList.Clear();
        }

        private VUser GetVUser(int userid)
        {
            return new VUser(int.Parse(_setting.MaxUser), _reportName, _setting.Type, userid, int.Parse(_setting.Iterations), _vuScript, _setting.BrowserCache, Request.GetIPAddress(_createdUserCount), LogBuffer, ErrorBuffer, reportDataBuffer);
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
       
    }
   
}
