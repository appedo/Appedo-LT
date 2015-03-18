using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AppedoLT.Core
{
    [DataContract]
    public class LoadGenRunningStatusData
    {
        private DateTime _time = new DateTime();

        [DataMember(Name = "time")]
        public string timeStr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _time.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _time = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }
        public DateTime Time
        {
            get { return _time; }
            set
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = value.ToUniversalTime() - origin;
                _time = Constants.GetInstance().ConvertFromUnixTimestamp(Math.Floor(diff.TotalMilliseconds));
            }
        }

        private List<Log> _log = new List<Log>();
        private List<RequestException> _error = new List<RequestException>();
        private List<ReportData> _reportData = new List<ReportData>();
        private List<TransactionRunTimeDetail> _transactionsData = new List<TransactionRunTimeDetail>();

        [DataMember(Name = "createduser")]
        public int CreatedUser { get; set; }
        [DataMember(Name = "completeduser")]
        public int CompletedUser { get; set; }
        [DataMember(Name = "iscompleted")]
        public int IsCompleted { get; set; }
        [DataMember(Name = "log")]
        public List<Log> Log { get { return _log; } set { _log = value; } }
        [DataMember(Name = "error")]
        public List<RequestException> Error { get { return _error; } set { _error = value; } }
        [DataMember(Name = "reporddata")]
        public List<ReportData> ReportData { get { return _reportData; } set { _reportData = value; } }

        [DataMember(Name = "transactions")]
        public List<TransactionRunTimeDetail> Transactions { get { return _transactionsData; } set { _transactionsData = value; } }

    }

    [DataContract]
    public class Log
    {
        private DateTime _time = new DateTime();
        public string reportname = string.Empty;

        [DataMember(Name = "loadGen")]
        public string loadGen = Constants.GetInstance().LoadGen;
        [DataMember(Name = "scenarioname")]
        public string scenarioname = string.Empty;
        [DataMember(Name = "scriptid")]
        public string scriptid = string.Empty;
        [DataMember(Name = "scriptname")]
        public string scriptname = string.Empty;
        [DataMember(Name = "logid")]
        public string logid = string.Empty;
        [DataMember(Name = "logname")]
        public string logname = string.Empty;
        [DataMember(Name = "userid")]
        public string userid = string.Empty;
        [DataMember(Name = "iterationid")]
        public string iterationid = string.Empty;
        [DataMember(Name = "message")]
        public string message = string.Empty;

        public DateTime time { get { return _time; } set { _time = value; } }

        [DataMember(Name = "time")]
        public string timeStr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _time.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _time = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},\"{9}\",{10}", this.loadGen, this.reportname, this.scenarioname, this.scriptid, this.scriptname,
                                                                                     this.logid, this.logname, this.userid, this.iterationid, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    [DataContract]
    public class RequestException
    {
        private DateTime _time = new DateTime();

        [DataMember(Name = "requestexceptionid")]
        public string requestexceptionid { get; set; }

        public string reportname = string.Empty;

        [DataMember(Name = "scenarioname")]
        public string scenarioname = string.Empty;

        [DataMember(Name = "scriptname")]
        public string scriptname = string.Empty;

        [DataMember(Name = "requestid")]
        public string requestid = string.Empty;

        [DataMember(Name = "userid")]
        public string userid = string.Empty;

        [DataMember(Name = "iterationid")]
        public string iterationid = string.Empty;

        [DataMember(Name = "errorcode")]
        public string errorcode = string.Empty;

        [DataMember(Name = "message")]
        public string message = string.Empty;

        public DateTime time { get { return _time; } set { _time = value; } }

        [DataMember(Name = "from")]
        public string from = string.Empty;

        [DataMember(Name = "loadGen")]
        public string loadGen = Constants.GetInstance().LoadGen;

        [DataMember(Name = "request")]
        public string request = string.Empty;

        [DataMember(Name = "time")]
        public string timeStr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _time.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _time = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        public override string ToString()
        {

            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},\"{8}\",{9},\"{10}\"", this.loadGen, this.reportname, this.scenarioname, this.scriptname, this.requestid,
                                                                            this.userid, this.iterationid, this.errorcode, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"), this.request.Replace("\"", "\"\""));
        }
    }

    [DataContract]
    public class ReportData
    {
        public DateTime _starttime = new DateTime();
        public DateTime _endtime = new DateTime();


        public DateTime starttime { get { return _starttime; } set { _starttime = value; } }
        public DateTime endtime { get { return _endtime; } set { _endtime = value; } }


        [DataMember(Name = "loadgen")]
        public string loadgen { get; set; }

        [DataMember(Name = "sourceip")]
        public string sourceip { get; set; }

        [DataMember(Name = "loadgenname")]
        public string loadgenanme { get; set; }

        [DataMember(Name = "scenarioname")]
        public string scenarioname { get; set; }

        [DataMember(Name = "scriptid")]
        public string scriptid { get; set; }

        [DataMember(Name = "containerid")]
        public string containerid { get; set; }

        [DataMember(Name = "containername")]
        public string containername { get; set; }

        [DataMember(Name = "pageid")]
        public string pageid { get; set; }

        [DataMember(Name = "requestid")]
        public string requestid { get; set; }

        [DataMember(Name = "address")]
        public string address { get; set; }

        [DataMember(Name = "userid")]
        public int userid { get; set; }

        [DataMember(Name = "iterationid")]
        public int iterationid { get; set; }

        [DataMember(Name = "starttime")]
        public string starttimestr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _starttime.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _starttime = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        [DataMember(Name = "endtime")]
        public string endtimestr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _endtime.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _endtime = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        [DataMember(Name = "diff")]
        public double diff { get; set; }

        [DataMember(Name = "reponsecode")]
        public long responsesize { get; set; }

        [DataMember(Name = "responsesize")]
        public string reponseCode { get; set; }
        public ReportData()
        {
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                                                                                 this.loadgen,
                                                                                 this.sourceip,
                                                                                 this.loadgenanme,
                                                                                 this.scenarioname,
                                                                                 this.scriptid,
                                                                                 this.containerid,
                                                                                 this.containername,
                                                                                 this.pageid,
                                                                                 this.requestid,
                                                                                 this.address,
                                                                                 this.userid,
                                                                                 this.iterationid,
                                                                                 this.starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                                 this.endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                                 this.diff.ToString(),
                                                                                 this.reponseCode,
                                                                                 this.responsesize);
        }
    }

    [DataContract]
    public class TransactionRunTimeDetail
    {
        public DateTime _starttime = new DateTime();
        public DateTime _endtime = new DateTime();

        public DateTime StartTime
        {
            get { return _starttime; }
            set { _starttime = value; }
        }
        public DateTime EndTime
        {
            get { return _endtime; }
            set { _endtime = value; }
        }

        [DataMember(Name = "scriptid")]
        public string ScriptId = string.Empty;

        [DataMember(Name = "scenarioname")]
        public string ScenarioName = string.Empty;

        [DataMember(Name = "scriptname")]
        public string ScriptName = string.Empty;

        [DataMember(Name = "userid")]
        public string UserId = string.Empty;

        [DataMember(Name = "iterationid")]
        public string IterationId = string.Empty;

        [DataMember(Name = "transactionname")]
        public string TransactionName = string.Empty;

        [DataMember(Name = "starttime")]
        public string StartTimeStr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _starttime.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _starttime = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        [DataMember(Name = "endtime")]
        public string EndTimestr
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = _endtime.ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds).ToString();
            }
            set
            {
                _endtime = Constants.GetInstance().ConvertFromUnixTimestamp(Convert.ToDouble(value));
            }
        }

        [DataMember(Name = "difference")]
        public double Difference
        {
            get
            {
                if (IsEnd == true)
                {
                    TimeSpan diff = EndTime - StartTime;
                    return diff.TotalMilliseconds;
                }
                else
                {
                    return 0;
                }
            }
            set { }
        }

        [DataMember(Name = "isend")]
        public string IsEndstr { get { return IsEnd.ToString(); } set { IsEnd = Convert.ToBoolean(value); } }


        public bool IsEnd = false;

        public override string ToString()
        {
            StringBuilder query = new StringBuilder();
            query.Append("").Append(ScriptId).Append("").Append(",");
            query.Append("\"").Append(ScenarioName).Append("\"").Append(",");
            query.Append("\"").Append(ScriptName).Append("\"").Append(",");
            query.Append(UserId).Append(",");
            query.Append(IterationId).Append(",");
            query.Append("\"").Append(TransactionName).Append("\"").Append(",");
            query.Append("\"").Append(StartTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("\"").Append(",");
            query.Append("\"").Append(EndTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("\"").Append(",");
            query.Append(Convert.ToInt16(IsEnd)).Append(",");
            query.Append(Difference.ToString());
            return query.ToString();
        }
    }
}
