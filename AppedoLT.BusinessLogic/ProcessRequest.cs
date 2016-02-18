using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Threading.Tasks;

namespace AppedoLT.BusinessLogic
{

    public class ProcessRequest
    {
        private string _reportName = string.Empty;
        private string _scriptName = string.Empty;
        private string _resposeUrl, _receivedCookies;
        private string _type = "1";
        private int _userid;
        private int _iterationid;
        private int _maxUser;
        private int _iteration;
        private int _index;
        private int _maxConnection = 1;
        private int _createdConnection = 1;
        private bool _browserCache = false;
        private bool _secondaryRequestPlayed = false;
        private XmlNode _vuScriptXml;
        private IPEndPoint _IPAddress = null;
        private Dictionary<string, object> _exVariablesValues = new Dictionary<string, object>();
        Dictionary<string, string> receivedCookies = new Dictionary<string, string>();
        public event LockError OnLockError;
        public VUserStatus VUserStatus;
        public event LockReportData OnLockReportData;
        public bool IsValidation = false;
        private List<string> cacheUrl = new List<string>();
        private Random _random = new Random();
        private Stack<string> _pageId = new Stack<string>();
        private Stack<string[]> _containerId = new Stack<string[]>();
        public event LockRequestResponse OnLockRequestResponse;
        Request req;
        public bool Break { get; set; }
        private ExecutionReport Status = ExecutionReport.GetInstance();
        public ProcessRequest(int maxUser, string reportName, string type, int userid, int iteration, XmlNode vuScript, bool browserCache, IPEndPoint ipaddress, Dictionary<string, object> exVariablesValue, Dictionary<string, string> cookies, LockError OnlockError, VUserStatus VuserStatus, LockReportData OnlockReportData, bool Isvalidation, Stack<string> pageId, Stack<string[]> containerId)
        {
            _maxUser = maxUser;
            _browserCache = browserCache;
            _type = type;
            _userid = userid;
            _iteration = iteration;
            _vuScriptXml = vuScript;
            _reportName = reportName;
            _IPAddress = ipaddress;
            _scriptName = _vuScriptXml.Attributes["name"].Value;
            _exVariablesValues = exVariablesValue;
            receivedCookies = cookies;
            OnLockError = OnlockError;
            VUserStatus = VuserStatus;
            OnLockReportData = OnlockReportData;
            IsValidation = Isvalidation;
            _pageId = pageId;
            _containerId = containerId;
        }


