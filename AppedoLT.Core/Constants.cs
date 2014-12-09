using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net.Sockets;
using System.Collections;


namespace AppedoLT.Core
{
    public enum VariableType { Extractor = 0, File, String, Constant, Counter, Sql,RandomNumber,RandomString,Number,CurrentDate }

    public class Constants 
    {
        private string _password = "ss1t_l1c@ns@_k@y_p@ssw0rd";
        private string _salt = "ss1t_S@1t";
        private string _iv = "ssit1234products";
        private string _MACAddress = string.Empty;
        private string _executingAssplyFolder=string.Empty;
        private string _certificatePath = string.Empty;
        private string _dataFolderPath = string.Empty;
        private string _dataFolderPathMonitor = string.Empty;
        private int _recordConncetion=-1;
        private string _recodingIPAddress=string.Empty;
        private string _recodingPort = string.Empty;
        private int _requestTimeOut = 0;
        private string _uploadIPAddress = string.Empty;
        private string _uploadPort = string.Empty;
        private int _maxUser = 0;

        public string ChartsSummaryFileName = "chart_ summary.csv";
        public string ChartsAvgResponse = "chart_useravgresponse.csv";
        public string ReportSummayReportFileName="report_summaryreport.csv";
        public string ReportRequestSummayReportFileName = "report_requestsummaryreport.csv";
        public string ReportPageSummayReportFileName = "report_pagesummaryreport.csv";
        public string ReportContainerSummayReportFileName = "report_containersummaryreport.csv";
        public string ReportTransactionSummayReportFileName = "report_transactionsummaryreport.csv";
      
       
        public List<string> HttpPostContentType = null;
        public List<string> HttpMethods = null;

        #region Global Constants
        public bool IsReportGenerating = false;
        public DateTime ApplicationStartTime = new DateTime();
        public bool IsSystemDateTimeChanged = false;
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
        public string CertificatePath
        {
            get
            {
                if (_certificatePath == string.Empty)
                {
                    _certificatePath = ExecutingAssemblyLocation + ConfigurationManager.AppSettings["CertificateFile"];
                }
                return _certificatePath;
            }
            private set { }
        }
        public bool IsValidationScreenOpen = false;
        #endregion

        private static Constants _instance;

