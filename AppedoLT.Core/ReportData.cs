using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public class ReportData
    {
        public string loadgen { get; set; }
        public string sourceip { get; set; }
        public string scenarioname { get; set; }
        public string scriptid { get; set; }
        public string containerid { get; set; }
        public string containername { get; set; }
        public string pageid { get; set; }
        public string requestid { get; set; }
        public string address { get; set; }
        public int userid { get; set; }
        public int iterationid { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public double diff { get; set; }
        public long responsesize { get; set; }
        public string reponseCode { get; set; }
        public ReportData()
        {
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", this.loadgen, this.sourceip, ExecutionReport.GetInstance().LoadGenName, this.scenarioname,
                                                                                 this.scriptid, this.containerid, this.containername, this.pageid,
                                                                                 this.requestid, this.address, this.userid, this.iterationid,
                                                                                 this.starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                                 this.endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                                 this.diff.ToString(), this.reponseCode,
                                                                                 this.responsesize);
        }
    }
}