        public void ProcessParallelRequest(XmlNode request)
        {
            //XmlNode request = (XmlNode)request1;
            if (Break == true) { return; }

            RequestResponse responseResult = new RequestResponse();
            string response = string.Empty;
            req = null;
            try
            {
                #region FireRequest
                if (request.Attributes["requestsizeconstant"] != null || _vuScriptXml.Attributes["type"].Value == "tcp")
                {
                    #region TCP
                    if (request.Attributes["enable"] == null || Convert.ToBoolean(request.Attributes["enable"].Value) == true)
                    {
                        List<AppedoLT.Core.Tuple<string, string>> variables = new List<AppedoLT.Core.Tuple<string, string>>();
                        if (request.OuterXml.Contains("$$"))
                        {
                            variables = EvaluteExpTcp(request);
                        }
                        Connection con = new Connection(request.Attributes["serverip"].Value, int.Parse(request.Attributes["port"].Value));

                        req = new TcpRequest(request, con, false);
                        req.Variables = variables;
                        req.GetResponse();

                        if (OnLockRequestResponse != null)
                        {
                            #region Validation
                            responseResult.RequestResult = req;
                            responseResult.WebRequestResponseId = Convert.ToInt32(Constants.GetInstance().UniqueID);
                            LockRequestResponse(responseResult);
                            #endregion
                        }
                        else
                        {
                            LockResponseTime(req.RequestNode.Attributes["id"].Value, req.RequestNode.Attributes["Path"] == null ? req.RequestName : req.RequestNode.Attributes["Path"].Value, req.StartTime, req.EndTime, req.ResponseTime, req.ResponseSize, req.ResponseCode.ToString());
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Http
                    bool cacheEnabled = false;
                    if (request != null && Convert.ToBoolean(request.Attributes["IsEnable"].Value) == true)
                    {
                        try
                        {
                            List<AppedoLT.Core.Tuple<string, string>> variables = new List<AppedoLT.Core.Tuple<string, string>>();
                            if (request.OuterXml.Contains("$$"))
                            {
                                variables = EvaluteExp(request);
                            }
                            request.Attributes["Address"].Value = new StringBuilder().Append(request.Attributes["Schema"].Value).Append("://").Append(request.Attributes["Host"].Value).Append(":").Append(request.Attributes["Port"].Value).Append(request.Attributes["Path"].Value).ToString();
                            Uri temp = new Uri(request.Attributes["Address"].Value);
                            if (_browserCache == true && _index > 1)
                            {
                                try
                                {
                                    XmlNode requestHeadeNode = request.SelectSingleNode("./headers/header[@name='Accept']");
                                    Match mat = new Regex("Content-Type: (.*?)\r\n", RegexOptions.Singleline | RegexOptions.Multiline).Match(request.Attributes["ResponseHeader"].Value);

                                    if (requestHeadeNode != null && requestHeadeNode.Attributes["value"].Value.Contains("/"))
                                    {
                                        if (requestHeadeNode.Attributes["value"].Value.ToLower().Contains("application") == false)
                                        {
                                            string acceptType = requestHeadeNode.Attributes["value"].Value.Split('/')[1];
                                            acceptType = acceptType.ToLower();
                                            if ((acceptType.Contains("image")
                                                || acceptType.Contains("css")
                                                || acceptType.Contains("js")
                                                || acceptType.Contains("javascript")
                                                || temp.LocalPath.EndsWith(".js")
                                                || temp.LocalPath.EndsWith(".css")
                                                || temp.LocalPath.EndsWith(".png")
                                                || temp.LocalPath.EndsWith(".jpg")
                                                || temp.LocalPath.EndsWith(".pdf")
                                                || temp.LocalPath.EndsWith(".gif")
                                                || temp.LocalPath.EndsWith(".ico"))
                                                && acceptType.Contains("application") == false
                                               )
                                            {
                                                cacheEnabled = true;
                                            }
                                        }
                                    }
                                    if (mat.Success == true && mat.Groups[1] != null && mat.Groups[1].Value.Contains("/"))
                                    {
                                        if (mat.Groups[1].Value.ToLower().Contains("application") == false)
                                        {
                                            string acceptType = mat.Groups[1].Value.Split('/')[1];
                                            acceptType = acceptType.ToLower();
                                            if (acceptType.Contains("image")
                                                || acceptType.Contains("css")
                                                || acceptType.Contains("js")
                                                || acceptType.Contains("javascript"))
                                            {
                                                cacheEnabled = true;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                            }
                            string fileNameExt = Path.GetExtension(temp.LocalPath);
                            if (cacheEnabled == false && !(fileNameExt != string.Empty && _vuScriptXml.Attributes["exclutionfiletypes"].Value.Contains(fileNameExt.Replace(".", string.Empty).Trim().ToLower()) == true))
                            {

                                req = new HttpRequest(request, ref receivedCookies, _userid.ToString() + (_createdConnection++ % _maxConnection).ToString(), _IPAddress, IsValidation);
                                req.Variables = variables;
                                req.GetResponse();

                                if (OnLockRequestResponse != null)
                                {
                                    #region Validation
                                    StringBuilder pDataBuffer = new StringBuilder();
                                    foreach (PostData pData in GetPostData(request.SelectSingleNode("//params")))
                                    {
                                        pDataBuffer.Append(pData.value.ToString());
                                    }
                                    responseResult.PostData = pDataBuffer.ToString();
                                    responseResult.RequestResult = req;
                                    responseResult.WebRequestResponseId = Convert.ToInt32(Constants.GetInstance().UniqueID);
                                    LockRequestResponse(responseResult);

                                    #endregion
                                }
                                // else
                                //  {
                                LockResponseTime(req.RequestNode.Attributes["id"].Value, req.RequestNode.Attributes["Path"] == null ? req.RequestName : req.RequestNode.Attributes["Path"].Value, req.StartTime, req.EndTime, req.ResponseTime, req.ResponseSize, req.ResponseCode.ToString());
                                //  }

                                #region SecondaryReqEnable
                                if (Convert.ToBoolean(_vuScriptXml.Attributes["dynamicreqenable"].Value) == true && !(_browserCache == true && _index > 1) && Convert.ToBoolean(req.RequestNode.Attributes["Excludesecondaryreq"].Value) == true)
                                {

                                    Queue<String> links = FetchLinksFromSource(req.ResponseStr);
                                    int created = 0;
                                    while (links.Count > 0)
                                    {
                                        while (created > 0)
                                        {
                                            Thread.Sleep(1000);
                                        }
                                        try
                                        {
                                            Request secReq = new HttpRequest(request, links.Dequeue(), receivedCookies, _userid.ToString() + (_createdConnection++ % _maxConnection).ToString(), _IPAddress, IsValidation);
                                            //  Request secReq = new HttpRequest(request, links.Dequeue(), receivedCookies, _userid.ToString(), _IPAddress, IsValidation);

                                            RequestResponse responseResultSec = new RequestResponse();
                                            if (_browserCache == false || cacheUrl.Exists(t => t.CompareTo(secReq.RequestNode.Attributes["Address"].Value) == 0) == false)
                                            {
                                                cacheUrl.Add(secReq.RequestNode.Attributes["Address"].Value);
                                                if (secReq.RequestNode.Attributes["Address"].Value.EndsWith(".jsp") == false)
                                                {
                                                    //new Thread(() =>
                                                    {
                                                        //  created++;
                                                        try
                                                        {
                                                            secReq.GetResponse();

                                                            if (OnLockRequestResponse != null)
                                                            {
                                                                responseResultSec.RequestResult = secReq;
                                                                responseResultSec.WebRequestResponseId = Convert.ToInt32(Constants.GetInstance().UniqueID);
                                                                LockRequestResponse(responseResultSec);
                                                            }
                                                            else
                                                            {
                                                                LockResponseTime(req.RequestNode.Attributes["id"].Value, req.RequestNode.Attributes["Path"] == null ? req.RequestName : req.RequestNode.Attributes["Path"].Value, req.StartTime, req.EndTime, req.ResponseTime, req.ResponseSize, req.ResponseCode.ToString());
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                                        }
                                                        finally
                                                        {
                                                            //      created--;
                                                        }
                                                    }
                                                    // ).Start();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                        }
                                        finally
                                        {
                                            _secondaryRequestPlayed = true;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            #region ToolException
                            if (ex.Message.StartsWith("Thread was being aborted.") == false)
                            {
                                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                #endregion

                #region correlation

                foreach (XmlNode exVar in request.SelectNodes("extractor"))
                {
                    string variableName = exVar.Attributes["name"].Value;
                    string selectionType = exVar.Attributes["selctiontype"].Value;
                    int groupindex = Convert.ToInt32(exVar.Attributes["groupindex"].Value);

                    MatchCollection match = Regex.Matches(req.ResponseStr, exVar.Attributes["regex"].Value, RegexOptions.Singleline | RegexOptions.Multiline);
                    string[] strs = new string[_exVariablesValues.Keys.Count];
                    _exVariablesValues.Keys.CopyTo(strs, 0);

                    foreach (string key in strs)
                    {
                        if (key.StartsWith(variableName))
                        {
                            _exVariablesValues.Remove(key);
                        }
                    }
                    if (match.Count > 0)
                    {
                        //if it is random type or array type
                        if (selectionType == "all" || selectionType == "random")
                        {
                            int randValue = _random.Next(1, match.Count + 1);
                            _exVariablesValues.Add(variableName + "_arraysize", match.Count);
                            _exVariablesValues.Add(variableName + "_last", match[match.Count - 1].Groups[groupindex].Value.Trim());
                            req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName + "_arraysize", match.Count.ToString()));
                            req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName + "_last", match[match.Count - 1].Groups[groupindex].Value.Trim()));
                            if (selectionType == "random")
                            {
                                _exVariablesValues.Add(variableName + "_rand", match[randValue - 1].Groups[groupindex].Value.Trim());
                                req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName + "_rand", match[randValue - 1].Groups[groupindex].Value.Trim()));
                            }
                            for (int i = 0; i < match.Count; i++)
                            {
                                _exVariablesValues.Add(variableName + "_" + (i + 1).ToString(), match[i].Groups[groupindex].Value.Trim());
                                req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName + "_" + (i + 1).ToString(), match[i].Groups[groupindex].Value.Trim()));
                            }
                        }
                        else if (selectionType == "single")
                        {
                            int ordinal = 0;
                            int.TryParse(exVar.Attributes["ordinal"].Value, out ordinal);
                            if (ordinal == 0)
                            {
                                _exVariablesValues.Remove(variableName);
                                _exVariablesValues.Add(variableName, match[0].Groups[groupindex].Value.Trim());
                                req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName, match[0].Groups[groupindex].Value.Trim()));

                            }
                            else
                            {
                                for (int i = 0; i < match.Count; i++)
                                {
                                    if (i + 1 == ordinal)
                                    {
                                        _exVariablesValues.Remove(variableName);
                                        _exVariablesValues.Add(variableName, match[i].Groups[groupindex].Value.Trim());
                                        req.ExtractedVariables.Add(new AppedoLT.Core.Tuple<string, string>(variableName, match[i].Groups[groupindex].Value.Trim()));

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Extrator faild
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                req = null;
                responseResult = null;
                request = null;
            }


        }
        public List<AppedoLT.Core.Tuple<string, string>> EvaluteExp(XmlNode expression)
        {
            #region Parm has variable
            List<AppedoLT.Core.Tuple<string, string>> Varialbles = new List<AppedoLT.Core.Tuple<string, string>>();
            Regex regex = new Regex(@"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");

            Match match = null;


            foreach (XmlAttribute attribute in expression.Attributes)
            {
                match = regex.Match(attribute.Value);

                while (match.Success == true)
                {
                    AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                    parm.Key = match.Groups[1].Value;
                    try
                    {
                        parm.Value = GetVariableValue(parm.Key);
                    }
                    catch
                    {
                        parm.Value = string.Empty;
                        LockException("0", "Unable to evaluate " + parm.Key, "700", string.Empty);
                    }
                    finally
                    {
                        attribute.Value = attribute.Value.Remove(match.Index, match.Length);
                        attribute.Value = attribute.Value.Insert(match.Index, parm.Value);
                        Varialbles.Add(parm);
                    }
                    match = regex.Match(attribute.Value);
                }
            }

            StringBuilder exp = new StringBuilder();
            exp.Append(expression.InnerXml);
            match = Regex.Match(exp.ToString(), @"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");
            while (match.Success == true)
            {
                AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                parm.Key = match.Groups[1].Value;
                try
                {
                    parm.Value = GetVariableValue(parm.Key);
                }
                catch
                {
                    parm.Value = string.Empty;
                    LockException("0", "Unable to evaluate " + parm.Key, "700", string.Empty);
                }
                finally
                {
                    exp.Remove(match.Index, match.Length);
                    exp.Insert(match.Index, HttpUtility.HtmlEncode(parm.Value));
                    Varialbles.Add(parm);
                }
                match = Regex.Match(exp.ToString(), @"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");
            }

            expression.InnerXml = exp.ToString();
            return Varialbles;
            #endregion
        }
        private Queue<String> FetchLinksFromSource(string htmlSource)
        {

            HtmlTag tag;
            HtmlParser parse = new HtmlParser(htmlSource);
            Queue<String> links = new Queue<String>();
            if (htmlSource.Contains("html") == true)
            {

                string baseURL = string.Empty;
                string value;

                while (parse.ParseNext("base", out tag))
                {
                    if (tag.Attributes.TryGetValue("href", out value))
                    {
                        baseURL = value;
                    }
                }
                parse.Reset();

                while (parse.ParseNext("img", out tag))
                {
                    if (tag.Attributes.TryGetValue("src", out value))
                    {
                        links.Enqueue(baseURL + value);
                    }
                }
                parse.Reset();

                while (parse.ParseNext("link", out tag))
                {
                    if (tag.Attributes.TryGetValue("href", out value))
                    {
                        links.Enqueue(baseURL + value);
                    }
                }

                parse.Reset();
                while (parse.ParseNext("script", out tag))
                {

                    if (tag.Attributes.TryGetValue("href", out value))
                    {
                        links.Enqueue(baseURL + value);
                    }
                }
            }
            return links;
        }
        private List<PostData> GetPostData(XmlNode postData)
        {
            int count = 0;
            List<PostData> postDataBuffer = new List<PostData>();

            if (postData != null && postData.HasChildNodes && postData.Attributes["type"].Value == "postdata")
            {
                #region PostData
                PostData pData = new PostData();
                pData.type = 1;
                foreach (XmlNode parm in postData.ChildNodes)
                {
                    count++;
                    if (count == postData.ChildNodes.Count)
                    {
                        pData.value.Append(parm.Attributes["name"].Value).Append("=").Append(System.Web.HttpUtility.UrlEncode(parm.Attributes["value"].Value));
                    }
                    else
                    {
                        pData.value.Append(parm.Attributes["name"].Value).Append("=").Append(System.Web.HttpUtility.UrlEncode(parm.Attributes["value"].Value)).Append("&");
                    }
                }
                pData.size = pData.value.Length;
                postDataBuffer.Add(pData);
                #endregion
            }
            else if (postData != null && postData.HasChildNodes && postData.Attributes["type"].Value == "multipart/form-data")
            {
                #region Multipart/Form-Data
                PostData pData = new PostData();
                //pData.type = 1 for string, 2 for file
                pData.type = 1;
                foreach (XmlNode parm in postData.ChildNodes)
                {
                    count++;
                    if (count == 1)
                    {
                        pData.value.Append("--").Append(postData.Attributes["boundary"].Value);
                    }
                    else
                    {
                        pData.value.Append("\r\n--").Append(postData.Attributes["boundary"].Value);
                    }
                    pData.value.Append("\r\n").Append("Content-Disposition: ").Append(parm.Attributes["contentdisposition"].Value).Append(";").Append("").Append(" name=").Append(parm.Attributes["name"].Value);
                    if (parm.Attributes["filename"] == null)
                    {
                        pData.value.Append("\r\n\r\n").Append(parm.Attributes["value"].Value);
                    }
                    else
                    {
                        pData.value.Append(";").Append(" filename=").Append(parm.Attributes["filename"].Value).Append("\r\n").Append("Content-Type: ").Append(parm.Attributes["contenttype"].Value).Append("\r\n\r\n");
                        pData.size = pData.value.Length;
                        postDataBuffer.Add(pData);
                        try
                        {
                            if (File.Exists(Constants.GetInstance().ExecutingAssemblyLocation + parm.Attributes["value"].Value))
                            {
                                pData = new PostData();
                                pData.type = 2;
                                pData.value.Append(Constants.GetInstance().ExecutingAssemblyLocation + parm.Attributes["value"].Value);
                                pData.size = new FileInfo(Constants.GetInstance().ExecutingAssemblyLocation + parm.Attributes["value"].Value).Length;
                                postDataBuffer.Add(pData);
                                pData = new PostData();
                                pData.type = 1;
                            }

                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                    if (count == postData.ChildNodes.Count)
                    {
                        pData.value.Append("\r\n--").Append(postData.Attributes["boundary"].Value).Append("--").Append("\r\n");
                    }
                }
                pData.size = pData.value.Length;
                postDataBuffer.Add(pData);
                #endregion
            }
            return postDataBuffer;
        }


        private void LockRequestResponse(RequestResponse data)
        {
            data.ContainerName = _containerId.Peek()[1];
            if (OnLockRequestResponse != null) OnLockRequestResponse.Invoke(data);
        }
        private void LockResponseTime(string requestid, string address, DateTime starttime, DateTime endtime, double diff, long responsesize, string reponseCode)
        {
            try
            {
                if (Break == false)
                {
                    ReportData rd = new ReportData();
                    rd.loadgen = Constants.GetInstance().LoadGen;
                    rd.sourceip = _IPAddress.Address.ToString();
                    rd.loadgenanme = ExecutionReport.GetInstance().LoadGenName;
                    rd.scenarioname = Status.ScenarioName;
                    rd.scriptid = _vuScriptXml.Attributes["id"].Value;
                    rd.containerid = _containerId.Peek()[0];
                    rd.containername = _containerId.Peek()[1];
                    rd.pageid = _pageId.Count < 1 ? "1" : _pageId.Peek();
                    rd.userid = _userid;
                    rd.iterationid = _iterationid;
                    rd.requestid = requestid;
                    rd.address = address;
                    rd.starttime = starttime;
                    rd.endtime = endtime;
                    rd.diff = diff;
                    rd.responsesize = responsesize;
                    rd.reponseCode = reponseCode;

                    if (OnLockReportData != null && rd != null)
                    {
                        OnLockReportData.Invoke(rd);
                        if (req.HasError == true)
                        {
                            LockException(req.RequestId.ToString(), req.ErrorMessage, req.ErrorCode, req.RequestName);
                        }
                        if (req.ResponseCode >= 200 && req.ResponseCode <= 299)
                        {
                            VUserStatus.TwoHundredStatusCodeCount++;
                        }
                        else if (req.ResponseCode >= 300 && req.ResponseCode <= 399)
                        {
                            VUserStatus.ThreeHundredStatusCodeCount++;
                        }
                        else if (req.ResponseCode >= 400 && req.ResponseCode <= 499)
                        {
                            VUserStatus.FourHundredStatusCodeCount++;
                        }
                        else if (req.ResponseCode >= 500 && req.ResponseCode <= 599)
                        {
                            VUserStatus.FiveHundredStatusCodeCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public List<AppedoLT.Core.Tuple<string, string>> EvaluteExpTcp(XmlNode expression)
        {
            #region Parm has variable
            List<AppedoLT.Core.Tuple<string, string>> Varialbles = new List<AppedoLT.Core.Tuple<string, string>>();
            Regex regex = new Regex(@"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");

            Match match = null;

            foreach (XmlAttribute attribute in expression.Attributes)
            {
                match = regex.Match(attribute.Value);
                while (match.Success == true)
                {
                    AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                    parm.Key = match.Groups[1].Value;
                    try
                    {
                        parm.Value = GetVariableValue(parm.Key);
                    }
                    catch
                    {
                        parm.Value = string.Empty;
                    }
                    finally
                    {
                        attribute.Value = attribute.Value.Remove(match.Index, match.Length);
                        attribute.Value = attribute.Value.Insert(match.Index, parm.Value);
                        Varialbles.Add(parm);
                    }
                    match = regex.Match(attribute.Value);
                }
            }

            StringBuilder exp = new StringBuilder();
            exp.Append(expression.InnerXml);
            match = Regex.Match(exp.ToString(), @"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");
            while (match.Success == true)
            {
                AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                parm.Key = match.Groups[1].Value;
                try
                {
                    parm.Value = GetVariableValue(parm.Key);
                }
                catch
                {
                    parm.Value = string.Empty;
                    LockException("0", "Unable to evaluate " + parm.Key, "700", string.Empty);
                }
                finally
                {
                    exp.Remove(match.Index, match.Length);
                    exp.Insert(match.Index, parm.Value);
                    Varialbles.Add(parm);
                }
                match = Regex.Match(exp.ToString(), @"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");
            }

            expression.InnerXml = exp.ToString();
            return Varialbles;
            #endregion
        }
        private EvalutionResult EvaluteExp(string expression)
        {
            #region Parm has variable
            EvalutionResult evalutionResult = new EvalutionResult();
            if (expression.Contains("$$") == true)
            {
                string type = "";
                MatchCollection match = Regex.Matches(expression, @"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");
                for (int ind = 0; ind < match.Count; ind++)
                {
                    Parameter parm = new Parameter();

                    parm.Name = match[ind].Groups[1].Value;
                    if (_exVariablesValues.ContainsKey(parm.Name) == true)
                    {
                        if (parm.Name.EndsWith("_rand"))
                        {
                            Random ran = new Random();
                            string resultVariable = string.Empty;
                            parm.Value = parm.Name.Replace("rand", Convert.ToInt64(_exVariablesValues[parm.Name]).ToString());
                            expression = expression.Replace("$$" + parm.Name + "$$", _exVariablesValues[resultVariable].ToString());
                        }
                        else
                        {
                            expression = expression.Replace("$$" + parm.Name + "$$", _exVariablesValues[parm.Name].ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            type = VariableManager.dataCenter.GetVariableType(parm.Name.Split('.')[0]);
                            if (type == "file" || type == "string" || type == "number" || type == "randomnumber" || type == "randomstring" || type == "currentdate")
                            {
                                parm.Value = VariableManager.dataCenter.GetVariableValue(_userid, _iterationid, parm.Name, _maxUser).ToString();
                                expression = expression.Replace("$$" + parm.Name + "$$", parm.Value);
                            }
                        }
                        catch
                        {
                            evalutionResult.isSuccess = false;
                            evalutionResult.value = string.Format("-Unable to evalute {0} value. Userid:{1}, Iterationid:{2}, ScriptName:{3}", parm.Name.Split('.')[0], _userid, _iterationid, _vuScriptXml.Attributes["name"].Value);
                            return evalutionResult;
                        }
                    }
                }
            }

            evalutionResult.isSuccess = true;
            evalutionResult.value = expression;

            return evalutionResult;

            #endregion
        }

        private string GetVariableValue(string variablename)
        {
            //lock (_exVariablesValues)
            {
                string result = string.Empty;
                if (_exVariablesValues.ContainsKey(variablename) == true)
                {
                    result = _exVariablesValues[variablename].ToString();
                }
                else
                {
                    string type = VariableManager.dataCenter.GetVariableType(variablename.Split('.')[0]);
                    if (type == "file" || type == "string" || type == "number" || type == "randomnumber" || type == "randomstring" || type == "currentdate")
                    {
                        result = VariableManager.dataCenter.GetVariableValue(_userid, _iterationid, variablename, _maxUser).ToString();
                    }
                }
                return System.Web.HttpUtility.HtmlEncode(result);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="message"></param>
        /// <param name="errorCode">
        /// 600- Unable to evalutate.
        /// 700- System exception.
        /// 800- Assertion Faild.
        /// </param>
        private void LockException(string requestid, string message, string errorCode, string url)
        {
            RequestException exception = new RequestException();
            exception.reportname = _reportName;
            exception.scenarioname = Status.ScenarioName;
            exception.scriptid = _vuScriptXml.Attributes["id"].Value;
            exception.scriptname = _vuScriptXml.Attributes["name"].Value;
            exception.requestid = requestid;
            exception.iterationid = this._iterationid.ToString();
            exception.userid = this._userid.ToString();
            exception.requestexceptionid = Constants.GetInstance().UniqueID;
            exception.time = DateTime.Now;
            exception.from = "web";
            exception.message = message;
            exception.errorcode = errorCode;
            exception.request = url;
            exception.containerid = _containerId.Peek()[0];
            exception.containername = _containerId.Peek()[1];
            if (OnLockError != null && exception != null)
            {
                OnLockError.Invoke(exception);
                VUserStatus.ErrorCount++;
            }
        }

        private void LockException(RequestException exception)
        {
            exception.containerid = _containerId.Peek()[0];
            exception.containername = _containerId.Peek()[1];
            exception.scriptid = _vuScriptXml.Attributes["id"].Value;
            if (OnLockError != null && exception != null)
            {
                OnLockError.Invoke(exception);
                VUserStatus.ErrorCount++;
            }
        }

    }






}