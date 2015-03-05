using System;
using System.Runtime.Serialization;

namespace AppedoLT.Core
{
    [DataContract]
    public class Log
    {
        [DataMember(Name = "_message")]
        private string _message = string.Empty;
        [DataMember(Name = "loadGen")]
        public string loadGen = Constants.GetInstance().LoadGen;
        [DataMember(Name = "reportname")]
        public string reportname = string.Empty;
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
        // public string message { set { _message = "\"" ++ "\""; } get { return  _message} }
        [DataMember(Name = "time")]
        public DateTime time = new DateTime();
      
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},\"{9}\",{10}", this.loadGen, this.reportname, this.scenarioname, this.scriptid, this.scriptname,
                                                                                     this.logid, this.logname, this.userid, this.iterationid, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
