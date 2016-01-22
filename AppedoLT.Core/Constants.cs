using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace AppedoLT.Core
{
    public enum VariableType { Extractor = 0, File, String, Constant, Counter, Sql, RandomNumber, RandomString, Number, CurrentDate }

    public delegate void LockReportData(ReportData data);
    public delegate void LockLog(Log data);
    public delegate void LockError(RequestException data);
    public delegate void LockTransactions(TransactionRunTimeDetail data);
    public delegate void LockUserDetail(UserDetail data);
    public delegate void LockRequestResponse(RequestResponse data);
    public delegate void IterationCompleted(string scriptName, int userid, int iterationid);
    public delegate void VUserRunCompleted(string scriptName, int userid);
    public delegate void VUserCreated(string scriptName, int userid);

    public class Constants
    {
        private string _password = "ss1t_l1c@ns@_k@y_p@ssw0rd";
        private string _salt = "ss1t_S@1t";
        private string _iv = "ssit1234products";
        private string _MachineUniqueID = string.Empty;
        private string _executingAssplyFolder = string.Empty;
        private string _certificatePath = string.Empty;
        private string _dataFolderPath = string.Empty;
        private string _dataFolderPathMonitor = string.Empty;
        private int _recordConncetion = -1;
        private string _recodingIPAddress = string.Empty;
        private int _recodingHttpsPort = 0;
        private string _recodingPort = string.Empty;
        private int _requestTimeOut = 0;
        private string _uploadIPAddress = string.Empty;
        private string _uploadPort = string.Empty;
        private int _maxUser = 0;
        private int _uniqueID = 0;
        private object _logObj = new object();
        private DateTime _dateTime = new DateTime(2000, 1, 1);

        public string btnExecutionType = "Validate";

        public string ChartsSummaryFileName = "chart_ summary.csv";
        public string ChartsAvgResponse = "chart_useravgresponse.csv";
        public string ReportSummayReportFileName = "report_summaryreport.csv";
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
                    _dataFolderPath = ExecutingAssemblyLocation + "\\Data";
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
                    _recodingIPAddress = System.Configuration.ConfigurationManager.AppSettings["RecordingIPAddress"];
                }
                return _recodingIPAddress;
            }
            private set { }
        }

        public int RecodingHttpsPort
        {
            get
            {
                if (_recodingHttpsPort == 0)
                {
                    _recodingHttpsPort = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["RecodingHttpsPort"]);
                }
                return _recodingHttpsPort;
            }
            private set { }
        }
        public string RecodingPort
        {
            get
            {
                if (_recodingPort == string.Empty)
                {
                    _recodingPort = System.Configuration.ConfigurationManager.AppSettings["RecordingPort"];
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
                    _uploadIPAddress = System.Configuration.ConfigurationManager.AppSettings["uploadip"];
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
                    _uploadPort = System.Configuration.ConfigurationManager.AppSettings["uploadport"];
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
                    _uploadPort = System.Configuration.ConfigurationManager.AppSettings["appedoport"];
                }
                return _uploadPort;
            }
            private set { }
        }
        public string MACAddress
        {
            get
            {
                if (_MachineUniqueID == string.Empty)
                {
                    try
                    {
                        _MachineUniqueID = String.Empty;
                        System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_Processor");
                        System.Management.ManagementObjectCollection moc = mc.GetInstances();

                        foreach (System.Management.ManagementObject mo in moc)
                        {
                            _MachineUniqueID = mo.Properties["ProcessorId"].Value.ToString();
                        }
                        mc = new System.Management.ManagementClass("Win32_BIOS");
                        moc = mc.GetInstances();
                        foreach (System.Management.ManagementObject mo in moc)
                        {
                            _MachineUniqueID = _MachineUniqueID + "_" + mo.Properties["SerialNumber"].Value.ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    return _MachineUniqueID;
                }
                else
                {
                    return _MachineUniqueID;
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
                        _recordConncetion = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["RecordConncetion"]);
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
                        _requestTimeOut = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RequstTimeout"]);
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
                        _maxUser = Convert.ToInt32(Decrypt(System.Configuration.ConfigurationManager.AppSettings["users"]));
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
        public string Encrypt(string raw)
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
        public string Decrypt(string encrypted)
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
                // to find the firefox process by name & then to close
                Process[] AllProcesses = Process.GetProcesses();
                foreach (var process in AllProcesses)
                {
                    if (process.MainWindowTitle != "")
                    {
                        string s = process.ProcessName.ToLower();
                        //if (s == "iexplore" || s == "iexplorer" || s == "chrome" || s == "firefox")
                        if (s == "firefox")
                            process.Kill();
                    }
                }

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
            HttpMethods.AddRange(new string[] { "GET", "POST" });
            HttpPostContentType.AddRange(new string[] { "Form", "Multipart/form-data", "Text" });
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.MaxServicePointIdleTime = 100;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            });
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
                return GetUniqueContainerName(input, script, ++times);
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
        public string GetPageContent(string Url)
        {
            HttpWebRequest WebRequestObject = null;
            string PageContent = string.Empty;
            try
            {
                WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);
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
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                WebRequestObject = null;
            }
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
        public string GetQuery(string reportName, XmlDocument doc)
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
                                                           insert into errorcount_{0} select containerid,containername, requestid, request,count(*) from error where error.scriptname='{1}' group by error.requestid order by requestid;
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
        public void Zip(string sourcePath, string destinationPath)
        {
            ZipFile zip = new ZipFile();
            zip.AddDirectory(sourcePath);
            zip.Save(destinationPath);
        }
        public void UnZip(string zipFilePath, string destinationPath)
        {
            using (ZipFile zip = new ZipFile(zipFilePath))
            {

                zip.ExtractAll(destinationPath);
            }
        }
        public string UniqueID
        {
            get
            {
                lock (_logObj)
                {
                    if (_uniqueID == 0)
                    {
                        Thread.Sleep(1000);
                        _uniqueID = ((int)(DateTime.UtcNow.Subtract(_dateTime)).TotalSeconds);
                    }
                    return (_uniqueID++).ToString();
                }
            }
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

        public string MachineUniqueID
        {
            get
            {
                if (_MachineUniqueID == string.Empty)
                {
                    try
                    {
                        _MachineUniqueID = String.Empty;
                        System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_Processor");
                        System.Management.ManagementObjectCollection moc = mc.GetInstances();

                        foreach (System.Management.ManagementObject mo in moc)
                        {
                            _MachineUniqueID = mo.Properties["ProcessorId"].Value.ToString();
                        }
                        mc = new System.Management.ManagementClass("Win32_BIOS");
                        moc = mc.GetInstances();
                        foreach (System.Management.ManagementObject mo in moc)
                        {
                            _MachineUniqueID = _MachineUniqueID + "_" + mo.Properties["SerialNumber"].Value.ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    return _MachineUniqueID;
                }
                else
                {
                    return _MachineUniqueID;
                }
            }
            private set { }
        }

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(timestamp);
        }

        //private MessageQueue _LTDataQueue = null;
        //public MessageQueue LTDataQueue
        //{
        //    get
        //    {

        //        if (_LTDataQueue == null && MessageQueue.Exists(@".\Private$\ltdata") == false)
        //        {
        //            _LTDataQueue = MessageQueue.Create(@".\Private$\ltdata");
        //        }
        //        else
        //        {
        //            _LTDataQueue = new MessageQueue(@".\Private$\ltdata");
        //        }

        //        return _LTDataQueue;
        //    }
        //    set { _LTDataQueue = value; }
        //}
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
            get;

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

    public class NBFSNet
    {
        private WcfBinaryCodec m_wcfBinaryCodec = new WcfBinaryCodec(Encoding.UTF8);

        public NBFSNet() { }

        public string DecodeBinaryXML(byte[] encodedXML)
        {
            if (encodedXML == null)
            {
                return "";
            }
            return m_wcfBinaryCodec.DecodeBinaryXML(encodedXML, false);
        }

        public byte[] EncodeBinaryXML(string xml)
        {
            if (String.IsNullOrEmpty(xml.Trim()))
            {
                return null;
            }
            return m_wcfBinaryCodec.EncodeBinaryXML(xml);
        }
    }

    public class WcfBinaryCodec
    {
        public WcfBinaryCodec()
        { }

        public WcfBinaryCodec(Encoding encoding)
        {
            m_encoding = encoding;
        }

        Encoding m_encoding = Encoding.UTF8;

        /// <summary>
        /// Decode a bytestream that was encoded by WCF's BinaryEncodingBindingElement.  Will throw if the bytestream does
        /// not decode properly or the result is not valid XML.  I/O streams are flushed but not closed.
        /// </summary>        
        /// <param name="explodeNewlines">if true, the returned string will be nicely indented according to 
        /// element depth, and each attribute will be placed on its own line</param>
        /// <returns></returns>
        public void DecodeBinaryXML(Stream binaryInput, Stream xmlOutput, bool? explodeNewlines)
        {
            // defaults
            var explode = explodeNewlines ?? false;

            // parse bytestream into the XML DOM
            var doc = new XmlDocument();
            using (var binaryReader = XmlDictionaryReader.CreateBinaryReader(binaryInput, WcfDictionaryBuilder.Dict, XmlDictionaryReaderQuotas.Max))
            {
                doc.Load(binaryReader);
            }

            // write document to the output stream with customized settings
            var settings = new XmlWriterSettings()
            {
                CheckCharacters = false,
                CloseOutput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                Encoding = m_encoding,
                Indent = explode,
                IndentChars = "\t",
                NewLineChars = Environment.NewLine,
                NewLineHandling = explode ? NewLineHandling.Replace : NewLineHandling.None,
                NewLineOnAttributes = explode
            };
            using (var writer = XmlWriter.Create(xmlOutput, settings))
            {
                doc.Save(writer);
                writer.Flush();
                xmlOutput.Flush();
            }
        }

        public string DecodeBinaryXML(byte[] binaryInput, bool? explodeNewLines)
        {
            var input = new MemoryStream(binaryInput);
            var output = new MemoryStream();
            DecodeBinaryXML(input, output, explodeNewLines);
            output.Seek(0, SeekOrigin.Begin);
            return new StreamReader(output, m_encoding).ReadToEnd();
        }

        /// <summary>
        /// Encode a text stream into a binary XML stream compatible with WCF's BinaryEncodingBindingElement.  Will throw if 
        /// the input stream cannot be parsed into an XML document.  I/O streams are flushed but not closed.
        /// </summary>
        /// <param name="xmlInput"></param>
        /// <param name="binaryOutput"></param>
        public void EncodeBinaryXML(Stream xmlInput, Stream binaryOutput)
        {
            // parse string into the XML DOM
            var doc = new XmlDocument();
            doc.Load(xmlInput);

            // write bytestream
            using (var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(binaryOutput, WcfDictionaryBuilder.Dict, null, false))
            {
                doc.Save(binaryWriter);
                binaryWriter.Flush();
                binaryOutput.Flush();
            }
        }

        public byte[] EncodeBinaryXML(string xmlInput)
        {
            var input = new MemoryStream(m_encoding.GetBytes(xmlInput));
            var output = new MemoryStream();
            EncodeBinaryXML(input, output);
            return output.ToArray();
        }
    }

    public static class WcfDictionaryBuilder
    {
        private static XmlDictionary dict;

        public static XmlDictionary Dict
        {
            get { return dict; }
        }

        static WcfDictionaryBuilder()
        {
            dict = new XmlDictionary();
            dict.Add("mustUnderstand");
            dict.Add("Envelope");
            dict.Add("http://www.w3.org/2003/05/soap-envelope");
            dict.Add("http://www.w3.org/2005/08/addressing");
            dict.Add("Header");
            dict.Add("Action");
            dict.Add("To");
            dict.Add("Body");
            dict.Add("Algorithm");
            dict.Add("RelatesTo");
            dict.Add("http://www.w3.org/2005/08/addressing/anonymous");
            dict.Add("URI");
            dict.Add("Reference");
            dict.Add("MessageID");
            dict.Add("Id");
            dict.Add("Identifier");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm");
            dict.Add("Transforms");
            dict.Add("Transform");
            dict.Add("DigestMethod");
            dict.Add("Address");
            dict.Add("ReplyTo");
            dict.Add("SequenceAcknowledgement");
            dict.Add("AcknowledgementRange");
            dict.Add("Upper");
            dict.Add("Lower");
            dict.Add("BufferRemaining");
            dict.Add("http://schemas.microsoft.com/ws/2006/05/rm");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/SequenceAcknowledgement");
            dict.Add("SecurityTokenReference");
            dict.Add("Sequence");
            dict.Add("MessageNumber");
            dict.Add("http://www.w3.org/2000/09/xmldsig#");
            dict.Add("http://www.w3.org/2000/09/xmldsig#enveloped-signature");
            dict.Add("KeyInfo");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            dict.Add("http://www.w3.org/2001/04/xmlenc#");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/sc");
            dict.Add("DerivedKeyToken");
            dict.Add("Nonce");
            dict.Add("Signature");
            dict.Add("SignedInfo");
            dict.Add("CanonicalizationMethod");
            dict.Add("SignatureMethod");
            dict.Add("SignatureValue");
            dict.Add("DataReference");
            dict.Add("EncryptedData");
            dict.Add("EncryptionMethod");
            dict.Add("CipherData");
            dict.Add("CipherValue");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            dict.Add("Security");
            dict.Add("Timestamp");
            dict.Add("Created");
            dict.Add("Expires");
            dict.Add("Length");
            dict.Add("ReferenceList");
            dict.Add("ValueType");
            dict.Add("Type");
            dict.Add("EncryptedHeader");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd");
            dict.Add("RequestSecurityTokenResponseCollection");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust#BinarySecret");
            dict.Add("http://schemas.microsoft.com/ws/2006/02/transactions");
            dict.Add("s");
            dict.Add("Fault");
            dict.Add("MustUnderstand");
            dict.Add("role");
            dict.Add("relay");
            dict.Add("Code");
            dict.Add("Reason");
            dict.Add("Text");
            dict.Add("Node");
            dict.Add("Role");
            dict.Add("Detail");
            dict.Add("Value");
            dict.Add("Subcode");
            dict.Add("NotUnderstood");
            dict.Add("qname");
            dict.Add("");
            dict.Add("From");
            dict.Add("FaultTo");
            dict.Add("EndpointReference");
            dict.Add("PortType");
            dict.Add("ServiceName");
            dict.Add("PortName");
            dict.Add("ReferenceProperties");
            dict.Add("RelationshipType");
            dict.Add("Reply");
            dict.Add("a");
            dict.Add("http://schemas.xmlsoap.org/ws/2006/02/addressingidentity");
            dict.Add("Identity");
            dict.Add("Spn");
            dict.Add("Upn");
            dict.Add("Rsa");
            dict.Add("Dns");
            dict.Add("X509v3Certificate");
            dict.Add("http://www.w3.org/2005/08/addressing/fault");
            dict.Add("ReferenceParameters");
            dict.Add("IsReferenceParameter");
            dict.Add("http://www.w3.org/2005/08/addressing/reply");
            dict.Add("http://www.w3.org/2005/08/addressing/none");
            dict.Add("Metadata");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/08/addressing");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/08/addressing/fault");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/06/addressingex");
            dict.Add("RedirectTo");
            dict.Add("Via");
            dict.Add("http://www.w3.org/2001/10/xml-exc-c14n#");
            dict.Add("PrefixList");
            dict.Add("InclusiveNamespaces");
            dict.Add("ec");
            dict.Add("SecurityContextToken");
            dict.Add("Generation");
            dict.Add("Label");
            dict.Add("Offset");
            dict.Add("Properties");
            dict.Add("Cookie");
            dict.Add("wsc");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/sc");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/sc/dk");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/sc/sct");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/RST/SCT");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/RSTR/SCT");
            dict.Add("RenewNeeded");
            dict.Add("BadContextToken");
            dict.Add("c");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/sc/dk");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/sc/sct");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/SCT");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT/Renew");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/SCT/Renew");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT/Cancel");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/SCT/Cancel");
            dict.Add("http://www.w3.org/2001/04/xmlenc#aes128-cbc");
            dict.Add("http://www.w3.org/2001/04/xmlenc#kw-aes128");
            dict.Add("http://www.w3.org/2001/04/xmlenc#aes192-cbc");
            dict.Add("http://www.w3.org/2001/04/xmlenc#kw-aes192");
            dict.Add("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
            dict.Add("http://www.w3.org/2001/04/xmlenc#kw-aes256");
            dict.Add("http://www.w3.org/2001/04/xmlenc#des-cbc");
            dict.Add("http://www.w3.org/2000/09/xmldsig#dsa-sha1");
            dict.Add("http://www.w3.org/2001/10/xml-exc-c14n#WithComments");
            dict.Add("http://www.w3.org/2000/09/xmldsig#hmac-sha1");
            dict.Add("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1");
            dict.Add("http://www.w3.org/2001/04/xmlenc#ripemd160");
            dict.Add("http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p");
            dict.Add("http://www.w3.org/2000/09/xmldsig#rsa-sha1");
            dict.Add("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            dict.Add("http://www.w3.org/2001/04/xmlenc#rsa-1_5");
            dict.Add("http://www.w3.org/2000/09/xmldsig#sha1");
            dict.Add("http://www.w3.org/2001/04/xmlenc#sha256");
            dict.Add("http://www.w3.org/2001/04/xmlenc#sha512");
            dict.Add("http://www.w3.org/2001/04/xmlenc#tripledes-cbc");
            dict.Add("http://www.w3.org/2001/04/xmlenc#kw-tripledes");
            dict.Add("http://schemas.xmlsoap.org/2005/02/trust/tlsnego#TLS_Wrap");
            dict.Add("http://schemas.xmlsoap.org/2005/02/trust/spnego#GSS_Wrap");
            dict.Add("http://schemas.microsoft.com/ws/2006/05/security");
            dict.Add("dnse");
            dict.Add("o");
            dict.Add("Password");
            dict.Add("PasswordText");
            dict.Add("Username");
            dict.Add("UsernameToken");
            dict.Add("BinarySecurityToken");
            dict.Add("EncodingType");
            dict.Add("KeyIdentifier");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#HexBinary");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Text");
            dict.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1#GSS_Kerberosv5_AP_REQ");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1#GSS_Kerberosv5_AP_REQ1510");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID");
            dict.Add("Assertion");
            dict.Add("urn:oasis:names:tc:SAML:1.0:assertion");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-rel-token-profile-1.0.pdf#license");
            dict.Add("FailedAuthentication");
            dict.Add("InvalidSecurityToken");
            dict.Add("InvalidSecurity");
            dict.Add("k");
            dict.Add("SignatureConfirmation");
            dict.Add("TokenType");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKey");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#EncryptedKeySHA1");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID");
            dict.Add("AUTH-HASH");
            dict.Add("RequestSecurityTokenResponse");
            dict.Add("KeySize");
            dict.Add("RequestedTokenReference");
            dict.Add("AppliesTo");
            dict.Add("Authenticator");
            dict.Add("CombinedHash");
            dict.Add("BinaryExchange");
            dict.Add("Lifetime");
            dict.Add("RequestedSecurityToken");
            dict.Add("Entropy");
            dict.Add("RequestedProofToken");
            dict.Add("ComputedKey");
            dict.Add("RequestSecurityToken");
            dict.Add("RequestType");
            dict.Add("Context");
            dict.Add("BinarySecret");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/spnego");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/tlsnego");
            dict.Add("wst");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/trust");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/RST/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/RSTR/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/CK/PSHA1");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/SymmetricKey");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/security/trust/Nonce");
            dict.Add("KeyType");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/trust/SymmetricKey");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/04/trust/PublicKey");
            dict.Add("Claims");
            dict.Add("InvalidRequest");
            dict.Add("RequestFailed");
            dict.Add("SignWith");
            dict.Add("EncryptWith");
            dict.Add("EncryptionAlgorithm");
            dict.Add("CanonicalizationAlgorithm");
            dict.Add("ComputedKeyAlgorithm");
            dict.Add("UseKey");
            dict.Add("http://schemas.microsoft.com/net/2004/07/secext/WS-SPNego");
            dict.Add("http://schemas.microsoft.com/net/2004/07/secext/TLSNego");
            dict.Add("t");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce");
            dict.Add("RenewTarget");
            dict.Add("CancelTarget");
            dict.Add("RequestedTokenCancelled");
            dict.Add("RequestedAttachedReference");
            dict.Add("RequestedUnattachedReference");
            dict.Add("IssuedTokens");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/Renew");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/Cancel");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey");
            dict.Add("Access");
            dict.Add("AccessDecision");
            dict.Add("Advice");
            dict.Add("AssertionID");
            dict.Add("AssertionIDReference");
            dict.Add("Attribute");
            dict.Add("AttributeName");
            dict.Add("AttributeNamespace");
            dict.Add("AttributeStatement");
            dict.Add("AttributeValue");
            dict.Add("Audience");
            dict.Add("AudienceRestrictionCondition");
            dict.Add("AuthenticationInstant");
            dict.Add("AuthenticationMethod");
            dict.Add("AuthenticationStatement");
            dict.Add("AuthorityBinding");
            dict.Add("AuthorityKind");
            dict.Add("AuthorizationDecisionStatement");
            dict.Add("Binding");
            dict.Add("Condition");
            dict.Add("Conditions");
            dict.Add("Decision");
            dict.Add("DoNotCacheCondition");
            dict.Add("Evidence");
            dict.Add("IssueInstant");
            dict.Add("Issuer");
            dict.Add("Location");
            dict.Add("MajorVersion");
            dict.Add("MinorVersion");
            dict.Add("NameIdentifier");
            dict.Add("Format");
            dict.Add("NameQualifier");
            dict.Add("Namespace");
            dict.Add("NotBefore");
            dict.Add("NotOnOrAfter");
            dict.Add("saml");
            dict.Add("Statement");
            dict.Add("Subject");
            dict.Add("SubjectConfirmation");
            dict.Add("SubjectConfirmationData");
            dict.Add("ConfirmationMethod");
            dict.Add("urn:oasis:names:tc:SAML:1.0:cm:holder-of-key");
            dict.Add("urn:oasis:names:tc:SAML:1.0:cm:sender-vouches");
            dict.Add("SubjectLocality");
            dict.Add("DNSAddress");
            dict.Add("IPAddress");
            dict.Add("SubjectStatement");
            dict.Add("urn:oasis:names:tc:SAML:1.0:am:unspecified");
            dict.Add("xmlns");
            dict.Add("Resource");
            dict.Add("UserName");
            dict.Add("urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName");
            dict.Add("EmailName");
            dict.Add("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress");
            dict.Add("u");
            dict.Add("ChannelInstance");
            dict.Add("http://schemas.microsoft.com/ws/2005/02/duplex");
            dict.Add("Encoding");
            dict.Add("MimeType");
            dict.Add("CarriedKeyName");
            dict.Add("Recipient");
            dict.Add("EncryptedKey");
            dict.Add("KeyReference");
            dict.Add("e");
            dict.Add("http://www.w3.org/2001/04/xmlenc#Element");
            dict.Add("http://www.w3.org/2001/04/xmlenc#Content");
            dict.Add("KeyName");
            dict.Add("MgmtData");
            dict.Add("KeyValue");
            dict.Add("RSAKeyValue");
            dict.Add("Modulus");
            dict.Add("Exponent");
            dict.Add("X509Data");
            dict.Add("X509IssuerSerial");
            dict.Add("X509IssuerName");
            dict.Add("X509SerialNumber");
            dict.Add("X509Certificate");
            dict.Add("AckRequested");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/AckRequested");
            dict.Add("AcksTo");
            dict.Add("Accept");
            dict.Add("CreateSequence");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/CreateSequence");
            dict.Add("CreateSequenceRefused");
            dict.Add("CreateSequenceResponse");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/CreateSequenceResponse");
            dict.Add("FaultCode");
            dict.Add("InvalidAcknowledgement");
            dict.Add("LastMessage");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/LastMessage");
            dict.Add("LastMessageNumberExceeded");
            dict.Add("MessageNumberRollover");
            dict.Add("Nack");
            dict.Add("netrm");
            dict.Add("Offer");
            dict.Add("r");
            dict.Add("SequenceFault");
            dict.Add("SequenceTerminated");
            dict.Add("TerminateSequence");
            dict.Add("http://schemas.xmlsoap.org/ws/2005/02/rm/TerminateSequence");
            dict.Add("UnknownSequence");
            dict.Add("http://schemas.microsoft.com/ws/2006/02/tx/oletx");
            dict.Add("oletx");
            dict.Add("OleTxTransaction");
            dict.Add("PropagationToken");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor");
            dict.Add("wscoor");
            dict.Add("CreateCoordinationContext");
            dict.Add("CreateCoordinationContextResponse");
            dict.Add("CoordinationContext");
            dict.Add("CurrentContext");
            dict.Add("CoordinationType");
            dict.Add("RegistrationService");
            dict.Add("Register");
            dict.Add("RegisterResponse");
            dict.Add("ProtocolIdentifier");
            dict.Add("CoordinatorProtocolService");
            dict.Add("ParticipantProtocolService");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor/CreateCoordinationContext");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor/CreateCoordinationContextResponse");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor/Register");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor/RegisterResponse");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wscoor/fault");
            dict.Add("ActivationCoordinatorPortType");
            dict.Add("RegistrationCoordinatorPortType");
            dict.Add("InvalidState");
            dict.Add("InvalidProtocol");
            dict.Add("InvalidParameters");
            dict.Add("NoActivity");
            dict.Add("ContextRefused");
            dict.Add("AlreadyRegistered");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat");
            dict.Add("wsat");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Completion");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Durable2PC");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Volatile2PC");
            dict.Add("Prepare");
            dict.Add("Prepared");
            dict.Add("ReadOnly");
            dict.Add("Commit");
            dict.Add("Rollback");
            dict.Add("Committed");
            dict.Add("Aborted");
            dict.Add("Replay");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Commit");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Rollback");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Committed");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Aborted");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Prepare");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Prepared");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/ReadOnly");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/Replay");
            dict.Add("http://schemas.xmlsoap.org/ws/2004/10/wsat/fault");
            dict.Add("CompletionCoordinatorPortType");
            dict.Add("CompletionParticipantPortType");
            dict.Add("CoordinatorPortType");
            dict.Add("ParticipantPortType");
            dict.Add("InconsistentInternalState");
            dict.Add("mstx");
            dict.Add("Enlistment");
            dict.Add("protocol");
            dict.Add("LocalTransactionId");
            dict.Add("IsolationLevel");
            dict.Add("IsolationFlags");
            dict.Add("Description");
            dict.Add("Loopback");
            dict.Add("RegisterInfo");
            dict.Add("ContextId");
            dict.Add("TokenId");
            dict.Add("AccessDenied");
            dict.Add("InvalidPolicy");
            dict.Add("CoordinatorRegistrationFailed");
            dict.Add("TooManyEnlistments");
            dict.Add("Disabled");
            dict.Add("ActivityId");
            dict.Add("http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics");
            dict.Add("http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1#Kerberosv5APREQSHA1");
            dict.Add("http://schemas.xmlsoap.org/ws/2002/12/policy");
            dict.Add("FloodMessage");
            dict.Add("LinkUtility");
            dict.Add("Hops");
            dict.Add("http://schemas.microsoft.com/net/2006/05/peer/HopCount");
            dict.Add("PeerVia");
            dict.Add("http://schemas.microsoft.com/net/2006/05/peer");
            dict.Add("PeerFlooder");
            dict.Add("PeerTo");
            dict.Add("http://schemas.microsoft.com/ws/2005/05/routing");
            dict.Add("PacketRoutable");
            dict.Add("http://schemas.microsoft.com/ws/2005/05/addressing/none");
            dict.Add("http://schemas.microsoft.com/ws/2005/05/envelope/none");
            dict.Add("http://www.w3.org/2001/XMLSchema-instance");
            dict.Add("http://www.w3.org/2001/XMLSchema");
            dict.Add("nil");
            dict.Add("type");
            dict.Add("char");
            dict.Add("boolean");
            dict.Add("byte");
            dict.Add("unsignedByte");
            dict.Add("short");
            dict.Add("unsignedShort");
            dict.Add("int");
            dict.Add("unsignedInt");
            dict.Add("long");
            dict.Add("unsignedLong");
            dict.Add("float");
            dict.Add("double");
            dict.Add("decimal");
            dict.Add("dateTime");
            dict.Add("string");
            dict.Add("base64Binary");
            dict.Add("anyType");
            dict.Add("duration");
            dict.Add("guid");
            dict.Add("anyURI");
            dict.Add("QName");
            dict.Add("time");
            dict.Add("date");
            dict.Add("hexBinary");
            dict.Add("gYearMonth");
            dict.Add("gYear");
            dict.Add("gMonthDay");
            dict.Add("gDay");
            dict.Add("gMonth");
            dict.Add("integer");
            dict.Add("positiveInteger");
            dict.Add("negativeInteger");
            dict.Add("nonPositiveInteger");
            dict.Add("nonNegativeInteger");
            dict.Add("normalizedString");
            dict.Add("ConnectionLimitReached");
            dict.Add("http://schemas.xmlsoap.org/soap/envelope/");
            dict.Add("Actor");
            dict.Add("Faultcode");
            dict.Add("Faultstring");
            dict.Add("Faultactor");
            dict.Add("Detail");
        }
    }

}
