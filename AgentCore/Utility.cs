using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

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

    }
}
