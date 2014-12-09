using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public class TransactionRunTimeDetail
    {
        public string ScriptId = string.Empty;
        public string ScenarioName = string.Empty;
        public string ScriptName = string.Empty;
        public string UserId = string.Empty;
        public string IterationId = string.Empty;
        public string TransactionName = string.Empty;
        public DateTime StartTime = new DateTime();
        public DateTime EndTime = new DateTime();
        public bool IsEnd = false;
        public double Difference
        {
            get
            {
                if(IsEnd==true)
                {
                    TimeSpan diff = EndTime - StartTime;
                    return diff.TotalMilliseconds;
                }
                else
                {
                    return 0;
                }
            }
        }
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
