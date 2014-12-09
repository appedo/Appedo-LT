using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public enum Status {Running,ReportGenerating,Completed}
    public class ExecutionReport
    {
        private static ExecutionReport _instance;
        public static ExecutionReport GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ExecutionReport();
            }
            return _instance;
        }

        private int _createdUser = 0;
        private int _completedUser = 0;
        private string _scenarioName = string.Empty;
        private string _reportName = string.Empty;
        private int _totalLoadGenUsed = 1;
        private int _currentLoadGenid = 1;
        private string _loadGenName = "loadgen1";
        private DateTime _startTime = DateTime.Now;
        public Status _executionStatus = Status.Completed;
        

        public object LockObjForCreatedUser = new object();
        public object LockObjForCompletedUser = new object();

        public Status ExecutionStatus
        {
            get{
                return _executionStatus;
            }
            set
            {
                if(value==Status.Running)
                {
                    _createdUser = 0;
                    _completedUser = 0;
                }
                _executionStatus=value;
            }
        }
        public string ScenarioName { get { return _scenarioName; } set { _scenarioName = value; } }
        public string ReportName { get { return _reportName; } set { _reportName = value; } }
        public int CreatedUser { get { return _createdUser; } set { _createdUser = value; } }
        public int CompletedUser { get { return _completedUser; } set { _completedUser = value; } }
        public int TotalLoadGenUsed
        {
            get { return _totalLoadGenUsed; }
            set { _totalLoadGenUsed = value; }
        }
        public int CurrentLoadGenid
        {
            get { return _currentLoadGenid; }
            set { _currentLoadGenid = value; }
        }
        public string LoadGenName
        {
            get { return _loadGenName; }
            set { _loadGenName = value; }
        }
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

    }
}
