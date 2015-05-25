using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace AppedoLT.Core
{
    [DataContract]
    public class LoadGenRunningStatusData
    {
        private DateTime _time = new DateTime();

        
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
        private List<UserDetail> _userDetailData = new List<UserDetail>();
      
      
        [DataMember(Name = "runid")]
        public string Runid { get; set; }
        [DataMember(Name = "log")]
        public List<Log> Log { get { return _log; } set { _log = value; } }
        [DataMember(Name = "error")]
        public List<RequestException> Error { get { return _error; } set { _error = value; } }
        [DataMember(Name = "reporddata")]
        public List<ReportData> ReportData { get { return _reportData; } set { _reportData = value; } }
        [DataMember(Name = "transactions")]
        public List<TransactionRunTimeDetail> Transactions { get { return _transactionsData; } set { _transactionsData = value; } }
        [DataMember(Name = "userdetail")]
        public List<UserDetail> UserDetailData { get { return _userDetailData; } set { _userDetailData = value; } }

    }
    
    [DataContract]
    public class Log
    {
        private DateTime _time = new DateTime();
        public string reportname = string.Empty;

        [DataMember(Name = "loadgen")]
        public string loadGen = Constants.GetInstance().LoadGen;
        [DataMember(Name = "scenarioname")]
        public string scenarioname = string.Empty;
        [DataMember(Name = "scriptid")]
        public string scriptid = string.Empty;
        [DataMember(Name = "scriptname")]
        public string scriptname = string.Empty;
        [DataMember(Name = "log_id")]
        public string logid = string.Empty;
        [DataMember(Name = "log_name")]
        public string logname = string.Empty;
        [DataMember(Name = "user_id")]
        public string userid = string.Empty;
        [DataMember(Name = "iteration_id")]
        public string iterationid = string.Empty;
        [DataMember(Name = "message")]
        public string message = string.Empty;
        
        [Browsable(false)]
        public DateTime time { get { return _time; } set { _time = value; } }

        [Browsable(false)]
        [DataMember(Name = "log_time")]
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

      
        [DisplayName("Logid")]
        public string LogID { get { return logid; } private set { } }
        [DisplayName("Log name")]
        public string Logname { get { return logname; } private set { } }
        [DisplayName("Message")]
        public string Message { get { return message; } private set { } }
        [DisplayName("Time")]
        public string Time { get { return _time.ToString(); } private set { } }
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
        private string _errorcode = string.Empty;
        
        [Browsable(false)]
        [DataMember(Name = "requestexceptionid")]
        public string requestexceptionid { get; set; }

        public string reportname = string.Empty;

        [DataMember(Name = "scenarioname")]
        public string scenarioname = string.Empty;

        [DataMember(Name = "scriptname")]
        public string scriptname = string.Empty;

        [DataMember(Name = "containerid")]
        public string containerid { get; set; }

        [DataMember(Name = "containername")]
        public string containername { get; set; }

        [DataMember(Name = "requestid")]
        public string requestid = string.Empty;

        [DataMember(Name = "user_id")]
        public string userid = string.Empty;

        [DataMember(Name = "iteration_id")]
        public string iterationid = string.Empty;
        
        [Browsable(false)]
        [DataMember(Name = "errorcode")]
        public string errorcode
        {
            get {return _errorcode; }
            set {
                if (value == null || value == string.Empty)
                    _errorcode = "700";
                else _errorcode = value;
            }
        }

        [DataMember(Name = "message")]
        public string message = string.Empty;

        [Browsable(false)]
        public DateTime time { get { return _time; } set { _time = value; } }

        [DataMember(Name = "source")]
        public string from = string.Empty;

        [DataMember(Name = "loadgen")]
        public string loadGen = Constants.GetInstance().LoadGen;

        [DataMember(Name = "request")]
        public string request = string.Empty;
        
        [Browsable(false)]
        [DataMember(Name = "log_time")]
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
     
        [DisplayName("Url")]
        public string Url { get { return request; } private set { } }
        [DisplayName("Errorcode")]
        public string Errorcode { get { return _errorcode; } private set { } }
        [DisplayName("Message")]
        public string Message { get { return message; } private set { } }
        
        [DisplayName("Requestid")]
        public string Requestid { get { return requestid; } private set { } }
        
        public override string ToString()
        {

            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},\"{8}\",{9},\"{10}\",{11},{12}", this.loadGen, this.reportname, this.scenarioname, this.scriptname, this.requestid,
                                                                            this.userid, this.iterationid, this.errorcode, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"), this.request.Replace("\"", "\"\""),this.containerid,this.containername);
        }
    }

    [DataContract]
    [Serializable]
    public class ReportData
    {
        private DateTime _starttime = new DateTime();
        private DateTime _endtime = new DateTime();

        private string _loadgen = string.Empty;
        private string _sourceip = "0";
        private string _loadgenanme = string.Empty;
        private string _scenarioname = string.Empty;
        private string _scriptid = "0";
        private string _containerid = "0";
        private string _containername = string.Empty;
        private string _pageid = "0";
        private string _requestid = "0";
        private string _address = string.Empty;
        private string _reponseCode = "0";

        public DateTime starttime { get { return _starttime; } set { _starttime = value; } }
        public DateTime endtime { get { return _endtime; } set { _endtime = value; } }


        [DataMember(Name = "loadgen")]
        public string loadgen { get { return _loadgen; } set { _loadgen = value; } }

        [DataMember(Name = "source_ip")]
        public string sourceip { get { return _sourceip; } set { _sourceip = value; } }

        [DataMember(Name = "loadgen_name")]
        public string loadgenanme { get { return _loadgenanme; } set { _loadgenanme = value; } }

        [DataMember(Name = "scenarioname")]
        public string scenarioname { get { return _scenarioname; } set { _scenarioname = value; } }

        [DataMember(Name = "script_id")]
        public string scriptid { get { return _scriptid; } set { _scriptid = value; } }

        [DataMember(Name = "container_id")]
        public string containerid { get { return _containerid; } set { _containerid = value; } }

        [DataMember(Name = "containername")]
        public string containername { get { return _containername; } set { _containername = value; } }

        [DataMember(Name = "page_id")]
        public string pageid { get { return _pageid; } set { _pageid = value; } }

        [DataMember(Name = "request_id")]
        public string requestid { get { return _requestid; } set { _loadgen = _requestid; } }

        [DataMember(Name = "address")]
        public string address { get { return _address; } set { _address = value; } }

        [DataMember(Name = "userid")]
        public int userid { get; set; }

        [DataMember(Name = "iteration_id")]
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

        [DataMember(Name = "responsesize")]
        public long responsesize { get; set; }
        
        [DataMember(Name = "reponsecode")]
        public string reponseCode { get { return _reponseCode; } set { _reponseCode = value; } }
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

        [DataMember(Name = "script_id")]
        public string ScriptId = string.Empty;

        [DataMember(Name = "scenarioname")]
        public string ScenarioName = string.Empty;

        [DataMember(Name = "scriptname")]
        public string ScriptName = string.Empty;

        [DataMember(Name = "userid")]
        public string UserId = string.Empty;

        [DataMember(Name = "iteration_id")]
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

        [DataMember(Name = "diff")]
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

    [DataContract]
    public class UserDetail
    {
        [DataMember(Name = "loadgenname")]
        public string loadgenanme { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "script_id")]
        public string scriptid { get; set; }

        [DataMember(Name = "userid")]
        public int userid { get; set; }

        public DateTime _time = new DateTime();
        public DateTime Time { get { return _time; } set { _time = value; } }

        [DataMember(Name = "runtime")]
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
    }
}
