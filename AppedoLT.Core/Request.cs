using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT.Core
{
    public abstract class Request
    {
        protected Constants _Constants = Constants.GetInstance();
        protected Stopwatch responseTime;
        protected string _responseStr = string.Empty;
        protected int _bufferSize = 8192;
        protected MemoryStream ResponseStream;
        private int _requestTimeOut;
        private List<Tuple<string, string>> _variables = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> _extractedVariables = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> _parameters = new List<Tuple<string, string>>();

        public int RequestTimeOut
        {
            get
            {
                if (_requestTimeOut == 0)
                {
                    _requestTimeOut = _Constants.RequestTimeOut;
                }
               
                return _requestTimeOut;
            }
            private set { }
        }
        public int RequestId { get; set; }
        public string RequestName
        {
            get
            {
                if (RequestNode.Attributes["Path"] == null) return RequestNode.Attributes["name"].Value;
                else return RequestNode.Attributes["Path"].Value;
            }
            private set { }
        }
        public XmlNode RequestNode { get; set; }
        public int ResponseCode { get; set; }
        public long ResponseSize { get; set; }
        public string ResponseStr
        {
            get
            {
                if (_responseStr == string.Empty)
                {
                    if (ResponseStream != null && ResponseStream.Length > 0)
                    {
                        _responseStr = Encoding.ASCII.GetString(ResponseStream.ToArray(), 0, (int)ResponseStream.Length);
                    }
                    return _responseStr;
                }
                else
                {
                    return _responseStr;
                }
            }
            private set { }
        }
        public bool Success { get; set; }
        public bool AssertionResult { get; set; }
        public StringBuilder AssertionFaildMsg = new StringBuilder();
        public StringBuilder AssertionSucessMsg = new StringBuilder();
        public double ResponseTime
        {
            get
            {
                return responseTime.Elapsed.TotalMilliseconds;
            }
            private set
            {
            }
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime
        {

            get
            {
                return StartTime.AddTicks(responseTime.Elapsed.Ticks);
            }
            private set { }
        }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool StoreRequestBody { get; set; }
        public List<Tuple<string, string>> Variables { get { return _variables; } set { _variables = value; } }
        public List<Tuple<string, string>> ExtractedVariables { get { return _extractedVariables; } set { _extractedVariables = value; } }
        public List<Tuple<string, string>> Parameters { get { return _parameters; } set { _parameters = value; } }

        public abstract void GetResponse();
        public abstract void Abort();
        public abstract void PerformAssertion();
       
        private static List<IPAddress> _SystemIPs = new List<IPAddress>();
        private static int _ipaddressIndex = 0;
        private static bool _IPSpoofingEnabled = false;
        public static bool IPSpoofingEnabled
        {
            get { return _IPSpoofingEnabled; }
            set
            {
                _IPSpoofingEnabled = value;
                _SystemIPs.Clear();
                String strHostName = Dns.GetHostName();
                IPHostEntry iphostentry = Dns.GetHostByName(strHostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    _SystemIPs.Add(ipaddress);
                    if (_IPSpoofingEnabled == false) break;
                }
            }
        }
        public static IPAddress GetIPAddress(int userid)
        {
            lock (_SystemIPs)
            {
                if (_SystemIPs.Count == 0)
                {
                    String strHostName = Dns.GetHostName();
                    IPHostEntry iphostentry = Dns.GetHostByName(strHostName);
                    foreach (IPAddress ipaddress in iphostentry.AddressList)
                    {
                        _SystemIPs.Add(ipaddress);
                        if (IPSpoofingEnabled == false) break;
                    }
                }
                userid = userid - 1;
                return _SystemIPs[userid % _SystemIPs.Count];
            }
        }
        ~Request()
        {
            ResponseStream = null;
            Variables = null;
            Parameters = null;
            RequestNode = null;
            _variables = null;
            _extractedVariables = null;
            _parameters = null;
        }

    }
}
