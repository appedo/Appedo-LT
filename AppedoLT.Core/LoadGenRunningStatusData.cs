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
        [DataMember(Name = "createduser")]
        public int CreatedUser { get; set; }
        [DataMember(Name = "completeduser")]
        public int CompletedUser { get; set; }
        [DataMember(Name = "iscompleted")]
        public int IsCompleted { get; set; }
        [DataMember(Name = "log")]
        public List<Log> Log { get { return _log; } set { _log = value; } }

    }

    [DataContract]
    public class Log
    {
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
        [DataMember(Name = "time")]
        public DateTime time = new DateTime();

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},\"{9}\",{10}", this.loadGen, this.reportname, this.scenarioname, this.scriptid, this.scriptname,
                                                                                     this.logid, this.logname, this.userid, this.iterationid, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
