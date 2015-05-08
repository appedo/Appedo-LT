using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppedoLT.Core
{
  public  class RequestResponse
    {
        public Request RequestResult;

        public int WebRequestResponseId { get; set; }
        public string RequestId { get; set; }
        public string ContainerName { get; set; }
        public HttpWebRequest Request = null;
        public string PostData;
        public string ResponseCode = string.Empty;
        public string Response;
        public List<Parameter> Parameters = new List<Parameter>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime
        { get; set; }
        public double Duration
        {
            get;
            set;
        }
        public bool IsSucess { get; set; }
        public string TcpIPRequest { get; set; }
        public string TcpIPResponse { get; set; }
    }
}
