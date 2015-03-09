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

        private List<Log> _log = new List<Log>();
        private List<RequestException> _error = new List<RequestException>();

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
            set { 
                _time =Constants.GetInstance(). ConvertFromUnixTimestamp(Convert.ToDouble(value)); 
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
}