        public List<string> HeaderExcludeList = new List<string>();
        private string _loadgen = string.Empty;
        public string LoadGen
        {
            get
            {
                if (_loadgen == string.Empty)
                {
                    _loadgen = LocalIPAddress();
                }

                return _loadgen;
            }
        }
        public string DataFolderPath
        {
            get
            {
                if (_dataFolderPath == string.Empty)
                {
                    _dataFolderPath = ExecutingAssemblyLocation+"\\Data";
                }
                return _dataFolderPath;
            }
            private set { }
        }
        public string DataFolderPathMonitor
        {
            get
            {
                if (_dataFolderPathMonitor == string.Empty)
                {
                    _dataFolderPathMonitor = ExecutingAssemblyLocation + "\\DataMonitor";
                    if (Directory.Exists(_dataFolderPathMonitor) == false) Directory.CreateDirectory(_dataFolderPathMonitor);
                }
                return _dataFolderPathMonitor;
            }
            private set { }
        }
        public string RecodingIPAddress
        {
            get
            {
                if (_recodingIPAddress == string.Empty)
                {
                    _recodingIPAddress = ConfigurationSettings.AppSettings["RecordingIPAddress"];
                }
                return _recodingIPAddress;
            }
            private set { }
        }
        public string RecodingPort
        {
            get
            {
                if (_recodingPort == string.Empty)
                {
                    _recodingPort = ConfigurationSettings.AppSettings["RecordingPort"];
                }
                return _recodingPort;
            }
            private set { }
        }
        public string UploadIPAddress
        {
            get
            {
                if (_uploadIPAddress == string.Empty)
                {
                    _uploadIPAddress = ConfigurationSettings.AppSettings["uploadip"];
                }
                return _uploadIPAddress;
            }
            private set { }
        }
        public string UploadPort
        {
            get
            {
                if (_uploadPort == string.Empty)
                {
                    _uploadPort = ConfigurationSettings.AppSettings["uploadport"];
                }
                return _uploadPort;
            }
            private set { }
        }
        public string AppedoPort
        {
            get
            {
                if (_uploadPort == string.Empty)
                {
                    _uploadPort = ConfigurationSettings.AppSettings["appedoport"];
                }
                return _uploadPort;
            }
            private set { }
        }
        public string MACAddress
        {
            get
            {
                if (_MACAddress == string.Empty)
                {
                    try
                    {
                         _MACAddress = String.Empty;
                        System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_Processor");
                        System.Management.ManagementObjectCollection moc = mc.GetInstances();

                        foreach (System.Management.ManagementObject mo in moc)
                        {
                                _MACAddress = mo.Properties["ProcessorId"].Value.ToString();
                        }
                        mc = new System.Management.ManagementClass("Win32_BIOS");
                        moc = mc.GetInstances();
                        foreach (System.Management.ManagementObject mo in moc)
                        {
                                _MACAddress =_MACAddress+"_"+mo.Properties["SerialNumber"].Value.ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    return _MACAddress;
                }
                else
                {
                    return _MACAddress;
                }
            }
            private set { }
        }
        public int RecordConnection
        {
            get
            {
                if (_recordConncetion == -1)
                {
                    try
                    {
                        _recordConncetion = Convert.ToInt16(System.Configuration.ConfigurationSettings.AppSettings["RecordConncetion"]);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
                return _recordConncetion;
            }
        }
        public int RequestTimeOut
        {
            get
            {
                if (_requestTimeOut == 0)
                {
                    try
                    {
                        _requestTimeOut = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["RequstTimeout"]);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
                return _requestTimeOut;
            }
        }
        public int MaxUserCount
        {
            get
            {
                if (_maxUser == 0)
                {
                    try
                    {
                        _maxUser = Convert.ToInt32(Decrypt(System.Configuration.ConfigurationSettings.AppSettings["users"]));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        _maxUser = 25;
                    }
                }
                return _maxUser;
            }
        }
        public  string Encrypt(string raw)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                string encrypted = Convert.ToBase64String(output);

                return encrypted;
            }
        }
        public  string Decrypt(string encrypted)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);

                string decypted = Encoding.UTF8.GetString(decryptedOutput);
                return decypted;
            }
        }
        public XmlDocument GetLicenseDoc()
        {
            XmlDocument doc = new XmlDocument();
            using (FileStream fileStream = new FileStream(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\Floodgates.bin", FileMode.Open, FileAccess.Read))
            {
                StreamReader read = new StreamReader(fileStream);
                string lic = read.ReadToEnd();
                doc.LoadXml(Constants.GetInstance().Decrypt(lic));
            }
            return doc;
        }
        public void SetFirefoxProxy()
        {
            try
            {
                DirectoryInfo[] myProfileDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mozilla\\Firefox\\Profiles\\").GetDirectories("*.default");
                String myFFPrefFile = myProfileDirectory[0].FullName + "\\prefs.js";
                if (File.Exists(myFFPrefFile) == true)
                {
                    StreamReader myReader = new StreamReader(myFFPrefFile);
                    StringBuilder myPrefContents = new StringBuilder();
                    string temp;
                    bool isProxyExist = false;

                    while (myReader.EndOfStream == false)
                    {
                        temp = myReader.ReadLine();
                        if (temp.ToLower().Contains("network.proxy") == true) isProxyExist = true;


                        if (temp.ToLower().Contains("network.proxy.ftp_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ftp_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ftp"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ftp\", \"localhost\");");
                        }

                        else if (temp.ToLower().Contains("network.proxy.http_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.http_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.http"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.http\", \"localhost\");");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ssl_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ssl_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ssl"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ssl\", \"localhost\");");
                        }

                        else if (temp.ToLower().Contains("network.proxy.type"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.type\", 1);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.no_proxies_on"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.no_proxies_on\", \"\");");
                        }
                        else if (temp.ToLower().Contains("network.proxy.share_proxy_settings"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.share_proxy_settings\", true);");
                        }
                        else
                        {
                            myPrefContents.AppendLine(temp);
                        }
                    }
                    if (isProxyExist == false)
                    {
                        myPrefContents.AppendLine("user_pref(\"network.proxy.type\", 1);");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.ftp\", \"localhost\");");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.ftp_port\", 8010);");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.http\", \"localhost\");");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.http_port\", 8010);");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.ssl\", \"localhost\");");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.ssl_port\", 8010);");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.no_proxies_on\", \"\");");
                        myPrefContents.AppendLine("user_pref(\"network.proxy.share_proxy_settings\", true);");
                    }
                    myReader.Close();
                    File.Delete(myFFPrefFile);
                    File.WriteAllText(myFFPrefFile, myPrefContents.ToString());
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public void ReSetFirefoxProxy()
        {
            try
            {
                DirectoryInfo[] myProfileDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mozilla\\Firefox\\Profiles\\").GetDirectories("*.default");
                String myFFPrefFile = myProfileDirectory[0].FullName + "\\prefs.js";
                if (File.Exists(myFFPrefFile) == true)
                {
                    StreamReader myReader = new StreamReader(myFFPrefFile);
                    StringBuilder myPrefContents = new StringBuilder();
                    string temp;

                    while (myReader.EndOfStream == false)
                    {
                        temp = myReader.ReadLine();
                        if (temp.ToLower().Contains("network.proxy.ftp_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ftp_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ftp"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ftp\", \"localhost\");");
                        }

                        else if (temp.ToLower().Contains("network.proxy.http_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.http_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.http"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.http\", \"localhost\");");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ssl_port"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ssl_port\", 8010);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.ssl"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.ssl\", \"localhost\");");
                        }

                        else if (temp.ToLower().Contains("network.proxy.type"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.type\", 0);");
                        }
                        else if (temp.ToLower().Contains("network.proxy.no_proxies_on"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.no_proxies_on\", \"\");");
                        }
                        else if (temp.ToLower().Contains("network.proxy.share_proxy_settings"))
                        {
                            myPrefContents.AppendLine("user_pref(\"network.proxy.share_proxy_settings\", true);");
                        }
                        else
                        {
                            myPrefContents.AppendLine(temp);
                        }
                    }
                    myReader.Close();
                    File.Delete(myFFPrefFile);
                    File.WriteAllText(myFFPrefFile, myPrefContents.ToString());
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public void SaveLiceseDoc(XmlDocument doc)
        {
            string licXML = GetXMLAsString(doc);
            if (File.Exists(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\Floodgates.bin") == true) File.Delete(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\Floodgates.bin");
            using (FileStream licFile = new FileStream(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\Floodgates.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                string licStr = Encrypt(licXML);
                licFile.Write(Encoding.Default.GetBytes(licStr), 0, licStr.Length);
            }

        }
        public ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            
            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(_password), Encoding.UTF8.GetBytes(_salt), 65536);
            byte[] key = spec.GetBytes(16);


            csp.IV = Encoding.UTF8.GetBytes(_iv);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }
        private X509Certificate2 _certificate;
        public X509Certificate2 Certificate
        {
            get
            {
                if (ConfigurationManager.AppSettings["CertificateFile"] != null)
                {
                    try
                    {
                        _certificate = new X509Certificate2(CertificatePath, "pass@12345");
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
                return _certificate;
            }
            private set { }
        }
        public static Constants GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Constants();
            }
            return _instance;
        }
        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private Constants()
        {
            System.Windows.Forms.TextBox.CheckForIllegalCrossThreadCalls = false;
            HttpPostContentType = new List<string>();
            HttpMethods = new List<string>();
            HttpMethods.AddRange(new string[] { "GET","POST" });
            HttpPostContentType.AddRange(new string[] {"Form","Multipart/form-data","Text" });
            ServicePointManager.MaxServicePointIdleTime = 100;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            HeaderExcludeList.AddRange(new string[] { "Cookie", "Connection", "Accept", "Host", "User-Agent", "Referer", "Accept-Encoding", "Content-Type", "Content-Length", "Expect", "If-Modified-Since" });
        }
        private string GetXMLAsString(XmlDocument myxml)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            myxml.WriteTo(tx);
            string str = sw.ToString();
            return str;
        }
        public bool IsTimeChaged()
        {
            bool isChaged = false;
            try
            {
                EventLog log = new EventLog("System");
                long currentTicks = DateTime.Now.Ticks;

                foreach (EventLogEntry entry in log.Entries.Cast<EventLogEntry>().Reverse())
                {
                    if (entry.TimeGenerated.Ticks > currentTicks)
                    {
                        isChaged = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            return isChaged;
        }
        public void ExecuteBat(string batchFile)
        {
            int ExitCode;
            ProcessStartInfo ProcessInfo;
            Process Process;
            ProcessInfo = new ProcessStartInfo(batchFile);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;

            Process = Process.Start(ProcessInfo);
            Process.WaitForExit();

            ExitCode = Process.ExitCode;
            Process.Close();
           
        }
        public DataTable GetDataTableFromCSVFile(string FilePath, string Delim)
        {
            DataTable tbl = new DataTable();
            string CsvData = string.Empty;
            CsvData = File.ReadAllText(FilePath);
            bool firstRow = true;
            foreach (string row in CsvData.Split("\n".ToCharArray()))
            {
                DataRow dr = tbl.NewRow();
                System.Text.RegularExpressions.RegexOptions options = (
                    System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace
                  | System.Text.RegularExpressions.RegexOptions.Multiline
                  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Regex reg = new Regex(Delim
                  + @"(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))", options);
                var csvArray = reg.Split(row.Replace("\n", "").Replace("\r", ""));
                for (int i = 0; i < csvArray.Length; i++)
                {
                    csvArray[i] = csvArray[i].Replace("\"\"", "");
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = csvArray[i] });
                    else
                        dr[i] = csvArray[i];

                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            return tbl;
        }
        public DataTable GetDataTableFromCSVFile(string FilePath)
        {
            DataTable tbl = new DataTable();
            string CsvData = string.Empty;
            CsvData = File.ReadAllText(FilePath);
            bool firstRow = true;
            foreach (string row in CsvData.Split("\n".ToCharArray()))
            {
                DataRow dr = tbl.NewRow();
                System.Text.RegularExpressions.RegexOptions options = (
                    System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace
                  | System.Text.RegularExpressions.RegexOptions.Multiline
                  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Regex reg = new Regex(","
                  + @"(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))", options);
                var csvArray = reg.Split(row.Replace("\n", "").Replace("\r", ""));
                for (int i = 0; i < csvArray.Length; i++)
                {
                    csvArray[i] = csvArray[i].Replace("\"\"", "");
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = csvArray[i] });
                    else
                        dr[i] = csvArray[i];

                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            return tbl;
        }
        public DataTable GetDataTableFromCSV(string CsvData)
        {
            DataTable tbl = new DataTable();
            bool firstRow = true;
            foreach (string row in CsvData.Split("\n".ToCharArray()))
            {
                DataRow dr = tbl.NewRow();
                System.Text.RegularExpressions.RegexOptions options = (
                    System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace
                  | System.Text.RegularExpressions.RegexOptions.Multiline
                  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Regex reg = new Regex(","
                  + @"(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))", options);
                var csvArray = reg.Split(row.Replace("\n", "").Replace("\r", ""));
                for (int i = 0; i < csvArray.Length; i++)
                {
                    csvArray[i] = csvArray[i].Replace("\"\"", "");
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = csvArray[i] });
                    else
                        dr[i] = csvArray[i];

                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            return tbl;
        }
        public string ReceiveGZipHeader(Stream stream)
        {
            StringBuilder header = new StringBuilder();
            byte[] bytes = new byte[10];
            while (stream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.Default.GetString(bytes, 0, 1));
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n"))
                    break;
            }
            return header.ToString();
        }
        public string ReadHeader(Stream stream)
        {
            StringBuilder header = new StringBuilder();
            byte[] bytes = new byte[10];
            StringBuilder response = new StringBuilder();

            while (stream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.Default.GetString(bytes, 0, 1));
                response.Append(Encoding.Default.GetString(bytes, 0, 1));

                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n\r\n"))
                    break;
            }
            return header.ToString();
        }
        public string GetUniqueContainerName(string input, XmlNode script, int times)
        {
            if (script.SelectSingleNode(".//container[@name='" + input + "_" + times.ToString() + "']") != null)
            {
              return  GetUniqueContainerName(input, script, ++times);
            }
            else
            {
                return input + "_" + times.ToString();
            }
        }
        public string GetUniqueLoopName(string input, XmlNode script, int times)
        {
            if (script.SelectSingleNode(".//loop[@name='" + input + "_" + times.ToString() + "']") != null)
            {
                return GetUniqueLoopName(input, script, ++times);
            }
            else
            {
                return input + "_" + times.ToString();
            }
        }
        public XmlNode FindThirdRoot(XmlNode node)
        {
            try
            {
                if (node.ParentNode.ParentNode.ParentNode.ParentNode == null)
                {
                    return node;
                }
                else
                {
                    return FindThirdRoot(node.ParentNode);
                }
            }
            catch
            {
                return node;
            }
        }
        public  string GetPageContent(string Url)
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
        public  string GetPageContent(string Url,string data)
        {
            string PageContent = string.Empty;
            HttpWebRequest WebRequestObject=null;
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
            catch(Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            finally
            {
                WebRequestObject=null;
            }
            return PageContent;
        }
        public string GetQuery(string reportName,XmlDocument doc)
        {
            StringBuilder result = new StringBuilder();
            XmlNode runNode = doc.SelectSingleNode("//run[@reportname='" + reportName + "']");
            if (runNode != null)
            {

                foreach (XmlNode loadgen in runNode.SelectNodes("loadgen"))
                {
                    result.AppendFormat(@"
                                           ATTACH '{0}\database_{1}.db' AS AM;
                                           INSERT INTO reportdata SELECT * FROM AM.reportdata;
                                           INSERT INTO transactions SELECT * FROM AM.transactions;
                                           INSERT INTO log SELECT * FROM AM.log;
                                           INSERT INTO error SELECT * FROM AM.error;
                                           DETACH DATABASE 'AM';
                                         ", this.ExecutingAssemblyLocation + "\\data\\" + reportName + "", loadgen.Attributes["ipaddress"].Value.Replace('.', '_'));
                }

                result.AppendLine(@" CREATE INDEX idx_reportdata ON reportdata(scriptid);
                                           CREATE INDEX idx_reportdata_requestid ON reportdata(requestid);
                                           CREATE INDEX idx_scriptname ON error(scriptname );");
                foreach (XmlNode script in runNode.SelectSingleNode("scripts").ChildNodes)
                {
                    #region Script Query
                    string rampuptime = "2013-12-16 20:05:00";
                    result.AppendFormat(@"
                                                           CREATE VIEW containerresponsetime_{0} AS
                                                                                  SELECT scriptid,
                                                                                         userid,
                                                                                         iterationid,
                                                                                         containerid,
                                                                                         containername,
                                                                                         sum( diff ) AS responsetime,
                                                                                         min( starttime ) AS createddate
                                                                                   FROM reportdata
                                                                                   where scriptid={0}
                                                                                   GROUP BY scriptid,
                                                                                            userid,
                                                                                            iterationid,
                                                                                           containerid;                                               
               
                                                           CREATE TABLE requests_{0} ( 
                                                                                   containerid   INT,
                                                                                   containername VARCHAR,
                                                                                   requestid     INT,
                                                                                   address       VARCHAR 
                                                                                  );
                
                                                           CREATE TABLE requestresponse_{0} ( 
                                                                                   containerid   INT,
                                                                                   containername VARCHAR,
                                                                                   requestid     INT,
                                                                                   address       VARCHAR,
                                                                                   min           DOUBLE,
                                                                                   max           DOUBLE,
                                                                                   avg           DOUBLE, 
                                                                                   throughput    DOUBLE,
                                                                                   hitcount      INT
                                                                                   );
                
                                                          CREATE TABLE transactions_{0} ( 
                                                                                   transactionname VARCHAR,
                                                                                   min           DOUBLE,
                                                                                   max           DOUBLE,
                                                                                   avg           DOUBLE 
                                                                                   );
                
                                                           CREATE TABLE containerresponse_{0} ( 
                                                                                   containerid   INT,
                                                                                   containername VARCHAR,
                                                                                   min           DOUBLE,
                                                                                   max           DOUBLE,
                                                                                   avg           DOUBLE 

                                                                                   );
                                                        
                                                           CREATE TABLE errorcount_{0} ( 
                                                                                   containerid   INT,
                                                                                   containername VARCHAR,
                                                                                   requestid     INT,
                                                                                   address       VARCHAR,
                                                                                   count         INT 
                                                                                   );
                
                                                           CREATE TABLE errorcode_{0} ( 
                                                                                   errorcode   VARCHAR,
                                                                                   message     VARCHAR,
                                                                                   count       INT
                                                                                   );
                
                                                           insert into requests_{0} select containerid,containername, requestid,address from reportdata where scriptid={0} group by containerid,containername,requestid order by containerid,requestid;
                                                           insert into requestresponse_{0} select containerid,containername,requestid,address,min(diff),max(diff),avg(diff),sum(responsesize),count(diff)  from reportdata where scriptid={0} group by containerid,requestid order by containerid,requestid;
                                                           insert into containerresponse_{0} select containerid, containername,min(responsetime) AS min,max(responsetime) AS max,avg(responsetime) AS avg from containerresponsetime_{0} group by containerid order by containerid;
                                                           insert into transactions_{0} select transactionname,min(difference),max(difference),avg(difference) from transactions where scriptid={0} group by transactionname;
                                                           insert into errorcount_{0} select containerid,containername, error.requestid, address,count(*) from error left outer join requests_{0} on error.requestid=requests_{0}.requestid where error.scriptname='{1}' group by error.requestid order by error.requestid;
                                                           insert into errorcode_{0} select errorcode,message,count(*) from error where error.scriptname='{1}' group by message;", script.Attributes["id"].Value, script.Attributes["name"].Value, rampuptime).AppendLine();
                    #endregion
                }
                result.Append("CREATE INDEX idx_responsecode ON reportdata(reponsecode);");
                result.Append(@" insert into summaryreport SELECT 
                                                                         MIN(starttime) AS start_time,
                                                                         MAX(endtime) AS end_time,
                                                                         (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) as duration_sec,
                                                                         (SELECT COUNT(*) AS user_count FROM (SELECT userid,scriptid FROM reportdata  GROUP BY userid,scriptid) AS a) AS user_count,
                                                                          count(*) AS total_hits,
                                                                          IFNULL(AVG(diff)*1.0/1000,0) AS avg_response,
                                                                          CASE WHEN (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) = 0 THEN 0
                                                                               ELSE count(*)*1.0/(strftime('%s',max(starttime)) - strftime('%s',min(starttime))) END AS avg_hits,
                                                                          IFNULL(ROUND((SUM(responsesize)*1.0/1024)/1024,3),0) AS total_throughput,
                                                                          CASE WHEN (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) = 0 THEN 0
                                                                               ELSE ((SUM(responsesize)*8.0)/(strftime('%s',max(starttime)) - strftime('%s',min(starttime))))/1024/1024 END AS avg_throughput,
                                                                               (select count(*)from error ) AS total_errors,
                                                                          (select count(*) from (SELECT pageid from reportdata group by userid,iterationid,pageid))as total_page,
                                                                          (select IFNULL(AVG(pageresponse)*1.0/1000,0) from (select sum(diff)as pageresponse from reportdata  group by userid,iterationid,pageid))AS avg_page_response,
                                                                          (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=200 AND reponsecode<300 ) AS reponse_200,
                                                                          (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=300 AND reponsecode<400 ) AS reponse_300,
                                                                          (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=400 AND reponsecode<500 ) AS reponse_400,
                                                                          (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=500 AND reponsecode<600 ) AS reponse_500
                                                                      FROM 
                                                                          reportdata;");
            }
            return result.ToString();
        }
    }

    public class Tuple<T1, T2>
    {
        public T1 Key { get; set; }
        public T2 Value { get; set; }
        public Tuple(T1 first, T2 second)
        {
            Key = first;
            Value = second;
        }
        public Tuple()
        {
        }
    }

    public class Parameter
    {
        public string parameterid = string.Empty;
        public Parameter()
        {
            this.parameterid = System.Guid.NewGuid().ToString();
        }
        public string RawParameterName;
        public string RawParameterValue;
        public string Name { get; set; }
        public string Value
        {
            get ;

            set;
        }

        //Type=1 for query string paramer, 2 for Post parameter, 3 for multi-form data.
        public string Type;
        public string Boundary = string.Empty;
        public string ContentDisposition = string.Empty;
        public string FileName = string.Empty;
        public string ContentType = string.Empty;

        public Parameter Copy()
        {
            Parameter parm = new Parameter();
            parm.parameterid = this.parameterid;
            parm.RawParameterName = this.RawParameterName;
            parm.RawParameterValue = this.RawParameterValue;
            parm.Name = this.Name;
            parm.Value = this.Value;
            parm.Type = this.Type;
            parm.Boundary = this.Boundary;
            parm.ContentDisposition = this.ContentDisposition;
            parm.FileName = this.FileName;
            parm.ContentType = this.ContentType;

            return parm;
        }
    }

    public class VUScriptSetting : ICloneable
    {
        public string VUScriptid { get; set; }
        //1=Iteration 2=Duration
        public string Type { get; set; }
        public string DurationTime { get; set; }
        public string Iterations { get; set; }
        public string StartUser { get; set; }
        public string IncrementUser { set; get; }
        public string IncrementTime { set; get; }
        public string MaxUser { set; get; }
        public string ScenarioId { get; set; }
        public bool BrowserCache { get; set; }
        public int StartUserId { get; set; }
     
        public static VUScriptSetting GetDefault(string scriptId)
        {
            VUScriptSetting vUScriptSetting = new VUScriptSetting();
            vUScriptSetting.VUScriptid = scriptId;
            vUScriptSetting.Type = "1";
            vUScriptSetting.DurationTime = "0;0;0";
            vUScriptSetting.IncrementTime = "0;0;0";
            vUScriptSetting.Iterations = "1";
            vUScriptSetting.MaxUser = "1";
            vUScriptSetting.StartUser = "1";
            vUScriptSetting.IncrementUser = "1";
          
            vUScriptSetting.ScenarioId = string.Empty;
            vUScriptSetting.BrowserCache = false;
            vUScriptSetting.StartUserId = 0;
            return vUScriptSetting;

        }

        public static VUScriptSetting GetDefault(string scriptId, string scenarioId)
        {
            VUScriptSetting vUScriptSetting = new VUScriptSetting();
            vUScriptSetting.VUScriptid = scriptId;
            vUScriptSetting.Type = "1";
            vUScriptSetting.DurationTime = "0;0;0";
            vUScriptSetting.IncrementTime = "0;0;0";
            vUScriptSetting.Iterations = "1";
            vUScriptSetting.MaxUser = "1";
            vUScriptSetting.StartUser = "1";
            vUScriptSetting.IncrementUser = "1";
            vUScriptSetting.ScenarioId = scenarioId;
            vUScriptSetting.BrowserCache = false;
            vUScriptSetting.StartUserId = 0;
            
            return vUScriptSetting;

        }

        public double GetVUCreationIntervel()
        {
            return (new TimeSpan(int.Parse(IncrementTime.Split(';')[0]), int.Parse(IncrementTime.Split(';')[1]), int.Parse(IncrementTime.Split(';')[2]))).TotalMilliseconds;
        }

        public double GetVUDutaionIntervel()
        {
            return (new TimeSpan(int.Parse(DurationTime.Split(';')[0]), int.Parse(DurationTime.Split(';')[1]), int.Parse(DurationTime.Split(';')[2]))).TotalMilliseconds;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class HtmlTag
    {
        /// <summary>
        /// Name of this tag
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of attribute names and values for this tag
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// True if this tag contained a trailing forward slash
        /// </summary>
        public bool TrailingSlash { get; set; }
    };

    public class HtmlParser
    {
        protected string _html;
        protected int _pos;
        protected bool _scriptBegin;

        public HtmlParser(string html)
        {
            Reset(html);
        }

        /// <summary>
        /// Resets the current position to the start of the current document
        /// </summary>
        public void Reset()
        {
            _pos = 0;
        }

        /// <summary>
        /// Sets the current document and resets the current position to the
        /// start of it
        /// </summary>
        /// <param name="html"></param>
        public void Reset(string html)
        {
            _html = html;
            _pos = 0;
        }

        /// <summary>
        /// Indicates if the current position is at the end of the current
        /// document
        /// </summary>
        public bool EOF
        {
            get { return (_pos >= _html.Length); }
        }

        /// <summary>
        /// Parses the next tag that matches the specified tag name
        /// </summary>
        /// <param name="name">Name of the tags to parse ("*" = parse all
        /// tags)</param>
        /// <param name="tag">Returns information on the next occurrence
        /// of the specified tag or null if none found</param>
        /// <returns>True if a tag was parsed or false if the end of the
        /// document was reached</returns>
        public bool ParseNext(string name, out HtmlTag tag)
        {
            tag = null;

            // Nothing to do if no tag specified
            if (String.IsNullOrEmpty(name))
                return false;

            // Loop until match is found or there are no more tags
            while (MoveToNextTag())
            {
                // Skip opening '<'
                Move();

                // Examine first tag character
                char c = Peek();
                if (c == '!' && Peek(1) == '-' && Peek(2) == '-')
                {
                    // Skip over comments
                    const string endComment = "-->";
                    _pos = _html.IndexOf(endComment, _pos);
                    NormalizePosition();
                    Move(endComment.Length);
                }
                else if (c == '/')
                {
                    // Skip over closing tags
                    _pos = _html.IndexOf('>', _pos);
                    NormalizePosition();
                    Move();
                }
                else
                {
                    // Parse tag
                    bool result = ParseTag(name, ref tag);

                    // Because scripts may contain tag characters,
                    // we need special handling to skip over
                    // script contents
                    if (_scriptBegin)
                    {
                        const string endScript = "</script";
                        _pos = _html.IndexOf(endScript, _pos,
                          StringComparison.OrdinalIgnoreCase);
                        NormalizePosition();
                        Move(endScript.Length);
                        SkipWhitespace();
                        if (Peek() == '>')
                            Move();
                    }

                    // Return true if requested tag was found
                    if (result)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the contents of an HTML tag. The current position should
        /// be at the first character following the tag's opening less-than
        /// character.
        /// 
        /// Note: We parse to the end of the tag even if this tag was not
        /// requested by the caller. This ensures subsequent parsing takes
        /// place after this tag
        /// </summary>
        /// <param name="name">Name of the tag the caller is requesting,
        /// or "*" if caller is requesting all tags</param>
        /// <param name="tag">Returns information on this tag if it's one
        /// the caller is requesting</param>
        /// <returns>True if data is being returned for a tag requested by
        /// the caller or false otherwise</returns>

        protected bool ParseTag(string name, ref HtmlTag tag)
        {
            // Get name of this tag
            string s = ParseTagName();

            // Special handling
            bool doctype = _scriptBegin = false;
            if (String.Compare(s, "!DOCTYPE", true) == 0)
                doctype = true;
            else if (String.Compare(s, "script", true) == 0)
                _scriptBegin = true;

            // Is this a tag requested by caller?
            bool requested = false;
            if (name == "*" || String.Compare(s, name, true) == 0)
            {
                // Yes, create new tag object
                tag = new HtmlTag();
                tag.Name = s;
                tag.Attributes = new Dictionary<string, string>();
                requested = true;
            }

            // Parse attributes
            SkipWhitespace();
            while (Peek() != '>')
            {
                if (Peek() == '/')
                {
                    // Handle trailing forward slash
                    if (requested)
                        tag.TrailingSlash = true;
                    Move();
                    SkipWhitespace();
                    // If this is a script tag, it was closed
                    _scriptBegin = false;
                }
                else
                {
                    // Parse attribute name
                    s = (!doctype) ? ParseAttributeName() : ParseAttributeValue();
                    SkipWhitespace();
                    // Parse attribute value
                    string value = String.Empty;
                    if (Peek() == '=')
                    {
                        Move();
                        SkipWhitespace();
                        value = ParseAttributeValue();
                        SkipWhitespace();
                    }
                    // Add attribute to collection if requested tag
                    if (requested)
                    {
                        // This tag replaces existing tags with same name
                        if (tag.Attributes.Keys.Contains(s))
                            tag.Attributes.Remove(s);
                        tag.Attributes.Add(s, value);
                    }
                }
            }
            // Skip over closing '>'
            Move();

            return requested;
        }

        /// <summary>
        /// Parses a tag name. The current position should be the first
        /// character of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseTagName()
        {
            int start = _pos;
            while (!EOF && !Char.IsWhiteSpace(Peek()) && Peek() != '>')
                Move();
            return _html.Substring(start, _pos - start);
        }

        /// <summary>
        /// Parses an attribute name. The current position should be the
        /// first character of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseAttributeName()
        {
            int start = _pos;
            while (!EOF && !Char.IsWhiteSpace(Peek()) && Peek() != '>'
              && Peek() != '=')
                Move();
            return _html.Substring(start, _pos - start);
        }

        /// <summary>
        /// Parses an attribute value. The current position should be the
        /// first non-whitespace character following the equal sign.
        /// 
        /// Note: We terminate the name or value if we encounter a new line.
        /// This seems to be the best way of handling errors such as values
        /// missing closing quotes, etc.
        /// </summary>
        /// <returns>Returns the parsed value string</returns>
        protected string ParseAttributeValue()
        {
            int start, end;
            char c = Peek();
            if (c == '"' || c == '\'')
            {
                // Move past opening quote
                Move();
                // Parse quoted value
                start = _pos;
                _pos = _html.IndexOfAny(new char[] { c, '\r', '\n' }, start);
                NormalizePosition();
                end = _pos;
                // Move past closing quote
                if (Peek() == c)
                    Move();
            }
            else
            {
                // Parse unquoted value
                start = _pos;
                while (!EOF && !Char.IsWhiteSpace(c) && c != '>')
                {
                    Move();
                    c = Peek();
                }
                end = _pos;
            }
            return _html.Substring(start, end - start);
        }

        /// <summary>
        /// Moves to the start of the next tag
        /// </summary>
        /// <returns>True if another tag was found, false otherwise</returns>

        protected bool MoveToNextTag()
        {
            _pos = _html.IndexOf('<', _pos);
            NormalizePosition();
            return !EOF;
        }

        /// <summary>
        /// Returns the character at the current position, or a null
        /// character if we're at the end of the document
        /// </summary>
        /// <returns>The character at the current position</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Returns the character at the specified number of characters
        /// beyond the current position, or a null character if the
        /// specified position is at the end of the document
        /// </summary>
        /// <param name="ahead">The number of characters beyond the
        /// current position</param>
        /// <returns>The character at the specified position</returns>
        public char Peek(int ahead)
        {
            int pos = (_pos + ahead);
            if (pos < _html.Length)
                return _html[pos];
            return (char)0;
        }

        /// <summary>
        /// Moves the current position ahead one character
        /// </summary>
        protected void Move()
        {
            Move(1);
        }

        /// <summary>
        /// Moves the current position ahead the specified number of characters
        /// </summary>
        /// <param name="ahead">The number of characters to move ahead</param>
        protected void Move(int ahead)
        {
            _pos = Math.Min(_pos + ahead, _html.Length);
        }

        /// <summary>
        /// Moves the current position to the next character that is
        // not whitespace
        /// </summary>
        protected void SkipWhitespace()
        {
            while (!EOF && Char.IsWhiteSpace(Peek()))
                Move();
        }

        /// <summary>
        /// Normalizes the current position. This is primarily for handling
        /// conditions where IndexOf(), etc. return negative values when
        /// the item being sought was not found
        /// </summary>
        protected void NormalizePosition()
        {
            if (_pos < 0)
                _pos = _html.Length;
        }
    }
}
