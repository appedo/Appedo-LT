using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
   public class Log
    {
        public string loadGen = Constants.GetInstance().LoadGen;
        public string reportname = string.Empty;
        public string scenarioname = string.Empty;
        public string scriptid = string.Empty;
        public string scriptname = string.Empty;
        public string logid = string.Empty;
        public string logname = string.Empty;
        public string userid = string.Empty;
        public string iterationid = string.Empty;
        public string message = string.Empty;
        public DateTime time = new DateTime();
        
        public override string ToString()
        {

            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},\"{9}\",{10}", this.loadGen, this.reportname, this.scenarioname, this.scriptid, this.scriptname,
                                                                                     this.logid,this.logname,this.userid, this.iterationid, this.message.Replace("\"", "\"\""), this.time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
