﻿using AppedoLT.Core;
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
    /// <summary>
    /// Record Http transaction from browser and stored in xml.
    /// 
    /// prerequisites: 
    ///  lblResult- To display url.
    ///  txtContainer- To change container name
    ///  ddlParentContainer- To change parent container(Initialize, Actions, End)
    ///  vuScriptXml- To store request and response in xml
    ///  
    /// Author: Rasith
    /// </summary>
    class Record
    {
        #region The private fields
        private string _pageName = string.Empty;
        private string _host = string.Empty;
        private string _lastPageURL = string.Empty;
        private string _scriptResourcePath = string.Empty;
        private string _scriptid = string.Empty;
        private bool _runstart = false;
        private bool _isStop = false;
        private int _processingRequestCount = 0;
        private int _requestCount = 0;
        private List<Connection> _connections = new List<Connection>();
        private Queue<IRequestProcessor> _recordData = new Queue<IRequestProcessor>();
        private ConnectionManager _connectionManager = new ConnectionManager(Constants.GetInstance().RecordConnection);
        private Common _common = Common.GetInstance();
        private Stopwatch _pageDelay = new Stopwatch();
        private Thread _storeData = null;
        private X509Certificate2 _certificate;
        private BackgroundWorker _worker;
        private TcpListener _listener = null;
        private RadTextBox _txtContainer;
        private RadComboBox _ddlParentContainer;
        private XmlNode _uvScript;
        private XmlNode _lastInsertedContainer;
        private XmlNode _lastInsertedPage;
        private XmlNode _selectedFirstLevelContainer;
        private Label _lblResult;
        private Regex _expressForhead = new Regex("([A-Z]*) (.*) ([A-Z]*)/(.*)");
        private Regex _expressForHeaders = new Regex("(.*?): (.*?)\r\n");
        #endregion

        #region The event
        delegate void SetTextCallback(string txt);
        #endregion

        #region The constructor

        /// <summary>
        /// To create Record object that used to record browser transactions.
        /// </summary>
        /// <param name="lblResult">To display url</param>
        /// <param name="txtContainer">To user container name</param>
        /// <param name="ddlParentContainer">To select parent container(Init, Action, End)</param>
        /// <param name="vuScriptXml"></param>
        public Record(Label lblResult, RadTextBox txtContainer, RadComboBox ddlParentContainer, XmlNode vuScriptXml)
        {
            try
            {
                _uvScript = vuScriptXml;
                _scriptid = vuScriptXml.Attributes["id"].Value;
                _scriptResourcePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + vuScriptXml.Attributes["id"].Value;
                if (Directory.Exists(_scriptResourcePath) == true)
                    Directory.Delete(_scriptResourcePath, true);
                Directory.CreateDirectory(_scriptResourcePath);
                _isStop = false;
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
                    certFilePath = Constants.GetInstance().CertificatePath;
                _certificate = new X509Certificate2(certFilePath, "pass@12345");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        #endregion

        #region The public methods

        /// <summary>
        /// To start thread for storing result.
        /// </summary>
        public void Start()
        {
            try
            {
                _worker.RunWorkerAsync();
                //Create thread to store result.
                _storeData = new Thread(new ThreadStart(StoreResult));
                _storeData.Start();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// To stop recording.
        /// </summary>
        public void Stop()
        {
            try
            {
                _isStop = true;
                _connectionManager.CloseAllConnetions();
                int priviousCount = 0;
                int count = 0;

                while (true)
                {
                    count++;
                    if (_recordData.Count > 0) Thread.Sleep(1000);
                    else break;

                    if (count > 20)
                    {
                        break;
                    }
                }

                while (true)
                {
                    if (_recordData.Count > 0)
                    {
                        Thread.Sleep(1000);
                        count++;
                        if (count > 30 && _recordData.Count != 0 && priviousCount == _recordData.Count)
                        {
                            lock (_storeData)
                            {
                                try
                                {
                                    _storeData.Abort();
                                    _storeData = new Thread(new ThreadStart(StoreResult));
                                    _storeData.Start();
                                    count = 0;
                                }
                                catch
                                {

                                }
                                priviousCount = _recordData.Count;
                            }
                        }
                        if (count > 30) priviousCount = _recordData.Count;
                    }
                    else
                    {
                        break;
                    }
                }
                _pageDelay.Stop();
                _listener.Stop();
                AppedoLT.Core.Constants.GetInstance().ReSetFirefoxProxy();
                _storeData.Abort();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        #endregion

        #region The private methods

        /// <summary>
        /// Listener thread to listen tcp connection from browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            _listener.Start();
            try
            {
                while (true)
                {
                    // If parallel connection limit crossed
                    if (_processingRequestCount >= Constants.GetInstance().RecordConnection)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        // ProceessClient(client);
                        // Create new thread to process request from browser.
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

        /// <summary>
        /// To process tcp client from browser.
        /// </summary>
        /// <param name="clientObj"></param>
        private void ProceessClient(object clientObj)
        {
            try
            {
                _processingRequestCount++;
                TcpClient client = (TcpClient)clientObj;
                RequestProcessor pro = new RequestProcessor(client, this._connectionManager, _txtContainer.Text, _lblResult);
                pro.Process();

                //If request processed with out any error.
                if (((IRequestProcessor)pro) != null)
                {
                    IRequestProcessor response = (IRequestProcessor)pro;
                    if (response.url != null && response.url.AbsoluteUri != string.Empty)
                    {
                        lock (response)
                        {
                            _recordData.Enqueue(response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                _processingRequestCount--;
            }
        }

        /// <summary>
        /// To store processed request info into xml.
        /// </summary>
        private void StoreResult()
        {
            List<string> liExcludeTypes = new List<string>();
            if (ConfigurationManager.AppSettings["ExcludeSecondaryRequest"].Trim() != null && ConfigurationManager.AppSettings["ExcludeSecondaryRequest"].Trim().Contains(","))
            {
                string[] arrExludeTypes = ConfigurationManager.AppSettings["ExcludeSecondaryRequest"].Trim().Split(',');//<string1/string2/string3/--->     
                 //make a new string list    
                liExcludeTypes.AddRange(arrExludeTypes);

            }

            while (true)
            {
                try
                {
                    //If user stopped recoding.
                    if (_recordData.Count == 0 && _isStop == true)
                    {
                        break;
                    }
                    //No record for storing.
                    if (_recordData.Count == 0) Thread.Sleep(2000);
                    else
                    {
                        IRequestProcessor data;
                        data = _recordData.Dequeue();
                        data.Requestid = Constants.GetInstance().UniqueID;
                        string requestContentType = string.Empty;
                        string responseContentType = string.Empty;
                        string reqFilename = data.Requestid + "_req.bin";
                        string resFilename = data.Requestid + "_res";
                        string contentEncoding = string.Empty;

                        if (data.RequestHeader != null)
                        {
                            MatchCollection matchs = _expressForhead.Matches(data.RequestHeader);
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
                               
                                // 
                                if (liExcludeTypes.Count>0)
                                {
                                    Boolean bCheck = false;
                                    foreach(string str in liExcludeTypes) {
                                        if(data.url.AbsolutePath.EndsWith(str)) {
                                            request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Excludesecondaryreq", "false"));
                                            bCheck = true;
                                            break;
                                        }
                                    }

                                    if (!bCheck) { request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Excludesecondaryreq", "true")); }
                                    

                                }
                                else if (liExcludeTypes.Count==0)
                                {
                                    if (data.url.AbsolutePath.EndsWith(ConfigurationManager.AppSettings["ExcludeSecondaryRequest"].Trim()))
                                    {
                                        request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Excludesecondaryreq", "false"));

                                    }
                                    else
                                    {
                                        request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Excludesecondaryreq", "true"));
                                    }
                                }
                                else
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "Excludesecondaryreq", "true"));
                                }
                                

                                

                                //if(data.url.AbsolutePath.Trim().EndsWith()
                                if (data.ResponseCode >= 400 || data.ResponseCode <= 99)
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "HasErrorResponse", true.ToString()));
                                }
                                else
                                {
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "HasErrorResponse", false.ToString()));
                                }
                                #endregion

                                matchs = _expressForHeaders.Matches(data.RequestHeader);
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

                                matchs = _expressForHeaders.Matches(data.ResponseHeader);
                                if (matchs.Count > 0)
                                {
                                   // request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "ResponseHeader", data.ResponseHeader));
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
                                    XmlNode parameters = _uvScript.OwnerDocument.CreateElement("querystringparams");
                                    NameValueCollection nameAndValue = HttpUtility.ParseQueryString(data.url.Query, Encoding.ASCII);
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

                                string postData = data.RequestBody.Length > 0 || request.Attributes["Method"].Value == "POST" ? Encoding.Default.GetString(data.RequestBody.ToArray()) : string.Empty;

                                if (data.RequestBody.Length > 0 || request.Attributes["Method"].Value == "POST")
                                {
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
                                        else if (isWebServiceRequest == true && requestContentType.Contains("application/x-www-form-urlencoded") && !data.RequestHeader.Contains("X-Requested-With: XMLHttpRequest"))
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
                                        else if (isWebServiceRequest == true || requestContentType.ToLower().StartsWith("text/") || requestContentType.ToLower().StartsWith("application/json") || requestContentType == string.Empty || requestContentType.ToLower().Contains("/soap"))
                                        {
                                            parameters.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "type", "text"));
                                            if (requestContentType.ToLower().Contains("/soap"))
                                            {
                                                WcfBinaryCodec obj = new WcfBinaryCodec();
                                                XmlNode param = _uvScript.OwnerDocument.CreateElement("Param");
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode("Text")));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", "Text"));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", obj.DecodeBinaryXML(data.RequestBody.ToArray(), true)));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", obj.DecodeBinaryXML(data.RequestBody.ToArray(), true)));
                                                parameters.AppendChild(param);
                                            }
                                            else
                                            {
                                                #region Text
                                                XmlNode param = _uvScript.OwnerDocument.CreateElement("Param");
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "name", System.Web.HttpUtility.UrlDecode("Text")));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawname", "Text"));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "value", System.Web.HttpUtility.UrlDecode(postData)));
                                                param.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "rawvalue", postData));
                                                parameters.AppendChild(param);
                                            }
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
                                    else if (postData == string.Empty)
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
                                    byte[] bytes=ASCIIEncoding.ASCII.GetBytes( data.ResponseHeader+"\r\n\r\n");
                                    stream.Write(bytes, 0, bytes.Length);

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
                                        else if (data.ResponseHeader.ToLower().Contains("content-type: application/soap"))
                                        {
                                            WcfBinaryCodec obj1 = new WcfBinaryCodec();
                                            string soapData = obj1.DecodeBinaryXML(data.ResponseBody.ToArray(), true);
                                            stream.Write(Encoding.Default.GetBytes(soapData), 0, soapData.Length);

                                        }
                                        else
                                        {
                                            while ((readConut = data.ResponseBody.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                stream.Write(buffer, 0, readConut);
                                            }
                                        }
                                    }
                                  
                                    request.Attributes.Append(_common.GetAttribute(_uvScript.OwnerDocument, "resFilename", resFilename));

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
                                        if (requestContentType.ToLower().Contains("/soap"))
                                        {
                                            WcfBinaryCodec obj1 = new WcfBinaryCodec();
                                            string soapData = obj1.DecodeBinaryXML(data.RequestBody.ToArray(), true);
                                            stream.Write(Encoding.Default.GetBytes(soapData), 0, soapData.Length);
                                        }
                                        else
                                        {
                                            byte[] buffer = new byte[8129];
                                            int readConut = 0;
                                            while ((readConut = data.RequestBody.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                stream.Write(buffer, 0, readConut);
                                            }
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
                    Thread.Sleep(1000);
                    ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Used to create First Level Containers when record starts.
        /// </summary>
        private void CreateFirstLevelContainers()
        {
            try
            {
                //Create Initialize node
                XmlNode container = _common.CreateContainer(_uvScript.OwnerDocument, "Initialize");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[0].Tag = container;

                //Create Actions node
                container = _common.CreateContainer(_uvScript.OwnerDocument, "Actions");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[1].Tag = container;
                _selectedFirstLevelContainer = container;

                //Create End node
                container = _common.CreateContainer(_uvScript.OwnerDocument, "End");
                _uvScript.AppendChild(container);
                _ddlParentContainer.Items[2].Tag = container;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// To read GZipHeader from stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string ReceiveGZipHeader(Stream stream)
        {
            StringBuilder header = new StringBuilder();
            byte[] bytes = new byte[10];

            //Read all bytes one by one until reach new line char.
            while (stream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.Default.GetString(bytes, 0, 1));
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n"))
                    break;
            }
            return header.ToString();
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

        #endregion
    }
}