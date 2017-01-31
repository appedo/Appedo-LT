using AppedoLT.Core;
using System;
using System.Linq;
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
using System.Configuration;
using System.Threading.Tasks;
using System.Collections;

namespace AppedoLT.BusinessLogic
{

    /// <summary>
    /// This is used to create single vuser. Given script will be executed and result will be send to client.
    /// 
    /// prerequisites: 
    ///   maxUser- Max user count
    ///   reportName- Report name given by user
    ///   type- Iteration or duration.
    ///   userid- User id
    ///   iteration- Iteration count
    ///   vuScript- Script to be execute
    ///   browserCache- True- Browser cache enabled, else Browser cache disabled
    ///   ipaddress- Ip address is used by this vuser
    /// 
    /// Author: Rasith
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class VUser : IDisposable
    {
        #region Dispose
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _exVariablesValues = null;
                _transactions = null;
                _ReceivedCookies = null;
                _containerId = null;
                _pageId = null;
                cacheUrl = null;
                _IPAddress = null;
                _reportName = null;
                _resposeUrl = null;
                _receivedCookies = null;
                _type = null;
                receivedCookies = null;
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
        #endregion

        public static string Result = string.Empty;

        public event LockReportData OnLockReportData;
        public event LockLog OnLockLog;
        public event LockError OnLockError;
        public event LockTransactions OnLockTransactions;
        public event LockUserDetail OnLockUserDetail;
        public event LockRequestResponse OnLockRequestResponse;
        public event IterationCompleted OnIterationStart;
        public event VUserRunCompleted OnVUserRunCompleted;
        public event VUserCreated OnVUserCreated;
        public event LockVariable OnVariableCreated;
        public event LockResponse OnResponse;
        
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
        private int _bandwidthInKbps = -1;
        private bool _browserCache = false;
        private bool _bReplyThinkTime = true;
        private string _numOfParallelCon = "0";
        private bool _secondaryRequestPlayed = false;
        private XmlNode _vuScriptXml;
        private Constants _Constants = Constants.GetInstance();
        private XmlDocument _doc = null;
        private Thread _userThread;
        private ExecutionReport Status = ExecutionReport.GetInstance();
        //To store extracted variables
        private Dictionary<string, object> _exVariablesValues = new Dictionary<string, object>();
        //To store transaction details
        private Dictionary<string, TransactionRunTimeDetail> _transactions = new Dictionary<string, TransactionRunTimeDetail>();
        //To receive cookies
        private Dictionary<string, string> _ReceivedCookies = new Dictionary<string, string>();
        private Stack<string[]> _containerId = new Stack<string[]>();
        private Stack<string> _pageId = new Stack<string>();
        private List<string> cacheUrl = new List<string>();
        private IPEndPoint _IPAddress = null;
        private Random _random = new Random();

        private Constants _constants = Constants.GetInstance();
        Request req;

        public bool IsValidation = false;

        //To store cookies
        public string HeaderCookie
        {
            get
            {
                StringBuilder cookie = new StringBuilder();
                foreach (KeyValuePair<string, string> param in _ReceivedCookies)
                {
                    cookie.Append(param.Key.ToString()).Append("=").Append(param.Value.ToString()).Append(";");
                }
                if (cookie.Length > 0) cookie.Remove(cookie.Length - 1, 1);
                return cookie.ToString();
            }
            private set { }
        }
        public bool WorkCompleted = false;
        public bool Break { get; set; }
        ConnectionManager conncetionManager = new ConnectionManager(1);
        Dictionary<string, string> receivedCookies = new Dictionary<string, string>();
        public VUserStatus VUserStatus;
        private DateTime _userCreatededTime = new DateTime();
        private bool dynamicRequestsEnabled;

        public VUser(int maxUser, string reportName, string type, int userid, int iteration, XmlNode vuScript, bool browserCache, IPAddress ipaddress, bool bReplyThinkTime, string numOfParallelCon, int bandwidth)
        {
            //Set current time
            _userCreatededTime = DateTime.Now;
            //Assign vuscript xml to local variable
            _doc = vuScript.OwnerDocument;
            _maxUser = maxUser;
            _browserCache = browserCache;
            _bReplyThinkTime = bReplyThinkTime;
            if (numOfParallelCon != null)
            {
                _numOfParallelCon = numOfParallelCon;
            }
            else
            {
                _numOfParallelCon = "6";
            }
            _maxParallelConnections = int.Parse(_numOfParallelCon);

            _type = type;
            _userid = userid;
            _iteration = iteration;
            _vuScriptXml = vuScript;
            _reportName = reportName;
            _bandwidthInKbps = bandwidth;
            _IPAddress = new IPEndPoint(ipaddress, 0);
            VUserStatus = new VUserStatus();
            _scriptName = _vuScriptXml.Attributes["name"].Value;
            lock (Status.LockObjForCreatedUser)
            {
                Status.CreatedUser++;
            }
        }

        ~VUser()
        {
            Dispose(false);
        }

        //To start vuser
        public void Start()
        {
            LockUserDetail(1);
            _userThread = new Thread(new ThreadStart(StartExecution));
            //An apartment is a logical container within a process for objects sharing the same thread access requirements.
            _userThread.ApartmentState = ApartmentState.STA;
            _userThread.Start();
        }

        //To stop vuser.
        public void Stop()
        {
            new Thread(() =>
            {
                Break = true;
                try
                {
                    if (_userThread != null)
                    {
                        try
                        {
                            //To abort request
                            if (req != null) req.Abort();
                            //To stop vuser thread
                            _userThread.Abort();

                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }
                        _userThread = null;
                    }
                    Result = "";
                    _resposeUrl = string.Empty;
                    _receivedCookies = string.Empty;

                    //To execute end container.
                    foreach (XmlNode container in _vuScriptXml.ChildNodes)
                    {
                        if (container.Attributes["name"].Value == "End")
                        {
                            Break = false;
                            _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                            ExecuteContainer(container);
                            _containerId.Pop();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {
                    //To update user completed detail
                    lock (Status.LockObjForCompletedUser)
                    {
                        Status.CompletedUser++;
                        if (OnVUserRunCompleted != null) OnVUserRunCompleted.Invoke(_scriptName,_userid);
                        LockUserDetail(2);
                    }
                    WorkCompleted = true;
                    conncetionManager.CloseAllConnetions();
                    Break = true;
                    this.Dispose();
                }
            }).Start();
        }

        //This is vuser background operation
        private void StartExecution()
        {
            WorkCompleted = false;
            _exVariablesValues.Clear();
            if (OnVUserCreated != null) OnVUserCreated.Invoke(_scriptName,_userid);

            dynamicRequestsEnabled = Convert.ToBoolean(_vuScriptXml.Attributes["dynamicreqenable"].Value);
           
            try
            {
                if (_type == "1")
                {
                    #region Iterations for IterationType

                    for (_index = 1; _index <= _iteration; _index++)
                    {
                        if (Break == true) break;
                       
                        Result = "";
                        _iterationid = _index;
                        _resposeUrl = string.Empty;
                        _receivedCookies = string.Empty;

                        foreach (XmlNode container in _vuScriptXml.ChildNodes)
                        {
                            if (Break == true) break;
                            /* 1st iteration Init,Action. last iteration Action, End. Rest of them Acttion*/
                            if (container.Attributes["name"].Value == "Initialize")
                            {
                                if (_index == 1)
                                {
                                    _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                                    ExecuteContainer(container);
                                    _containerId.Pop();
                                }
                            }
                             // To execute Action container
                            else if (container.Attributes["name"].Value == "Actions")
                            {
                                _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                                ExecuteContainer(container);
                                _containerId.Pop();
                            }
                            // To execute End container
                            else if (container.Attributes["name"].Value == "End")
                            {
                                if (_index == _iteration)
                                {
                                    _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                                    ExecuteContainer(container);
                                    _containerId.Pop();
                                }
                            }
                        }
                        _transactions.Clear();
                        if (OnIterationStart != null) OnIterationStart.Invoke(_scriptName, _userid, _iterationid);
                    }
                    lock (Status.LockObjForCompletedUser)
                    {
                        Status.CompletedUser++;
                        LockUserDetail(2);
                    }
                    if (IsValidation == false) this.Dispose();
                    #endregion
                }
                else
                {
                    #region Iterations for durationtype
                    _iteration = 0;
                    for (_index = 1; true; _index++)
                    {
                        if (Break == true) break;
                        
                        Result = "";
                        _iterationid = _index;
                        _resposeUrl = string.Empty;
                        _receivedCookies = string.Empty;
                        foreach (XmlNode container in _vuScriptXml.ChildNodes)
                        {
                            if (Break == true) break;
                            if (container.Attributes["name"].Value == "Initialize")
                            {
                                if (_index == 1)
                                {
                                    _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                                    ExecuteContainer(container);
                                    _containerId.Pop();
                                }
                            }
                            else if (container.Attributes["name"].Value == "Actions")
                            {
                                _containerId.Push(new string[2] { container.Attributes["id"].Value, container.Attributes["name"].Value });
                                ExecuteContainer(container);
                                _containerId.Pop();
                            }
                        }
                        _transactions.Clear();
                        if (OnIterationStart != null) OnIterationStart.Invoke(_scriptName, _userid, _iterationid);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            finally
            {
                conncetionManager.CloseAllConnetions();
                WorkCompleted = true;
                if (OnVUserRunCompleted != null) OnVUserRunCompleted.Invoke(_scriptName, _userid);
            }

        }

        //It will execute give container. It is also used as recursive function when container is in another container.
        private void ExecuteContainer(XmlNode container)
        {
            try
            {
                if (Break == true) return;
                foreach (XmlNode child in container.ChildNodes)
                {

                    if (Break == true) break;
                    switch (child.Name)
                    {
                        #region Operations
                        case "container":

                            #region Container
                            _containerId.Push(new string[2] { child.Attributes["id"].Value, child.Attributes["name"].Value });
                            ExecuteContainer(child);
                            _containerId.Pop();
                            break;
                            #endregion

                        case "page":

                            #region Page
                            _pageId.Push(child.Attributes["id"].Value);
                            try
                            {
                                if (IsValidation == false)
                                {
                                    if (child.Attributes["delay"].Value.Contains("$$"))
                                    {
                                        EvaluteExp(child.Clone());
                                    }
                                    if (_bReplyThinkTime)
                                    {
                                        System.Threading.Thread.Sleep(child.Attributes["delay"].Value == "" ? 0 : Convert.ToInt32(child.Attributes["delay"].Value));
                                    }
                                }
                                _secondaryRequestPlayed = false;

                                // The below code commented to implement parallel connection feature
                                if (AppedoLT.Core.Constants.GetInstance().btnExecutionType == "Run")
                                {
                                    Boolean enablePrallel = false;
                                    foreach (XmlNode req in child.ChildNodes)
                                    {
                                        enablePrallel = false;
                                        req.Attributes["Address"].Value = new StringBuilder().Append(req.Attributes["Schema"].Value).Append("://").Append(req.Attributes["Host"].Value).Append(":").Append(req.Attributes["Port"].Value).Append(req.Attributes["Path"].Value).ToString();
                                       
                                        try
                                        {
                                            if (Convert.ToBoolean(req.Attributes["IsEnable"].Value) == false)
                                                continue;

                                            XmlNode requestHeadeNode = req.SelectSingleNode("./headers/header[@name='Accept']");
                                            // Match mat = new Regex("Content-Type: (.*?)\r\n", RegexOptions.Singleline | RegexOptions.Multiline).Match(req.Attributes["ResponseHeader"].Value);
                                            if (requestHeadeNode != null)
                                            {
                                                string acceptType = requestHeadeNode.Attributes["value"].Value.ToLower();
                                                enablePrallel = (acceptType.Contains("application") == false && acceptType.Contains("text/html") == false);
                                            }

                                            if (enablePrallel)
                                            {
                                                ProcessParalell(req.Clone());
                                            }
                                            else
                                            {
                                                ProcessSequential(req.Clone());
                                            }

                                            if (_secondaryRequestPlayed == true)
                                            {
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                        }
                                    }

                                    // Wait till all threads complete their work
                                    while (_requestProcessingThreadCount > 0)
                                    {
                                        Thread.Sleep(10);
                                    }
                                }
                                else
                                {                                    
                                    foreach (XmlNode req in child.ChildNodes)
                                    {
                                        if (Break == true) break;
                                        RequestCountHandler._ReqCount++;
                                        ProcessRequest(req.Clone());
                                        if (_secondaryRequestPlayed == true)
                                        {
                                            break;
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            }

                            _pageId.Pop();
                            break;
                            #endregion

                        case "loop":

                            #region Loop
                            for (int index = 1; index <= Convert.ToInt32(child.Attributes["loopcount"].Value); index++)
                            {
                                if (Break == true) break;
                                ExecuteContainer(child);
                            }
                            break;
                            #endregion

                        case "whileloop":

                            #region WhileLoop
                            while (true)
                            {
                                if (Break == true) break;
                                string expression1 = child.Attributes["condition"].Value;

                                #region Parm has variable
                                if (expression1.Contains("$$") == true)
                                {
                                    EvalutionResult EvalutionResult = EvaluteExp(expression1);
                                    expression1 = EvalutionResult.isSuccess == true ? EvalutionResult.value : false.ToString();

                                }
                                #endregion

                                ExpressionEval eval1 = new ExpressionEval();
                                eval1.Expression = expression1;

                                bool result1 = false;
                                try
                                {
                                    result1 = eval1.EvaluateBool();
                                }
                                catch
                                {
                                }

                                #region IfThenElse Container
                                if (result1 == false) break;
                                else
                                {
                                    ExecuteContainer(child);
                                }
                                #endregion
                            }
                            break;
                            #endregion

                        case "if":

                            #region If
                            string expression = child.Attributes["condition"].Value;

                            #region Parm has variable
                            if (expression.Contains("$$") == true)
                            {
                                EvalutionResult EvalutionResult = EvaluteExp(expression);
                                expression = EvalutionResult.isSuccess == true ? EvalutionResult.value : false.ToString();
                            }
                            #endregion

                            ExpressionEval eval = new ExpressionEval();
                            eval.Expression = expression;

                            bool result = eval.EvaluateBool();

                            #region IfThenElse Container

                            if (result == true)
                            {
                                ExecuteContainer(child.ChildNodes[0]);
                            }
                            else
                            {
                                ExecuteContainer(child.ChildNodes[1]);
                            }

                            #endregion

                            break;
                            #endregion

                        case "request":
                            #region Request
                            AppedoLogger.Log(new LogMessage()
                            {
                                ThreadID = Thread.CurrentThread.ManagedThreadId,
                                TotalThread = Process.GetCurrentProcess().Threads.Count,
                                ActiveThreads = ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads).OfType<System.Diagnostics.ProcessThread>().Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running).Count(),
                                UserID = _userid,
                                IterationNumber = _iterationid,
                                RequestID = (child == null || child.Attributes["id"] == null) ? "" : child.Attributes["id"].Value,
                                ResponseID = 0,
                                Timestamp = DateTime.Now,
                                Status = "Before Request, Direct",
                                Request = (child == null || child.Attributes["Address"] == null) ? "" : child.Attributes["Address"].Value
                            });

                            ProcessRequest(child.Clone());

                            AppedoLogger.Log(new LogMessage()
                            {
                                ThreadID = Thread.CurrentThread.ManagedThreadId,
                                TotalThread = Process.GetCurrentProcess().Threads.Count,
                                ActiveThreads = ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads).OfType<System.Diagnostics.ProcessThread>().Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running).Count(),
                                UserID = _userid,
                                IterationNumber = _iterationid,
                                RequestID = (child == null || child.Attributes["id"] == null) ? "" : child.Attributes["id"].Value,
                                ResponseID = 0,
                                Timestamp = DateTime.Now,
                                Status = "After Request, Direct",
                                Request = (child == null || child.Attributes["Address"] == null) ? "" : child.Attributes["Address"].Value
                            });
                            break;
                            #endregion

                        case "delay":

                            #region Delay
                            System.Threading.Thread.Sleep(Convert.ToInt32(child.Attributes["delaytime"].Value));
                            break;
                            #endregion

                        case "log":

                            #region Log
                            LockLog(child.Clone());
                            break;
                            #endregion

                        case "javascript":

                            #region Javascript
                            ProcessJavascrit(child);
                            break;
                            #endregion

                        case "starttransaction":

                            #region Starttransaction
                            {
                                TransactionRunTimeDetail tranDetail = new TransactionRunTimeDetail();
                                tranDetail.ScriptId = _vuScriptXml.Attributes["id"].Value;
                                tranDetail.ScenarioName = Status.ScenarioName;
                                tranDetail.ScriptName = _vuScriptXml.Attributes["id"].Value;
                                tranDetail.UserId = _userid.ToString();
                                tranDetail.IterationId = _index.ToString();
                                tranDetail.StartTime = DateTime.Now;
                                tranDetail.TransactionName = child.Attributes["transactionname"].Value;
                                _transactions.Add(child.Attributes["transactionname"].Value, tranDetail);
                            }
                            break;
                            #endregion

                        case "endtransaction":

                            #region  Endtransaction
                            if (_transactions.ContainsKey(child.Attributes["transactionname"].Value))
                            {
                                TransactionRunTimeDetail tranDetailTemp = _transactions[child.Attributes["transactionname"].Value];
                                Thread.Sleep(100);
                                tranDetailTemp.EndTime = DateTime.Now;
                                tranDetailTemp.IsEnd = true;
                                LockTransactions(tranDetailTemp);
                            }
                            break;
                            #endregion

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("Thread was being aborted".ToLower()) == false)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
        }

        //It will process a single http or tcp request
        private void ProcessRequest(XmlNode request)
        {
            if (Break == true) { return; }

            string url = (request == null || request.Attributes["Address"] == null) ? "" : request.Attributes["Address"].Value;
            string requestId = (request == null || request.Attributes["id"] == null) ? "" : request.Attributes["id"].Value;
            AppedoLogger.Log(new LogMessage()
            {
                ThreadID = Thread.CurrentThread.ManagedThreadId,
                TotalThread = Process.GetCurrentProcess().Threads.Count,
                ActiveThreads = ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads).OfType<System.Diagnostics.ProcessThread>().Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running).Count(),
                UserID = _userid,
                IterationNumber = _iterationid,
                RequestID = requestId,
                ResponseID = 0,
                Timestamp = DateTime.Now,
                Status = "Before Request, Sequential",
                Request = url
            });

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
                        Connection con=new Connection(request.Attributes["serverip"].Value, int.Parse(request.Attributes["port"].Value));
                       
                        req = new TcpRequest(request, con, false, _bandwidthInKbps);
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
                            LockResponseTime(req.RequestNode.Attributes["id"].Value, req.RequestNode.Attributes["Path"] == null ? req.RequestName : req.RequestNode.Attributes["Path"].Value, req.StartTime, req.FirstByteReceivedTime, req.EndTime, req.TimeForFirstByte, req.ResponseTime, req.ResponseSize, req.ResponseCode.ToString());
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Http
                    bool cacheEnabled = false;
                    //If request is enabled
                    if (request != null && Convert.ToBoolean(request.Attributes["IsEnable"].Value) == true)
                    {
                        try
                        {
                            List<AppedoLT.Core.Tuple<string, string>> variables = new List<AppedoLT.Core.Tuple<string, string>>();
                            //If request has parameterization. 
                            if (request.OuterXml.Contains("$$"))
                            {
                                //It will replace variable with corresponding value.
                                variables = EvaluteExp(request);
                            }
                            request.Attributes["Address"].Value = new StringBuilder().Append(request.Attributes["Schema"].Value).Append("://").Append(request.Attributes["Host"].Value).Append(":").Append(request.Attributes["Port"].Value).Append(request.Attributes["Path"].Value).ToString();
                            Uri temp = new Uri(request.Attributes["Address"].Value);
                            
                            Match mat = new Regex("Content-Type: (.*?)\r\n", RegexOptions.Singleline | RegexOptions.Multiline).Match(request.Attributes["ResponseHeader"].Value);
                                                     
                            //If Browser cache enabled
                            if (_browserCache == true && _index > 1)
                            {
                                try
                                {
                                    XmlNode requestHeadeNode = request.SelectSingleNode("./headers/header[@name='Accept']");

                                    if (requestHeadeNode != null && requestHeadeNode.Attributes["value"].Value.Contains("/"))
                                    {
                                        if (requestHeadeNode.Attributes["value"].Value.ToLower().Contains("application") == false)
                                        {
                                            string acceptType = requestHeadeNode.Attributes["value"].Value.Split('/')[1];
                                            acceptType = acceptType.ToLower();
                                            //Filter request if Browser cache enabled
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
                                    //Filter request if Browser cache enabled
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
                            //Check request is not in exclutionfiletypes
                            if (cacheEnabled == false && !(fileNameExt != string.Empty && _vuScriptXml.Attributes["exclutionfiletypes"].Value.Contains(fileNameExt.Replace(".", string.Empty).Trim().ToLower()) == true))
                            {
                                req = new HttpRequest(request, ref receivedCookies, _userid.ToString() + (_createdConnection++ % _maxConnection).ToString(), _IPAddress, IsValidation, _bandwidthInKbps);
                                req.Variables = variables;
                                req.GetResponse();

                                //For validation
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

                                if (OnResponse != null)
                                {
                                    // Log the response if someone has subscribed for the event
                                    StringBuilder pDataBuffer = new StringBuilder();
                                    foreach (PostData pData in GetPostData(request.SelectSingleNode("//params")))
                                    {
                                        pDataBuffer.Append(pData.value.ToString());
                                    }
                                    responseResult.PostData = pDataBuffer.ToString();
                                    responseResult.RequestResult = req;
                                    responseResult.WebRequestResponseId = Convert.ToInt32(Constants.GetInstance().UniqueID);

                                    ResponseDetail details = new ResponseDetail();
                                    details.ReportName = _reportName;
                                    details.UserId = _userid;
                                    details.ScriptName = _scriptName;
                                    details.IterationId = _iterationid;
                                    if (responseResult != null && responseResult.RequestResult != null)
                                    {
                                        details.ResponseCode = responseResult.RequestResult.ResponseCode;
                                        bool writeResponseData = false;
                                        if (mat.Success == true && mat.Groups[1] != null && mat.Groups[1].Value.Contains("/"))
                                        {
                                            if (mat.Groups[1].Value.ToLower().Contains("application") || mat.Groups[1].Value.ToLower().Contains("text"))
                                            {
                                                writeResponseData = true;
                                            }
                                        }

                                        if (writeResponseData)
                                        {
                                            details.ResponseString = responseResult.RequestResult.ResponseStr;
                                        }
                                        else
                                        {
                                            details.ResponseString = "[[Data writing skipped]]";
                                        }
                                        details.RequestName = responseResult.RequestResult.RequestName;
                                    }

                                    OnResponse.Invoke(details);
                                }
                                LockResponseTime(req.RequestNode.Attributes["id"].Value, req.RequestNode.Attributes["Path"] == null ? req.RequestName : req.RequestNode.Attributes["Path"].Value, req.StartTime, req.FirstByteReceivedTime, req.EndTime, req.TimeForFirstByte, req.ResponseTime, req.ResponseSize, req.ResponseCode.ToString());
                                //string aa = System.Configuration.ConfigurationSettings.AppSettings.Get("xx");
                                
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
                                            Request secReq = new HttpRequest(request, links.Dequeue(), receivedCookies, _userid.ToString() + (_createdConnection++ % _maxConnection).ToString(), _IPAddress, IsValidation, _bandwidthInKbps);
                                            //  Request secReq = new HttpRequest(request, links.Dequeue(), receivedCookies, _userid.ToString(), _IPAddress, IsValidation);

                                            RequestResponse responseResultSec = new RequestResponse();

                                            if (_browserCache == false || cacheUrl.Exists(t => t.CompareTo(secReq.RequestNode.Attributes["Address"].Value) == 0) == false)
                                            {
                                                cacheUrl.Add(secReq.RequestNode.Attributes["Address"].Value);
                                               if (secReq.RequestNode.Attributes["Path"].Value.StartsWith("//") == true) secReq.RequestNode.Attributes["Path"].Value = secReq.RequestNode.Attributes["Path"].Value.Replace("//", "/");
                                             
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
                                                                if (!secReq.ErrorCode.StartsWith("2") && !secReq.ErrorCode.StartsWith("3"))
                                                                {
                                                                    LockException(Convert.ToString(secReq.RequestId), secReq.ErrorMessage, secReq.ErrorCode, secReq.RequestNode.Attributes["Address"].Value);
                                                                }
                                                            }

                                                            if (OnResponse != null)
                                                            {
                                                                mat = new Regex("Content-Type: (.*?)\r\n", RegexOptions.Singleline | RegexOptions.Multiline).Match(req.RequestNode.Attributes["ResponseHeader"].Value);
                             
                                                                responseResult.PostData = secReq.ToString();
                                                                responseResult.RequestResult = secReq;
                                                                responseResult.WebRequestResponseId = Convert.ToInt32(Constants.GetInstance().UniqueID);

                                                                ResponseDetail details = new ResponseDetail();
                                                                details.ReportName = _reportName;
                                                                details.UserId = _userid;
                                                                details.ScriptName = _scriptName;
                                                                details.IterationId = _iterationid;
                                                                if (responseResult != null && responseResult.RequestResult != null)
                                                                {
                                                                    details.ResponseCode = responseResult.RequestResult.ResponseCode;
                                                                    bool writeResponseData = false;
                                                                    if (mat.Success == true && mat.Groups[1] != null && mat.Groups[1].Value.Contains("/"))
                                                                    {
                                                                        if (mat.Groups[1].Value.ToLower().Contains("application") || mat.Groups[1].Value.ToLower().Contains("text"))
                                                                        {
                                                                            writeResponseData = true;
                                                                        }
                                                                    }

                                                                    if (writeResponseData)
                                                                    {
                                                                        details.ResponseString = responseResult.RequestResult.ResponseStr;
                                                                    }
                                                                    else
                                                                    {
                                                                        details.ResponseString = "[[Data writing skipped]]";
                                                                    }
                                                                    details.RequestName = responseResult.RequestResult.RequestName;
                                                                }

                                                                OnResponse.Invoke(details);
                                                            }
                                                            LockResponseTime(req.RequestNode.Attributes["id"].Value, secReq.RequestNode.Attributes["Path"] == null ? secReq.RequestName : secReq.RequestNode.Attributes["Path"].Value, secReq.StartTime, secReq.FirstByteReceivedTime, secReq.EndTime, secReq.TimeForFirstByte, secReq.ResponseTime, secReq.ResponseSize, secReq.ResponseCode.ToString());
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
                AppedoLogger.Log(new LogMessage()
                {
                    ThreadID = Thread.CurrentThread.ManagedThreadId,
                    TotalThread = Process.GetCurrentProcess().Threads.Count,
                    ActiveThreads = ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads).OfType<System.Diagnostics.ProcessThread>().Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running).Count(),
                    UserID = _userid,
                    IterationNumber = _iterationid,
                    RequestID = requestId,
                    ResponseID = 0,
                    Timestamp = DateTime.Now,
                    Status = "After Request, Sequential",
                    Request = url
                });

                req = null;
                responseResult = null;
                request = null;
            }
        }

        #region HTTP Functions
        private void ProcessJavascrit(XmlNode scriptNode)
        {
            try
            {
                StringBuilder script = new StringBuilder();
                script.Append("<script type=\"text/javascript\">").Append(Environment.NewLine);
                script.Append(scriptNode.Attributes["script"].Value);
                script.Append("</script>");
                using (WebBrowser JavaScriptRunner = new WebBrowser())
                {
                    // JavaScriptRunner
                    JavaScriptRunner.Navigate("about:blank");
                    JavaScriptRunner.ObjectForScripting = this;
                    if (IsValidation == true) JavaScriptRunner.ScriptErrorsSuppressed = false;
                    else
                    {
                        JavaScriptRunner.ScriptErrorsSuppressed = true;
                    }
                    JavaScriptRunner.Document.Write(script.ToString());
                    JavaScriptRunner.Document.Window.Error += new HtmlElementErrorEventHandler(GetScriptException);
                    JavaScriptRunner.Document.InvokeScript("main");
                }
            }
            catch (Exception ex)
            {
                LockException("0", ex.Message, "700", string.Empty);
            }
        }

        //Get query string from given queryString node.
        private string GetQueryString(XmlNode queryString)
        {
            StringBuilder result = new StringBuilder();
            foreach (XmlNode parm in queryString.ChildNodes)
            {
                result.Append(parm.Attributes["name"].Value).Append("=").Append(System.Web.HttpUtility.UrlEncode(parm.Attributes["value"].Value)).Append("&"); ;
            }
            if (result.Length > 0) result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        //Get post data from post data node
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

        //Get value for given variable. 
        private string GetVariableValue(string variablename, bool isEvaluation)
        {
            string result = string.Empty;

            //If given variable is in extractor values
            if (_exVariablesValues.ContainsKey(variablename) == true)
            {
                result = _exVariablesValues[variablename].ToString();
            }
            //Else it will search given variable in file. 
            else
            {
                string type = VariableManager.dataCenter.GetVariableType(variablename.Split('.')[0]);
                if (type == "file" || type == "string" || type == "number" || type == "randomnumber" || type == "randomstring" || type == "currentdate")
                {
                    result = VariableManager.dataCenter.GetVariableValue(_userid, _iterationid, variablename, _maxUser).ToString();
                }
            }

            if (OnVariableCreated != null && !isEvaluation)
            {
                VariableDetail detail = new VariableDetail();
                detail.UserId = _userid;
                detail.ReportName = _reportName;
                detail.ScriptName = _scriptName;
                detail.RequestedAt = DateTime.Now;
                detail.IterationId = _iterationid;
                detail.VariableName = variablename;
                detail.Value = System.Web.HttpUtility.HtmlDecode(result);

                OnVariableCreated.Invoke(detail);
            }

            return System.Web.HttpUtility.HtmlDecode(result);

        }

        //To replace variable name with value.
        public object GetValue(object variableName)
        {
            if (_exVariablesValues.ContainsKey(variableName.ToString()) == true)
            {
                if (variableName.ToString().EndsWith("_rand"))
                {
                    Random rand = new Random();
                    long tem = (rand.Next() % Int64.Parse(_exVariablesValues[variableName.ToString()].ToString()));
                    variableName = variableName.ToString().Replace("rand", tem.ToString());
                }
                return _exVariablesValues[variableName.ToString()];
            }
            else
            {
                try
                {
                    return VariableManager.dataCenter.GetVariableValue(_userid, _iterationid, variableName.ToString(), _maxUser);
                }
                catch
                {
                    throw new EvaluteException(string.Format("Unable to evalute {0} value. Userid:{1}, Iterationid:{2}, ScriptName:{3}", variableName, _userid, _iterationid, _vuScriptXml.Attributes["name"].Value));
                }
            }
        }

        //It will replace all variables with value in given xmlnode(Http request).
        public List<AppedoLT.Core.Tuple<string, string>> EvaluteExp(XmlNode expression, bool isEvaluation = false)
        {
            #region Parm has variable
            List<AppedoLT.Core.Tuple<string, string>> Varialbles = new List<AppedoLT.Core.Tuple<string, string>>();

            //Regular exp for find variable($$variableName$$)
            Regex regex = new Regex(@"\$\$([a-zA-Z_][a-zA-Z0-9_.]*)\$\$");

            Match match = null;

            //Replace variable in attributes
            foreach (XmlAttribute attribute in expression.Attributes)
            {
                match = regex.Match(attribute.Value);

                while (match.Success == true)
                {
                    AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                    parm.Key = match.Groups[1].Value;
                    try
                    {
                        parm.Value = GetVariableValue(parm.Key, isEvaluation);
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

            //Replace rest of variables
            while (match.Success == true)
            {
                AppedoLT.Core.Tuple<string, string> parm = new AppedoLT.Core.Tuple<string, string>();
                parm.Key = match.Groups[1].Value;
                try
                {
                    parm.Value = GetVariableValue(parm.Key, isEvaluation);
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

        //It will replace all variables with value in given xmlnode(TCP request).
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
                        parm.Value = GetVariableValue(parm.Key, false);
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
                    parm.Value = GetVariableValue(parm.Key, false);
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

        #endregion

        #region TCP Function
        //Get response for tcp request.
        private RequestResponse GetResponse(string containerid, string containername, string pageid, XmlNode tcpRequest, int _userid, int _iterationid, ref RequestResponse requestResponse)
        {
            //If it is validation
            if (IsValidation == true) requestResponse.IsSucess = true;
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            DateTime firstbytetime = new DateTime();
            Stopwatch elapsedTimer = new Stopwatch();
            Stopwatch firstbyteTimer = new Stopwatch();
            StringBuilder response = new StringBuilder();
            ASCIIEncoding asen = new ASCIIEncoding();
            bool isAssertionFaild = false;
            StringBuilder assertionFaildMsg = new StringBuilder();
            byte[] requestBytes;
            byte[] receiveBuffer = new byte[1];
            int responseSize;

            bool requestsizeconstant = false;
            bool responsesizeconstant = false;

            string responseCode = "200";
            string requestStr = tcpRequest.Attributes["requestcontent"].Value;
            try
            {
                //Replace all variables with value.
                EvalutionResult EvalutionResult = EvaluteExp(requestStr);
                if (EvalutionResult.isSuccess == true)
                {
                    requestStr = EvalutionResult.value;
                }
                else
                {
                    //Store exception
                    LockException(tcpRequest.Attributes["id"].Value, EvalutionResult.value, "600", tcpRequest.Attributes["name"].Value);
                }
                //Replace all variables with value(in parameters).
                foreach (XmlNode reqParam in tcpRequest.SelectNodes("params/param"))
                {
                    XmlNode param = reqParam.CloneNode(true);
                    //If contain variable
                    if (param.Attributes["value"].Value.Contains("$$") == true)
                    {
                        EvalutionResult = EvaluteExp(param.Attributes["value"].Value);
                        if (EvalutionResult.isSuccess == true)
                        {
                            param.Attributes["value"].Value = EvalutionResult.value;
                        }
                        else
                        {
                            LockException(tcpRequest.Attributes["id"].Value, EvalutionResult.value, "600", tcpRequest.Attributes["name"].Value);
                        }
                    }

                    #region Padding(Adding Char)(old logic
                    if (param.Attributes["length"] != null && param.Attributes["length"].Value.Length > 0)
                    {
                        int totalWidth = Convert.ToInt32(param.Attributes["length"].Value);
                        int difference = totalWidth - param.Attributes["value"].Value.Length;
                        if (difference > 0)
                        {
                            if (param.Attributes["paddingtype"].Value == "L") param.Attributes["value"].Value = param.Attributes["value"].Value.PadLeft(totalWidth, param.Attributes["paddingchar"].Value[0]);
                            else if (param.Attributes["paddingtype"].Value == "R") param.Attributes["value"].Value = param.Attributes["value"].Value.PadRight(totalWidth, param.Attributes["paddingchar"].Value[0]);
                        }
                        else if (difference < 0)
                        {
                            param.Attributes["value"].Value = param.Attributes["value"].Value.Substring(0, Convert.ToInt32(param.Attributes["length"].Value));
                        }
                    }
                    #endregion

                    if (IsValidation == true)
                    {
                        Parameter parm = new Parameter();
                        parm.Name = param.Attributes["name"].Value;
                        parm.Value = param.Attributes["value"].Value;
                        requestResponse.Parameters.Add(parm);
                    }
                    requestStr = requestStr.Replace(requestStr.Substring(Convert.ToInt16(param.Attributes["startposition"].Value) - 1, Convert.ToInt16(param.Attributes["length"].Value)), param.Attributes["value"].Value);
                }
            }
            catch (EvaluteException ex)
            {
                requestResponse.RequestId = tcpRequest.Attributes["id"].Value;
                requestResponse.StartTime = DateTime.Now;
                requestResponse.EndTime = DateTime.Now;
                requestResponse.FirstByteReceivedTime = DateTime.Now;
                requestResponse.TimeForFirstByte = 0; // Set as zero since there is exception 
                requestResponse.Duration = elapsedTimer.Elapsed.TotalMilliseconds;
                requestResponse.IsSucess = false;
                requestResponse.TcpIPResponse = ex.Message;
                requestResponse.TcpIPRequest = requestStr;
                requestResponse.ResponseCode = "400";

                RequestException exception = new RequestException();
                exception.reportname = _reportName;
                exception.scenarioname = Status.ScenarioName;
                exception.scriptname = _vuScriptXml.Attributes["name"].Value;
                exception.requestid = tcpRequest.Attributes["id"].Value;
                exception.request = tcpRequest.Attributes["name"].Value;
                exception.iterationid = this._iterationid.ToString();
                exception.userid = this._userid.ToString();
                exception.requestexceptionid = Guid.NewGuid().ToString();
                exception.time = DateTime.Now;
                exception.from = "Tool";
                exception.message = ex.Message;
                exception.errorcode = "700";
                LockException(exception);

                requestResponse.IsSucess = false;
                responseCode = exception.errorcode;
                return requestResponse;
            }

            requestBytes = asen.GetBytes(requestStr);

            try
            {
                requestResponse.StartTime = start = DateTime.Now;
                elapsedTimer.Start();
                firstbyteTimer.Start();
                Connection con;
                lock (conncetionManager)
                {
                    //Get connection.
                    con = conncetionManager.GetConnection(tcpRequest.Attributes["serverip"].Value, int.Parse(tcpRequest.Attributes["port"].Value));
                }
                lock (con)
                {
                    try
                    {
                        try
                        {
                            //Clear NetworkStream
                            con.NetworkStream.Flush();
                            //Send data
                            con.NetworkStream.Write(requestBytes, 0, requestBytes.Length);
                        }
                        catch
                        {
                            requestResponse.StartTime = start = DateTime.Now;
                            elapsedTimer.Reset();
                            elapsedTimer.Start();

                            firstbyteTimer.Reset();
                            firstbyteTimer.Start();

                            con.Reconnect();
                            con.NetworkStream.Flush();
                            con.NetworkStream.Write(requestBytes, 0, requestBytes.Length);

                        }
                        //Set ReadTimeout to 120sec
                        con.NetworkStream.ReadTimeout = 120000;

                        //Set ReceiveTimeout to 120sec
                        con.Client.ReceiveTimeout = 120000;

                        int timeOut = 1000;
                        //Read all data
                        while (con.Client.Available < Convert.ToInt32(tcpRequest.Attributes["responsesize"].Value))
                        {
                            Thread.Sleep(10);
                            timeOut = timeOut - 10;
                            if (timeOut <= 0) break;
                        }
                        timeOut = 119000;
                        //Read if there is any bending data
                        while (con.Client.Available <= 0)
                        {
                            Thread.Sleep(10);
                            timeOut = timeOut - 10;
                            if (timeOut <= 0) break;
                        }

                        if (con.Client.Available > 0)
                        {
                            firstbytetime = DateTime.Now;
                            requestResponse.FirstByteReceivedTime = firstbytetime;
                            firstbyteTimer.Stop();
                        }

                        //Read if there is any bending data
                        while (con.Client.Available != 0 && (responseSize = con.NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                        {
                            for (int index = 0; index < responseSize; index++)
                                response.Append(Convert.ToChar(receiveBuffer[index]));
                            if (con.Client.Available == 0) break;
                        }
                        requestResponse.EndTime = end = DateTime.Now;

                        //Read if there is any bending data
                        if (Convert.ToInt32(tcpRequest.Attributes["responsesize"].Value) > response.Length)
                        {
                            Thread.Sleep(10);
                            while (con.Client.Available != 0 && (responseSize = con.NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                            {
                                for (int index = 0; index < responseSize; index++)
                                    response.Append(Convert.ToChar(receiveBuffer[index]));
                                requestResponse.EndTime = end = DateTime.Now;

                                if (con.Client.Available == 0) break;
                            }

                        }

                        con.NetworkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    finally
                    {
                        con.IsHold = false;
                        if (firstbyteTimer.IsRunning)
                        {
                            firstbyteTimer.Stop();
                        }
                        elapsedTimer.Stop();
                        requestResponse.Duration = elapsedTimer.Elapsed.TotalMilliseconds;
                        requestResponse.TimeForFirstByte = firstbyteTimer.Elapsed.TotalMilliseconds;
                    }
                }

                #region Assertion
                requestsizeconstant = Convert.ToBoolean(tcpRequest.Attributes["requestsizeconstant"].Value);
                responsesizeconstant = Convert.ToBoolean(tcpRequest.Attributes["responsesizeconstant"].Value);
                if (response.Length == 0)
                {
                    isAssertionFaild = true;
                    assertionFaildMsg.Append("Request timeout.").Append(Environment.NewLine);

                }
                if (requestsizeconstant == true && requestStr.Length != Convert.ToInt16(tcpRequest.Attributes["requestsize"].Value))
                {
                    isAssertionFaild = true;
                    assertionFaildMsg.Append("Request size(" + requestStr.Length + ") is not match with actual request size(" + Convert.ToInt16(tcpRequest.Attributes["requestsize"].Value) + ").").Append(Environment.NewLine);

                }
                if (responsesizeconstant == true && response.Length != Convert.ToInt16(tcpRequest.Attributes["responsesize"].Value))
                {
                    isAssertionFaild = true;
                    assertionFaildMsg.Append("Reponse size(" + response.Length + ") is not match with actual response size(" + Convert.ToInt16(tcpRequest.Attributes["responsesize"].Value) + ")").Append(Environment.NewLine);
                }
                if (response.Length > 0)
                {
                    if (tcpRequest.SelectSingleNode("assertions") != null)
                    {
                        foreach (XmlNode assertion in tcpRequest.SelectNodes("assertions/assertion"))
                        {

                            switch (assertion.Attributes["type"].Value)
                            {
                                case "0":
                                    #region Req-NotContain
                                    try
                                    {
                                        int position = Convert.ToInt16(assertion.Attributes["reqposition"].Value);
                                        int length = Convert.ToInt16(assertion.Attributes["reqlength"].Value);
                                        string str = assertion.Attributes["reqtext"].Value;
                                        if (requestStr.Substring(position - 1, length) != str)
                                        {
                                            isAssertionFaild = true;
                                            assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        isAssertionFaild = true;
                                        assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                    }
                                    #endregion
                                    break;
                                case "1":
                                    #region Res-NotContain
                                    try
                                    {
                                        int position = Convert.ToInt16(assertion.Attributes["resposition"].Value);
                                        int length = Convert.ToInt16(assertion.Attributes["reslength"].Value);


                                        string str = assertion.Attributes["restext"].Value;
                                        if (response.ToString().Substring(position - 1, length) != str)
                                        {
                                            isAssertionFaild = true;
                                            assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        isAssertionFaild = true;
                                        assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                    }
                                    #endregion
                                    break;
                                case "2":
                                case "3":
                                    #region Req-Res Equal or no equal
                                    try
                                    {
                                        int reqposition = Convert.ToInt16(assertion.Attributes["reqposition"].Value);
                                        int reqlength = Convert.ToInt16(assertion.Attributes["reqlength"].Value);

                                        int resposition = Convert.ToInt16(assertion.Attributes["resposition"].Value);
                                        int reslength = Convert.ToInt16(assertion.Attributes["reslength"].Value);

                                        string req = requestStr.Substring(reqposition - 1, reqlength);
                                        string res = response.ToString().Substring(resposition - 1, reslength);
                                        if (assertion.Attributes["type"].Value == "2" && req == res)
                                        {
                                            isAssertionFaild = true;
                                            assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        }
                                        else if (assertion.Attributes["type"].Value == "3" && req != res)
                                        {
                                            isAssertionFaild = true;
                                            assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        isAssertionFaild = true;
                                        assertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine); ;
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                    }
                                    #endregion
                                    break;
                            }
                        }
                    }
                }
                #endregion
                //If assertion failed
                if (isAssertionFaild == true)
                {
                    assertionFaildMsg.Append(Environment.NewLine).Append("Request sent:").Append(Environment.NewLine).Append(requestStr).Append(Environment.NewLine).Append("Received Respose: ").Append(Environment.NewLine).Append(response.ToString()).Append(Environment.NewLine);

                    requestResponse.IsSucess = false;
                    requestResponse.TcpIPResponse = assertionFaildMsg.ToString();
                    requestResponse.TcpIPRequest = requestStr;
                    responseCode = "800";
                    requestResponse.IsSucess = false;
                    LockException(tcpRequest.Attributes["id"].Value, assertionFaildMsg.ToString(), responseCode, tcpRequest.Attributes["name"].Value);
                }
                else
                {
                    requestResponse.IsSucess = true;
                    requestResponse.TcpIPResponse = response.ToString();
                    requestResponse.TcpIPRequest = requestStr;
                    requestResponse.ResponseCode = "True";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                {
                    requestResponse.IsSucess = false;
                    requestResponse.TcpIPResponse = ex.Message;
                    requestResponse.TcpIPRequest = requestStr;
                    requestResponse.IsSucess = false;
                    LockException(tcpRequest.Attributes["id"].Value, ex.Message, "700", tcpRequest.Attributes["name"].Value); 
                    if (firstbyteTimer.IsRunning)
                    {
                        firstbytetime = DateTime.Now;
                        firstbyteTimer.Stop();
                    }
                    end = DateTime.Now;
                    elapsedTimer.Stop();
                }
            }

            elapsedTimer.Stop();
            requestResponse.Duration = elapsedTimer.Elapsed.TotalMilliseconds;
            requestResponse.RequestId = tcpRequest.Attributes["id"].Value; ;
            if (IsValidation != true)
            {

                LockResponseTime(tcpRequest.Attributes["id"].Value, tcpRequest.Attributes["name"].Value, start, firstbytetime, end, firstbyteTimer.Elapsed.TotalMilliseconds, elapsedTimer.Elapsed.TotalMilliseconds, response.Length, responseCode);
            }
            return requestResponse;
        }

        //Set variable value from javascript
        public void SetValue(object variableName, object value)
        {
            VariableManager.dataCenter.SetVariableValue(_userid, _iterationid, variableName.ToString(), value, _maxUser);
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

        #endregion

        #region Utility
        //Get attributes from document
        private XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = _doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        //Used to get list of link for secondary request
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
                parse.Reset();
                while (parse.ParseNext("script", out tag))
                {

                    if (tag.Attributes.TryGetValue("src", out value))
                    {
                        links.Enqueue(baseURL + value);
                    }
                }
            }
            return links;
        }

        //Lock exception during html parsing
        public void GetScriptException(object sender, HtmlElementErrorEventArgs arg)
        {
            RequestException exception = new RequestException();
            exception.reportname = _reportName;
            exception.scenarioname = Status.ScenarioName;
            exception.scriptname = _vuScriptXml.Attributes["name"].Value;
            exception.requestid = "0";

            exception.iterationid = this._iterationid.ToString();
            exception.userid = this._userid.ToString();
            exception.requestexceptionid = Guid.NewGuid().ToString();
            exception.time = DateTime.Now;
            exception.from = "Script";
            exception.message = arg.Description;
            exception.errorcode = "700";
            LockException(exception);
        }

        #endregion

        #region Logs

        /// <summary>
        ///  Lock Exception
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

        //Lock exception
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

        //Lock log
        private void LockLog(XmlNode log)
        {
            try
            {
                Log logObj = new Log();
                logObj.logid = log.Attributes["id"].Value;
                EvaluteExp(log, true);
                logObj.logname = log.Attributes["name"].Value;
                logObj.reportname = _reportName;
                logObj.scenarioname = Status.ScenarioName;
                logObj.scriptid = _vuScriptXml.Attributes["id"].Value;
                logObj.scriptname = _vuScriptXml.Attributes["name"].Value;
                logObj.iterationid = this._iterationid.ToString();
                logObj.userid = this._userid.ToString();
                logObj.time = DateTime.Now;
                logObj.message = HttpUtility.HtmlDecode(log.Attributes["message"].Value);
                if (OnLockLog != null && logObj != null) OnLockLog.Invoke(logObj);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        //Lock report data
        private void LockResponseTime(string requestid, string address, DateTime starttime, DateTime firstbytetime, DateTime endtime, double timeforfirstbyte, double diff, long responsesize, string reponseCode)
        {
            try
            {
                if (Break == false)
                {
                    Interlocked.Increment(ref RequestCountHandler._ReqCount);

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
                    rd.firstbytereceivedtime = firstbytetime;
                    rd.endtime = endtime;
                    rd.diff = diff;
                    rd.timeforfirstbyte = timeforfirstbyte;
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

        //Lock transaction
        private void LockTransactions(TransactionRunTimeDetail tranDetailTemp)
        {
            if (OnLockTransactions != null && tranDetailTemp != null) OnLockTransactions.Invoke(tranDetailTemp);
        }

        //Lock user detail
        private void LockUserDetail(int type)
        {
            UserDetail userDetail = new UserDetail();
            if (type == 1) userDetail.Time = _userCreatededTime;
            else userDetail.Time = DateTime.Now;
            userDetail.scriptid = _vuScriptXml.Attributes["id"].Value;
            userDetail.userid = _userid;
            userDetail.Type = type;
            userDetail.loadgenanme = ExecutionReport.GetInstance().LoadGenName;
            if (OnLockUserDetail != null && userDetail != null) OnLockUserDetail.Invoke(userDetail);
        }

        //Lock request and response if it is validation
        private void LockRequestResponse(RequestResponse data)
        {
            data.ContainerName = _containerId.Peek()[1];
            if (OnLockRequestResponse != null) OnLockRequestResponse.Invoke(data);
        }
        #endregion

        #region Paralellism
        int _requestProcessingThreadCount; 
        bool _isSequntialRequestRunning;
        object _sequentialSyncObject = new object();
        Queue<XmlNode> _sequentialRequestsQueue = new Queue<XmlNode>();
        int _maxParallelConnections = 1;

        void ProcessSequential(XmlNode request)
        {
            if (dynamicRequestsEnabled)
            {
                // When dynamic requests are enabled, then do the processing in synchronous manner
                ProcessRequest(request);
                return;
            }

            lock (_sequentialSyncObject)
            {
                _sequentialRequestsQueue.Enqueue(request);
                if (_isSequntialRequestRunning)
                {
                    // Return from here so that you can continue processing other paralell requests
                    // The current request will be processed once the current running request completes
                    return;
                }
            }

            // Wait if the threads are not available. The current thread will be blocked
            while (_requestProcessingThreadCount >= _maxParallelConnections)
            {
                if (Break == true) break;
                Thread.Sleep(10);
            }

            if (Break == true) return;

            lock (_sequentialSyncObject)
            {
                _isSequntialRequestRunning = true;
            }
            Interlocked.Increment(ref _requestProcessingThreadCount);

            new Thread(() =>
            {
                try
                {
                    // Process all the requests in the queue
                    while (true)
                    {
                        lock (_sequentialSyncObject)
                        {
                            if (_sequentialRequestsQueue.Count == 0)
                                return;

                            // Dequeue the request, if present
                            request = _sequentialRequestsQueue.Dequeue();
                        }

                        if (Break == true)
                            break;

                        ProcessRequest(request);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {
                    lock (_sequentialSyncObject)
                    {
                        _isSequntialRequestRunning = false;
                    }
                    Interlocked.Decrement(ref _requestProcessingThreadCount);
                }
            }).Start();
        }

        void ProcessParalell(XmlNode request)
        {
            // Wait if the threads are not available.
            while (_requestProcessingThreadCount >= _maxParallelConnections)
            {
                if (Break == true) 
                    break;

                Thread.Sleep(10);
            }

            if (Break == true)
                return;

            Interlocked.Increment(ref _requestProcessingThreadCount);
            new Thread(() =>
            {
                try
                {
                    ProcessRequest pr = new ProcessRequest(_maxUser, _reportName, _type, _userid, _iterationid, _vuScriptXml, _browserCache, _IPAddress, _exVariablesValues, receivedCookies, OnLockError, VUserStatus, OnLockReportData, IsValidation, _pageId, _containerId, _bandwidthInKbps);
                    pr.ProcessParallelRequest(request);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {
                    Interlocked.Decrement(ref _requestProcessingThreadCount);
                }
            }).Start();
        }
        #endregion
    }

    //To store evaluated exp info
    public class EvalutionResult
    {
        public bool isSuccess = true;
        public string value = string.Empty;
        public string errorMsg = string.Empty;
    }

    //To store vuser detail
    public class VUserStatus
    {
        public int TwoHundredStatusCodeCount { get; set; }
        public int ThreeHundredStatusCodeCount { get; set; }
        public int FourHundredStatusCodeCount { get; set; }
        public int FiveHundredStatusCodeCount { get; set; }
        public int ErrorCount { get; set; }
        public int CurrentIteration { get; set; }
    }
}