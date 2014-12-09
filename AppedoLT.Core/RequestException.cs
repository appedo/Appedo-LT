using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public class RequestException
    {
        public string requestexceptionid { get; set; }
        public string reportname = string.Empty;
        public string scenarioname = string.Empty;
        public string scriptname = string.Empty;
        public string requestid = string.Empty;
        public string userid = string.Empty;
        public string iterationid = string.Empty;
        public string request = string.Empty;
        public string errorcode = string.Empty;
        public string message = string.Empty;
        public DateTime time = new DateTime();
        public string from = string.Empty;
        public string loadGen = Constants.GetInstance().LoadGen;
        public override string ToString()
        {

            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},\"{8}\",{9}", this.loadGen, this.reportname, this.scenarioname, this.scriptname, this.requestid,
                                                                            this.userid, this.iterationid, this.errorcode, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
