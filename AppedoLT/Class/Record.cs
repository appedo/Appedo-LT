using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    class Record
    {
        private X509Certificate2 _certificate;
        private BackgroundWorker _worker;
        private int _requestCount = 0;
        private TcpListener _listener = null;
       // private RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        private RadTextBox _txtContainer;
        private RadComboBox _ddlParentContainer;
        private XmlNode _uvScript;
        private XmlNode _lastInsertedContainer;
        private XmlNode _lastInsertedPage;
        private XmlNode _selectedFirstLevelContainer;
        private Label _lblResult;
        private string _pageName = string.Empty;
        private string _host = string.Empty;
        private string _lastPageURL = string.Empty;
        private bool _runstart = false;
        private Stopwatch _pageDelay = new Stopwatch();
        delegate void SetTextCallback(string txt);
        List<Connection> connections = new List<Connection>();
        ConnectionManager connectionManager = new ConnectionManager(Constants.GetInstance().RecordConnection);
        Queue<IRequestProcessor> RecordData = new Queue<IRequestProcessor>();
        Thread storeData = null;
        bool isStop = false;
        int processingRequestCount = 0;
<<<<<<< HEAD

=======
        Thread StoreData = null;
        string _scriptResourcePath = string.Empty;
        string _scriptid = string.Empty;
<<<<<<< HEAD
>>>>>>> dev_master
=======
        Common _common = Common.GetInstance();
    
>>>>>>> dev_master
        public Record(Label lblResult, RadTextBox txtContainer, RadComboBox ddlParentContainer, XmlNode vuScriptXml)
        {
            try
            {
                _uvScript = vuScriptXml;
               
                _scriptid = vuScriptXml.Attributes["id"].Value;
                _scriptResourcePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + vuScriptXml.Attributes["id"].Value;
                if (Directory.Exists(_scriptResourcePath) == true) Directory.Delete(_scriptResourcePath, true);
                Directory.CreateDirectory(_scriptResourcePath);
                isStop = false;
                _txtContainer = txtContainer;
                _lblResult = lblResult;
                _listener = new TcpListener(Dns.Resolve(Constants.GetInstance().RecodingIPAddress).AddressList[0], int.Parse(Constants.GetInstance().RecodingPort));
                _worker = new BackgroundWorker();
                _worker.WorkerSupportsCancellation = true;
                _worker.DoWork += new DoWorkEventHandler(DoWork);
                _ddlParentContainer = ddlParentContainer;
                CreateFirstLevelContainers();
                ddlParentContainer.SelectedIndexChanged += new EventHandler(ddlParentContainer_SelectedIndexChanged);
                String certFilePath = String.Empty;
                if (ConfigurationManager.AppSettings["CertificateFile"] != null)
                    certFilePath =Constants.GetInstance().CertificatePath ;
                _certificate = new X509Certificate2(certFilePath, "pass@12345");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Start()
        {
            try
            {
                _worker.RunWorkerAsync();
                storeData = new Thread(new ThreadStart(StoreResult));
                storeData.Start();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

<<<<<<< HEAD
=======
        public void AutoRecovery()
        {
            //int priviousCount = 0;
            //new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            priviousCount = RecordData.Count;
            //            Thread.Sleep(60000);
            //            {
            //                lock (StoreData)
            //                {
            //                    if (isStop = true) break;
            //                    if (RecordData.Count != 0 && priviousCount == RecordData.Count)
            //                    {
            //                        try
            //                        {
            //                            StoreData.Abort();
            //                        }
            //                        catch
            //                        {

            //                        }
            //                        StoreData = new Thread(new ThreadStart(StoreResult));
            //                        StoreData.Start();
            //                    }
            //                    priviousCount = RecordData.Count;
            //                }
            //            }

            //        }

            //    }).Start();

        }

>>>>>>> dev_master
        public void Stop()
        {
            try
            {
                isStop = true;
                connectionManager.CloseAllConnetions();
<<<<<<< HEAD
=======
                int priviousCount = 0;
                int count = 0;

                while (true)
                {
                    count++;
                    if (RecordData.Count > 0) Thread.Sleep(1000);
                    else break;

                    if (count > 20)
                    {
                        break;
                    }
                }

>>>>>>> dev_master
                while (true)
                {
                    if (RecordData.Count > 0)
                    {
                        Thread.Sleep(1000);
<<<<<<< HEAD
=======
                        count++;
                        if (count > 30 && RecordData.Count != 0 && priviousCount == RecordData.Count)
                        {
                            lock (StoreData)
                            {
                                try
                                {
                                    StoreData.Abort();
                                    StoreData = new Thread(new ThreadStart(StoreResult));
                                    StoreData.Start();
                                    count = 0;
                                }
                                catch
                                {

                                }
                                priviousCount = RecordData.Count;
                            }
                        }
                        if (count > 30) priviousCount = RecordData.Count;

>>>>>>> dev_master
                    }
                    else
                    {
                        break;
                    }
                }

                _pageDelay.Stop();
                //_repositoryXml.Save();
                _listener.Stop();
                AppedoLT.Core.Constants.GetInstance().ReSetFirefoxProxy();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            _listener.Start();
            try
            {
                while (true)
                {
                    
                    if (processingRequestCount >= Constants.GetInstance().RecordConnection)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {

                        TcpClient client = _listener.AcceptTcpClient();
                        Thread th = new Thread(new ParameterizedThreadStart(ProceessClient));
                        th.Start(client);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            _listener.Stop();
        }

        void ProceessClient(object clientObj)
        {
            try
            {
                processingRequestCount++;
                TcpClient client = (TcpClient)clientObj;
                RequestProcessor pro = new RequestProcessor(client, this.connectionManager,  _txtContainer.Text, _lblResult);
                pro.Process();
                if (((IRequestProcessor)pro) != null)
                {
<<<<<<< HEAD
                    lock (RecordData)
=======
                    IRequestProcessor response = (IRequestProcessor)pro;
                    if (response.url != null && response.url.AbsoluteUri != string.Empty)
>>>>>>> dev_master
                    {
                        RecordData.Enqueue((IRequestProcessor)pro);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                processingRequestCount--;
            }
        }

        private void StoreResult()
        {
            while (true)
            {
                try
                {
                    if (RecordData.Count == 0 && isStop == true) break;
                    if (RecordData.Count == 0) Thread.Sleep(2000);
                    else
                    {
                        IRequestProcessor data;
                        lock (RecordData)
                        {
                            data = RecordData.Dequeue();
                        }
                        Thread.Sleep(100);
                        string requestContentType = string.Empty;
                        string responseContentType = string.Empty;
                        data.Requestid = Constants.GetInstance().UniqueID;
                        string reqFilename = data.Requestid + "_req.bin";
                        string resFilename = data.Requestid + "_res";
                        string contentEncoding = string.Empty;
<<<<<<< HEAD

=======
>>>>>>> dev_master
                        Regex expressForhead = new Regex("([A-Z]*) (.*) ([A-Z]*)/(.*)");
                        Regex expressForHeaders = new Regex("(.*?): (.*?)\r\n");
                        if (data.RequestHeader != null)
                        {
                            MatchCollection matchs = expressForhead.Matches(data.RequestHeader);
                            if (matchs.Count > 0)
                            {
                                XmlNode request = _uvScript.OwnerDocument.CreateElement("request");

                                #region Request attributes
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "id", data.Requestid));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Method", matchs[0].Groups[1].Value));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Path", data.url.AbsolutePath));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Protocal", matchs[0].Groups[3].Value));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Version", matchs[0].Groups[4].Value));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", data.url.LocalPath));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Schema", data.url.Scheme));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Host", data.url.Host));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Port", data.url.Port.ToString()));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "QueryString", data.url.Query));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Address", data.url.AbsoluteUri.Split('?')[0]));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Url", data.url.AbsoluteUri));
                                if (data.ResponseCode >= 400 || data.ResponseCode <= 99)
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "HasErrorResponse", true.ToString()));
                                }
                                else
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "HasErrorResponse", false.ToString()));
                                }
                                #endregion

                                matchs = expressForHeaders.Matches(data.RequestHeader);
                                bool isWebServiceRequest = false;

                                #region Headers
                                if (matchs.Count > 0)
                                {
                                    XmlNode headers = _uvScript.OwnerDocument.CreateElement("headers");
                                    foreach (Match match in matchs)
                                    {
                                        XmlNode header = _uvScript.OwnerDocument.CreateElement("header");
                                        header.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", match.Groups[1].Value));
                                        header.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", match.Groups[2].Value));
                                        if (match.Groups[1].Value == "Content-Type")
                                        {
                                            requestContentType = match.Groups[2].Value;
                                        }
                                        if (match.Groups[1].Value == "X-Requested-With")
                                        {
                                            isWebServiceRequest = true;
                                        }
                                        headers.AppendChild(header);
                                    }
                                    request.AppendChild(headers);
                                }
                                #endregion

                                #region Response Header

                                matchs = expressForHeaders.Matches(data.ResponseHeader);
                                if (matchs.Count > 0)
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "ResponseHeader", data.ResponseHeader));
                                }
                                matchs = new Regex("Content-Type: (.*?)\r\n").Matches(data.ResponseHeader);
                                if (matchs.Count > 0)
                                {
                                    responseContentType = matchs[0].Groups[1].Value;
                                    matchs = new Regex("Content-Type: [a-z]*/[a-z]*-([a-z]*).*?").Matches(data.ResponseHeader);
                                    if (matchs.Count <= 0) matchs = new Regex("Content-Type: [a-z]*/([a-z]*).*?").Matches(data.ResponseHeader);
                                    if (matchs.Count > 0) resFilename = resFilename + "." + matchs[0].Groups[1].Value;
                                }

                                matchs = new Regex("Content-Encoding: (.*?)\r\n").Matches(data.ResponseHeader);
                                if (matchs.Count > 0)
                                {
                                    contentEncoding = matchs[0].Groups[1].Value;
                                }
                                #endregion

                                #region Query string data
                                if (data.url.Query != null && data.url.Query != "")
                                {
                                    #region QueryStringParameters
<<<<<<< HEAD
                                    XmlNode parameters = _repositoryXml.doc.CreateElement("querystringparams");
                                    NameValueCollection nameAndValue = HttpUtility.ParseQueryString(data.url.Query,Encoding.ASCII);
=======
                                    XmlNode parameters = _uvScript.OwnerDocument.CreateElement("querystringparams");
                                    NameValueCollection nameAndValue = HttpUtility.ParseQueryString(data.url.Query, Encoding.ASCII);
>>>>>>> dev_master
                                    foreach (string key in nameAndValue.AllKeys)
                                    {
                                        XmlNode parameter = _uvScript.OwnerDocument.CreateElement("querystringparam");
                                        parameter.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", key));
                                        parameter.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", nameAndValue[key]));
                                        parameter.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", HttpUtility.UrlEncode(key)));
                                        parameter.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", HttpUtility.UrlEncode(nameAndValue[key])));
                                        parameters.AppendChild(parameter);
                                    }
                                    request.AppendChild(parameters);
                                    #endregion
                                }
                                #endregion

                                #region PostData
                                if (data.RequestBody.Length > 0 || request.Attributes["Method"].Value == "POST")
                                {
                                    string postData = data.RequestBody.Length > 0 ? Encoding.Default.GetString(data.RequestBody.ToArray()) : string.Empty;
                                    if (!(postData == null || postData == string.Empty))
                                    {
                                        XmlNode parameters = _uvScript.OwnerDocument.CreateElement("params");

                                        if (requestContentType.Contains("multipart/form-data"))
                                        {
                                            parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "type", "multipart/form-data"));

                                            #region Multipart
                                            try
                                            {
                                                string boundary = string.Empty;
                                                foreach (string temp in requestContentType.Split(';'))
                                                {
                                                    if (temp.ToLower().Contains("boundary") && temp.Contains("="))
                                                    {
                                                        boundary = temp.Split('=')[1];
                                                        break;
                                                    }
                                                }
                                                parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "boundary", boundary));

                                                foreach (string data1 in postData.Replace("--" + boundary + "--\r\n", string.Empty).Split(new string[] { "--" + boundary + "\r\n" }, StringSplitOptions.None))
                                                {

                                                    try
                                                    {
                                                        XmlNode param = _uvScript.OwnerDocument.CreateElement("param");
                                                        string temp = data1.Substring(0, data1.IndexOf("\r\n\r\n") + 1);

                                                        if (data1.Length > 0)
                                                        {

                                                            temp = temp.TrimStart('\r', '\n');
                                                            string[] dis_type = temp.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                                                            foreach (string nameAndValue in dis_type[0].Split(';'))
                                                            {
                                                                if (nameAndValue.Contains("Content-Disposition: "))
                                                                {
                                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "contentdisposition", nameAndValue.Replace("Content-Disposition: ", string.Empty).Trim()));
                                                                }
                                                                else if (nameAndValue.Contains(" name="))
                                                                {
                                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", nameAndValue.Replace(" name=", string.Empty).Trim()));
                                                                }
                                                                else if (nameAndValue.Contains(" filename="))
                                                                {
                                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "filename", nameAndValue.Replace(" filename=", string.Empty).Trim()));
                                                                }
                                                            }
                                                            if (dis_type.Length > 1)
                                                            {
                                                                foreach (string nameAndValue in dis_type[1].Split(';'))
                                                                {
                                                                    if (nameAndValue.Contains("Content-Type: "))
                                                                    {
                                                                        param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "contenttype", nameAndValue.Replace("Content-Type: ", string.Empty).Trim()));
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        if (data1.Length > 0)
                                                        {
                                                            temp = data1.Replace(temp, string.Empty).TrimStart();
                                                            if (param.Attributes["filename"] == null)
                                                            {
                                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", temp));
                                                                if (param.Attributes["value"].Value.EndsWith("\r\n") == true)
                                                                {
                                                                    param.Attributes["value"].Value = param.Attributes["value"].Value.Remove(param.Attributes["value"].Value.Length - 2, 2);
                                                                }
                                                            }
                                                            else
                                                            {

                                                                if (param.Attributes["filename"] != null && param.Attributes["filename"].Value.Replace("\"", string.Empty) != string.Empty)
                                                                {
                                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", "\\Upload\\" + param.Attributes["filename"].Value.Replace("\"", string.Empty)));
                                                                    if (Directory.Exists(Path.GetDirectoryName(Constants.GetInstance().ExecutingAssemblyLocation + param.Attributes["value"].Value)) == false)
                                                                    {
                                                                        Directory.CreateDirectory(Path.GetDirectoryName(Constants.GetInstance().ExecutingAssemblyLocation + param.Attributes["value"].Value));
                                                                    }
                                                                    using (FileStream streem = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + param.Attributes["value"].Value, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                                                    {
                                                                        streem.Write(System.Text.Encoding.Default.GetBytes(temp), 0, temp.Length - 2);
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", string.Empty));
                                                                }
                                                            }
                                                        }
                                                        if (data1.Length > 0) parameters.AppendChild(param);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                            }
                                            #endregion
                                        }
