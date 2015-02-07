using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AgentCore
{
    public class Utility
    {
        private string _executingAssplyFolder = string.Empty;
        private static Utility _instance;
        public static Utility GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Utility();
            }
            return _instance;
        }
        public string ExecutingAssemblyLocation
        {
            get
            {
                if (_executingAssplyFolder == string.Empty)
                {
                    _executingAssplyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                return _executingAssplyFolder;
            }
            private set { }
        }
        public string GetPageContent(string Url)
        {

            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);
            WebResponse Response = WebRequestObject.GetResponse();
            Stream WebStream = Response.GetResponseStream();
            StreamReader objReader = new StreamReader(WebStream);
            string PageContent = objReader.ReadToEnd();
            objReader.Close();
            WebStream.Close();
            Response.Close();
            return PageContent;
        }
        public string GetPageContent(string Url, string data)
        {
            string PageContent = string.Empty;
            HttpWebRequest WebRequestObject = null;
            try
            {
                WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);
                WebRequestObject.Method = "POST";
                WebRequestObject.ContentLength = data.Length;
                using (Stream stream = WebRequestObject.GetRequestStream())
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(data), 0, data.Length);
                }
                WebResponse Response = WebRequestObject.GetResponse();
                Stream WebStream = Response.GetResponseStream();
                StreamReader objReader = new StreamReader(WebStream);
                PageContent = objReader.ReadToEnd();
                objReader.Close();
                WebStream.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            finally
            {
                WebRequestObject = null;
            }
            return PageContent;
        }
        public CountersDetail GetCountersDetail(string responseStr)
        {
            CountersDetail res = Deserialise<CountersDetail>(responseStr);
            return res;
        }

        public T Deserialise<T>(string json)
        {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                T result = (T)deserializer.ReadObject(stream);
                return result;
            }
        }

        public byte[] Serialise<T>(T obj)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream1, obj);
            stream1.Seek(0, SeekOrigin.Begin);
            return stream1.ToArray();
        }
    }

    [DataContract]
    public class CountersDetail
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }
        [DataMember(Name = "failure")]
        public bool failure { get; set; }
        [DataMember(Name = "newCounterSet")]
        public newCounterSet[] newCounterSet { get; set; }

    }

    [DataContract]
    public class newCounterSet
    {
        [DataMember(Name = "counter_id")]
        public int counter_id { get; set; }

        [DataMember(Name = "query")]
        public string query { get; set; }
    }

    [DataContract]
    public class ParentCounterList
    {
        [DataMember(Name = "parentcounter")]
        public List<ParentCounter> ParentCounter { get; set; }
    }

    [DataContract]
    public class ParentCounter
    {
        [DataMember(Name = "parentcounterid")]
        public string ParentCounterId { get; set; }

        [DataMember(Name = "childcounterdetail")]
        public List<ChildCounterDetail> ChildCounterDetail { get; set; }
    }

    [DataContract]
    public class ChildCounterDetail
    {
        public bool HasInstace { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
        
        public string Name { get; set; }

        [DataMember(Name = "countername")]
        public string CounterName { get { return Name + "-" + InstanceName; } set { } }

        [DataMember(Name = "instancename")]
        public string InstanceName { get; set; }
       
        [DataMember(Name = "query")]
        public string query
        {
            get
            {
                return new StringBuilder().Append(HasInstace.ToString()).Append(",").Append(Category).Append(",").Append(Name).Append(",").Append(InstanceName).ToString();
            }
            set
            {
                string[] val = value.Split(',');
                Regex regex = new Regex("(.*),(.*),(.*),(.*)");
                Match match = null;
                match = regex.Match(value);
                if (val.Length > 1)
                {
                    HasInstace = Convert.ToBoolean(match.Groups[1].Value);
                    Name =match.Groups[2].Value;
                    InstanceName =match.Groups[3].Value;
                }
                else
                {
                    HasInstace = false;
                    Name = string.Empty;
                    InstanceName = string.Empty;
                }
            }
        }
    }
}