<<<<<<< HEAD
                                        else if (requestContentType.ToLower().StartsWith("text/") || requestContentType.ToLower().StartsWith("application/json") || requestContentType==string.Empty)
=======
                                        else if (isWebServiceRequest == true || requestContentType.ToLower().StartsWith("text/") || requestContentType.ToLower().StartsWith("application/json") || requestContentType == string.Empty)
>>>>>>> dev_master
                                        {
                                            parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "type", "text"));
                                            
                                            #region Text
                                            XmlNode param = _uvScript.OwnerDocument.CreateElement("Param");
                                            param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode("Text")));
                                            param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", "Text"));
                                            param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", System.Web.HttpUtility.UrlDecode(postData)));
                                            param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", postData));
                                            parameters.AppendChild(param);
                                            #endregion
                                        }
                                        else //if (requestContentType.Contains("application/x-www-form-urlencoded"))
                                        {
                                            parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "type", "form"));

                                            #region Postdata
                                            foreach (string post in postData.Split('&'))
                                            {
                                                XmlNode param = _uvScript.OwnerDocument.CreateElement("Param");
                                                string[] nameAndValue = post.Split('=');
                                                if (nameAndValue.Length == 1)
                                                {
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode(nameAndValue[0])));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", string.Empty));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", nameAndValue[0]));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", string.Empty));
                                                }
                                                else if (nameAndValue.Length == 2)
                                                {
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode(nameAndValue[0])));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", System.Web.HttpUtility.UrlDecode(nameAndValue[1])));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", nameAndValue[1]));
                                                    param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", string.Empty));
                                                }
                                                parameters.AppendChild(param);
                                            }
                                            #endregion
                                        }
                                        request.AppendChild(parameters);
                                    }
                                    else if (postData == string.Empty && requestContentType.ToLower().StartsWith("text/"))
                                    {
                                        XmlNode parameters = _uvScript.OwnerDocument.CreateElement("params");
                                        parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "type", "text"));

                                        #region Text
                                        XmlNode param = _uvScript.OwnerDocument.CreateElement("Param");
                                        param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode("Text")));
                                        param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", "Text"));
                                        param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", System.Web.HttpUtility.UrlDecode(postData)));
                                        param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", postData));
                                        parameters.AppendChild(param);
                                        request.AppendChild(parameters);
                                        #endregion

                                    }
                                }

                                #endregion

                                #region Write response to file

                                using (FileStream stream = new FileStream(_scriptResourcePath + "\\" + resFilename, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    if (data.ResponseBody.Length > 0)
                                    {
                                        byte[] buffer = new byte[1000000];
                                        int readConut = 0;
                                        data.ResponseBody.Seek(0, SeekOrigin.Begin);

                                        #region chunked
                                        if (data.ResponseHeader.Contains("Transfer-Encoding: chunked") == true)
                                        {
                                            MemoryStream tem = new MemoryStream();
                                            if (data.ResponseHeader.Contains("Transfer-Encoding: chunked") == true)
                                            {
                                                while (true)
                                                {
                                                    try
                                                    {
                                                        string length = ReceiveGZipHeader(data.ResponseBody);

                                                        int contentLength = length.Trim() == string.Empty ? 0 : int.Parse(length.Trim(), System.Globalization.NumberStyles.HexNumber);
                                                        if (contentLength == 0)
                                                        {
                                                            readConut = data.ResponseBody.Read(buffer, 0, 2);
                                                            break;
                                                        }
                                                        buffer = new byte[contentLength];
                                                        while (contentLength > 0)
                                                        {
                                                            readConut = data.ResponseBody.Read(buffer, 0, contentLength);
                                                            tem.Write(buffer, 0, readConut);
                                                            contentLength -= readConut;
                                                        }

                                                        readConut = data.ResponseBody.Read(buffer, 0, 2);
                                                    }
                                                    catch
                                                    {
                                                        break;
                                                    }
                                                }
                                                data.ResponseBody = tem;
                                                data.ResponseBody.Position = 0;
                                            }
                                        }
                                        #endregion

                                        if (contentEncoding == "gzip")
                                        {
                                            #region gzip
                                            using (GZipStream decompressedStream = new GZipStream(data.ResponseBody, CompressionMode.Decompress))
                                            {
                                                buffer = new byte[data.ResponseBody.Length];
                                                while (true)
                                                {
                                                    readConut = decompressedStream.Read(buffer, 0, buffer.Length);
                                                    if (readConut == 0) break;
                                                    stream.Write(buffer, 0, readConut);
                                                }
                                            }
                                            #endregion
                                        }

                                        else if (contentEncoding == "deflate")
                                        {
                                            #region deflate
                                            using (DeflateStream decompressedStream = new DeflateStream(data.ResponseBody, CompressionMode.Decompress))
                                            {
                                                buffer = new byte[data.ResponseBody.Length];
                                                while (true)
                                                {
                                                    readConut = decompressedStream.Read(buffer, 0, buffer.Length);
                                                    if (readConut == 0) break;
                                                    stream.Write(buffer, 0, readConut);
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            while ((readConut = data.ResponseBody.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                stream.Write(buffer, 0, readConut);
                                            }
                                        }
                                    }
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument,"resFilename", resFilename));

                                }
                                #endregion

                                #region NewContainer
                                if (_selectedFirstLevelContainer.ChildNodes.Count == 0 || _selectedFirstLevelContainer.LastChild.Attributes["name"].Value != data.ContainerName)
                                {
                                    XmlNode container = _common.CreateContainer(_uvScript.OwnerDocument, data.ContainerName);
                                    _selectedFirstLevelContainer.AppendChild(container);
                                    _lastInsertedContainer = container;
                                }
                                #endregion

                                #region NewPage
                                // if (_lastInsertedContainer.ChildNodes.Count == 0 || (data.RequestHeader.Contains("X-Requested-With") == false && responseContentType != string.Empty && responseContentType.StartsWith("text/html") == true && data.RequestBody != null && data.ResponseCode >= 200 && data.ResponseCode <= 399))// && isError == false))// && httpWebResponse.Headers["X-Requested-With"] != null))// && (!oS.oRequest.headers.Exists("X-Requested-With"))))
                                if (_lastInsertedContainer.ChildNodes.Count == 0 || (data.RequestHeader.Contains("X-Requested-With") == false && responseContentType.StartsWith("text/html") == true && data.ResponseCode >= 200 && data.ResponseCode <= 399 && data.ResponseBody != null))// && (!oS.oRequest.headers.Exists("X-Requested-With"))))
                                {
                                    string pageHtml = string.Empty;
                                    try
                                    {
                                        pageHtml = File.ReadAllText(_scriptResourcePath + "\\" + resFilename);
                                        data.ResponseBody.Position = 0;
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    }
                                    if (_lastInsertedContainer.ChildNodes.Count == 0 || pageHtml.Contains("<!DOCTYPE") || pageHtml.Contains("<!DOCTYPE".ToLower()))
                                    {
                                        try
                                        {
                                            XmlNode page = _uvScript.OwnerDocument.CreateElement("page");
                                            page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "pagename", data.url.LocalPath));
                                            page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "host", data.url.Host));

                                            if (_pageDelay.IsRunning == false)
                                            {
                                                page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "delay", "0"));
                                                _pageDelay.Start();
                                            }
                                            else
                                            {
                                                page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "delay", _pageDelay.ElapsedMilliseconds.ToString()));
                                                _pageDelay.Reset();
                                                _pageDelay.Start();
                                            }
                                            page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "id", Constants.GetInstance().UniqueID));
                                            page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", data.url.LocalPath));
                                            page.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "starttime", DateTime.Now.Ticks.ToString()));
                                            _lastInsertedContainer.AppendChild(page);
                                            _lastInsertedPage = page;
                                            _lastPageURL = data.url.AbsoluteUri;
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                        }
                                    }
                                }
                                #endregion

                                #region Write request to file
                                using (FileStream stream = new FileStream(_scriptResourcePath + "\\" + reqFilename, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    stream.Write(Encoding.Default.GetBytes(data.RequestHeader), 0, data.RequestHeader.Length);

                                    if (data.RequestBody.Length > 0)
                                    {
                                        data.RequestBody.Position = 0;
                                        byte[] buffer = new byte[8129];
                                        int readConut = 0;
                                        while ((readConut = data.RequestBody.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            stream.Write(buffer, 0, readConut);
                                        }
                                    }
                                }
                                #endregion

                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "reqFilename", reqFilename));
                                request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "IsEnable", true.ToString()));

                                if (_lastInsertedPage != null)
                                {
                                    _lastInsertedPage.AppendChild(request);
                                }
                                else
                                {
                                    ExceptionHandler.WritetoEventLog("_lastInsertedPage");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void CreateFirstLevelContainers()
        {
            try
            {
                XmlNode container = _common.CreateContainer(_uvScript.OwnerDocument,  "Initialize");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[0].Tag = container;

                container = _common.CreateContainer(_uvScript.OwnerDocument, "Actions");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[1].Tag = container;
                _selectedFirstLevelContainer = container;

                container = _common.CreateContainer(_uvScript.OwnerDocument, "End");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[2].Tag = container;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void ddlParentContainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _selectedFirstLevelContainer = (XmlNode)((RadComboBoxItem)(_ddlParentContainer.SelectedItem)).Tag;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private string ReceiveGZipHeader(Stream stream)
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
    }
}
