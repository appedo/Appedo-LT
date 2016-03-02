using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Configuration;

namespace AppedoLT.BusinessLogic
{
    public class HttpRequest : Request
    {

        #region The private fields

        private Dictionary<string, string> _cookiesBuffer
        {
            get;
            set;
        }
        private byte[] _buffer;
        private string _headerCookie
        {
            get
            {
                StringBuilder cookie = new StringBuilder();
                foreach (KeyValuePair<string, string> param in _cookiesBuffer)
                {
                    cookie.Append(param.Value.ToString()).Append(";");
                }
                if (cookie.Length > 0) cookie.Remove(cookie.Length - 1, 1);
                return cookie.ToString();
            }

        }
        private List<PostData> _posDataContainer
        {
            get;
            set;
        }
        private string _connectionGroup;
        private IPEndPoint _IPAdress = null;
        private HttpWebRequest _request;

        #endregion

        #region The constructor

        public HttpRequest(XmlNode request, ref Dictionary<string, string> cookies, string ConnectionGroup, IPEndPoint ipaddress, bool storeResult)
        {
            RequestId = int.Parse(request.Attributes["id"].Value);
            HasError = false;
            RequestNode = request;
            responseTime = new Stopwatch();
            ResponseStream = new MemoryStream();
            StoreRequestBody = storeResult;

            _connectionGroup = ConnectionGroup;
            _IPAdress = ipaddress;
            _cookiesBuffer = cookies;
        }
       
        public HttpRequest(XmlNode parentRequest, string secondaryRequest, Dictionary<string, string> cookies, string ConnectionGroup, IPEndPoint ipaddress, bool storeResult)
        {
            RequestId = 0;
            HasError = false;
            StoreRequestBody = storeResult;
            _cookiesBuffer = cookies;
            RequestNode = CreateRequestNode(parentRequest, secondaryRequest);
            responseTime = new Stopwatch();
            ResponseStream = new MemoryStream();
            _connectionGroup = ConnectionGroup;
            _IPAdress = ipaddress;
        }
      
        #endregion

        #region The destructor

        ~HttpRequest()
        {
            _posDataContainer = null;
            _connectionGroup = null;
        }

        #endregion

        #region The public methods

        public override void GetResponse()
        {
            responseTime.Start();
            StartTime = DateTime.Now;
            _request = null;
            try
            {
                #region Bind QueryStringParam

                if (RequestNode.SelectSingleNode("querystringparams") != null && RequestNode.SelectSingleNode("querystringparams").HasChildNodes)
                {
                    RequestNode.Attributes["Address"].Value += "?" + GetQueryString(RequestNode.SelectSingleNode("querystringparams"));
                }

                #endregion

                #region Create Request
                _request = (HttpWebRequest)WebRequest.Create(RequestNode.Attributes["Address"].Value);
                _request.Timeout = RequestTimeOut;
                _request.ConnectionGroupName = _connectionGroup;

                _request.ServicePoint.BindIPEndPointDelegate += new BindIPEndPoint((ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) =>
                {
                    if (IPAddress.IsLoopback(remoteEndPoint.Address))
                    {
                        if (remoteEndPoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            return new IPEndPoint(IPAddress.Parse("::1"), 0);
                        }
                        else
                        {
                            return new IPEndPoint(IPAddress.Loopback, 0);
                        }
                    }
                    else
                    {
                        return _IPAdress;
                    }

                    //if (Request.IPSpoofingEnabled == true)
                    //{
                    //    if (IPAddress.IsLoopback(remoteEndPoint.Address))
                    //    {
                    //        if (remoteEndPoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    //        {
                    //            return new IPEndPoint(IPAddress.Parse("::1"), 0);
                    //        }
                    //        else
                    //        {
                    //            return new IPEndPoint(IPAddress.Loopback, 0);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        return _IPAdress;
                    //    }
                    //}
                    //else
                    //{
                    //    return new IPEndPoint(IPAddress.Any, 0);
                    //}

                });

                //_request.Proxy = null;
                _request.Expect = null;
                _request.KeepAlive = true;
                _request.AllowAutoRedirect = false;
                _request.Connection = "keepalive";
                _request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                _request.ProtocolVersion = HttpVersion.Version11;
                _request.Method = RequestNode.Attributes["Method"].Value;
                _request.UnsafeAuthenticatedConnectionSharing = true;

                if (bool.Parse(ConfigurationManager.AppSettings["IsProxyEnabled"].ToString()))
                {
                    IWebProxy proxy = _request.Proxy;
                    WebProxy myProxy = new WebProxy();
                    // Print the Proxy Url to the console.
                    if (proxy != null)
                    {

                        // Create a new Uri object.
                        Uri newUri = new Uri("http://" + ConfigurationManager.AppSettings["ProxyHost"].ToString() + ":" + ConfigurationManager.AppSettings["ProxyPort"].ToString());
                        // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                        myProxy.Address = newUri;
                    }

                    _request.Proxy = myProxy;
                }
                else
                {
                    _request.Proxy = null;
                }

                #endregion

                #region Header
                _request.Headers.Add("Cookie", this._headerCookie);
                foreach (XmlNode hed in RequestNode.SelectSingleNode("headers").ChildNodes)
                {

                    if (_Constants.HeaderExcludeList.Contains(hed.Attributes["name"].Value) != true)
                    {
                        try
                        {
                            _request.Headers.Add(hed.Attributes["name"].Value, hed.Attributes["value"].Value);
                        }
                        catch (ArgumentException ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                            _Constants.HeaderExcludeList.Add(hed.Attributes["name"].Value);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        switch (hed.Attributes["name"].Value)
                        {
                            case "Accept":
                                _request.Accept = hed.Attributes["value"].Value;
                                break;
                            case "Referer":
                                _request.Referer = hed.Attributes["value"].Value;
                                break;
                            case "Content-Type":
                                _request.ContentType = hed.Attributes["value"].Value;
                                break;
                            case "User-Agent":
                                _request.UserAgent = hed.Attributes["value"].Value;
                                break;
                            case "If-Modified-Since":
                                try
                                {
                                    _request.IfModifiedSince = Convert.ToDateTime(hed.Attributes["value"].Value);
                                }
                                catch
                                {
                                }
                                break;
                        }
                    }


                }
                #endregion

                #region BindPostData
                if (_request.Method == "POST")
                {
                    _posDataContainer = GetPostData(RequestNode.SelectSingleNode("params"));
                    foreach (PostData pData in _posDataContainer.FindAll(f => f.size > 0))
                    {
                        if (_request.ContentLength < 0)
                        {
                            _request.ContentLength = pData.size;
                        }
                        else
                        {
                            _request.ContentLength += pData.size;
                        }
                    }

                    using (var dataStream = _request.GetRequestStream())
                    {

                        foreach (PostData pData in _posDataContainer.FindAll(f => f.size > 0))
                        {
                            if (pData.type == 1)
                            {
                                dataStream.Write(Encoding.ASCII.GetBytes(pData.value.ToString().ToCharArray(), 0, pData.value.Length), 0, pData.value.Length);
                            }
                            else if (pData.type == 2)
                            {
                                byte[] buff = new byte[1028];
                                int readSize = 0;
                                using (FileStream stream = new FileStream(pData.value.ToString(), FileMode.Open, FileAccess.Read))
                                {
                                    while (pData.size > 0)
                                    {
                                        readSize = stream.Read(buff, 0, buff.Length);
                                        try
                                        {
                                            dataStream.Write(buff, 0, readSize);
                                        }
                                        catch
                                        {

                                        }
                                        pData.size = pData.size - readSize;
                                    }
                                }
                            }
                        }
                        dataStream.Close();
                    }
                }
                #endregion

                #region GetResponse
                StringBuilder result = new StringBuilder();
                try
                {
                    #region GetVaildResponse
                    using (var httpWebResponse = _request.GetResponse() as HttpWebResponse)
                    {
                        if (httpWebResponse != null)
                        {
                            ResponseCode = Convert.ToInt16(((HttpStatusCode)httpWebResponse.StatusCode).ToString("d"));

                            result.Append("http/" + httpWebResponse.ProtocolVersion).Append(" ").Append(ResponseCode.ToString()).Append(" ").Append(httpWebResponse.StatusCode.ToString()).Append(Environment.NewLine);
                            foreach (string key in httpWebResponse.Headers.AllKeys)
                            {
                                result.Append(key).Append(": ").Append(httpWebResponse.Headers[key]).Append(Environment.NewLine);
                            }
                            result.Append(Environment.NewLine);
                            ResponseStream.Write(Encoding.ASCII.GetBytes(result.ToString()), 0, result.Length);
                            ReadResponseBody(result.ToString(), httpWebResponse.ContentLength, httpWebResponse.GetResponseStream(), ref ResponseStream);

                            #region StoreCookies
                            if (httpWebResponse.Headers["Set-Cookie"] != null)
                            {
                                lock (_cookiesBuffer)
                                {
                                    SetCookies(httpWebResponse.Headers["Set-Cookie"]);
                                }
                            }
                            #endregion

                            httpWebResponse.Close();
                        }
                    }
                    #endregion
                    Success = true;
                }
                catch (WebException webEx)
                {

                    Success = false;
                    HasError = true;
                    if (webEx.Response != null)
                    {
                        ResponseCode = Convert.ToInt16(((HttpWebResponse)webEx.Response).StatusCode.ToString("d"));
                        ErrorCode = ((HttpWebResponse)webEx.Response).StatusCode.ToString("d");
                    }
                    #region GetErrorEsponse

                    if (webEx == null)
                    {
                        result.Append("http/" + _request.ProtocolVersion).Append(" ").Append(webEx.Status.ToString("d")).Append(" ").Append(webEx.Status.ToString()).Append(Environment.NewLine);
                    }
                    else if (webEx.Response != null)
                    {
                        result.Append("http/" + _request.ProtocolVersion).Append(" ").Append(ResponseCode.ToString()).Append(" ").Append(((HttpWebResponse)webEx.Response).StatusCode.ToString()).Append(Environment.NewLine);
                    }
                    else
                    {
                        result.Append("http/" + _request.ProtocolVersion).Append(" ").Append(ResponseCode.ToString()).Append(" ").Append(webEx.Status.ToString()).Append(Environment.NewLine);
                    }

                    if (webEx.Response != null)
                    {
                        foreach (string key in webEx.Response.Headers.AllKeys)
                        {
                            result.Append(key).Append(": ").Append(webEx.Response.Headers[key]).Append(Environment.NewLine);
                        }
                        result.Append(Environment.NewLine);
                        #region StoreCookies

                        if (webEx.Response.Headers["Set-Cookie"] != null)
                        {
                            lock (_cookiesBuffer)
                            {
                                SetCookies(webEx.Response.Headers["Set-Cookie"]);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        result.Append(webEx.Message);
                    }

                    ResponseStream.Write(Encoding.ASCII.GetBytes(result.ToString()), 0, result.Length);
                    if (webEx.Response != null) ReadResponseBody(webEx.Response.ContentType, webEx.Response.ContentLength, webEx.Response.GetResponseStream(), ref ResponseStream);
                    responseTime.Stop();
                    ErrorMessage = webEx.Message;

                    #endregion

                }
                finally
                {
                    responseTime.Stop();
                    PerformAssertion();
                }

                #endregion
            }
            catch (Exception ex)
            {
                Success = false;
                HasError = true;
                ErrorMessage = ex.Message;
                ErrorCode = "700";
            }
            finally
            {
                if (_request != null)
                {
                    _request.ServicePoint.BindIPEndPointDelegate = null;
                    _request = null;
                }
            }
        }

        public override void PerformAssertion()
        {
            Hashtable hashtable = new Hashtable();
            #region Assertion

            if (RequestNode.SelectSingleNode("assertions") != null && RequestNode.SelectSingleNode("assertions").ChildNodes.Count > 0)
            {
                foreach (XmlNode assertion in RequestNode.SelectNodes("assertions/assertion"))
                {
                    if (assertion.Attributes["type"].Value == "text")
                    {
                        #region Text
                        if (assertion.Attributes["condition"].Value == "Response contain")
                        {
                            if (ResponseStr.Contains(assertion.Attributes["text"].Value) == true)
                            {
                                AssertionResult = true;
                                AssertionFaildMsg.Append(string.Format("Expected value({0}) present in the response.\r\n", assertion.Attributes["text"].Value));
                            }
                            else
                            {
                                AssertionResult = false;
                                // AssertionFaildMsg.Append(string.Format("Assertion({0}) Failed.\r\n", assertion.Attributes["name"].Value));
                                AssertionFaildMsg.Append(string.Format("Expected value({0}) not present in the response.\r\n", assertion.Attributes["text"].Value));
                            }
                        }
                        else
                        {
                            if (!(ResponseStr.Contains(assertion.Attributes["text"].Value) == true))
                            {
                                AssertionResult = true;
                                AssertionFaildMsg.Append(string.Format("Expected value({0}) present in the response.\r\n", assertion.Attributes["text"].Value));
                            }
                            else
                            {
                                AssertionResult = false;
                                AssertionFaildMsg.Append(string.Format("Assertion({0}) Failed.\r\n", assertion.Attributes["name"].Value));
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Reqex
                        int matchCount = 0;
                        try
                        {
                            matchCount = new Regex(assertion.Attributes["text"].Value).Matches(ResponseStr).Count;
                        }
                        catch (Exception)
                        {

                        }
                        if (assertion.Attributes["condition"].Value == "Response contain")
                        {
                            if (matchCount > 0)
                            {
                                AssertionResult = true;
                                AssertionFaildMsg.Append(string.Format("Expected value({0}) present in the response.\r\n", assertion.Attributes["text"].Value));
                            }
                            else
                            {
                                AssertionResult = false;
                                AssertionFaildMsg.Append(string.Format("Assertion({0}) Faild.\r\n", assertion.Attributes["name"].Value));
                            }
                        }
                        else
                        {
                            if (!(matchCount > 0))
                            {
                                AssertionResult = true;
                                AssertionFaildMsg.Append(string.Format("Expected value({0}) present in the response.\r\n", assertion.Attributes["text"].Value));
                            }
                            else
                            {
                                AssertionResult = false;
                                AssertionFaildMsg.Append(string.Format("Assertion({0}) Faild.\r\n", assertion.Attributes["name"].Value));
                            }
                        }
                        #endregion
                    }

                    hashtable.Add(assertion.Attributes["name"].Value, AssertionResult);
                }
                if (hashtable.ContainsValue(false))
                {
                    Success = false;
                    HasError = true;
                    ErrorMessage = AssertionFaildMsg.ToString();
                    ErrorCode = "800";
                }

                //if (AssertionResult == false)
                //{
                //    Success = false;
                //    HasError = true;
                //    ErrorMessage = AssertionFaildMsg.ToString();
                //    ErrorCode = "800";
                //}


            }
            #endregion
        }

        public override void Abort()
        {
            try
            {
                if (_request != null)
                {
                    _request.Abort();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        #endregion

        #region The private methods

        private void SetCookies(string strCookHeader)
        {
            try
            {
                strCookHeader = strCookHeader.Replace("\r", "");
                strCookHeader = strCookHeader.Replace("\n", "");
                string[] strCookTemp = strCookHeader.Split(',');

                int i = 0;
                int n = strCookTemp.Length;
                string name = string.Empty;
                string value = string.Empty;
                while (i < n)
                {
                    if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        name = strCookTemp[i].Split('=')[0];
                        value = strCookTemp[i] + "," + strCookTemp[i + 1];
                        value = value.Split(';')[0];
                        i = i + 1;
                    }
                    else
                    {
                        name = strCookTemp[i].Split('=')[0];
                        value = strCookTemp[i];
                        value = value.Split(';')[0];
                    }
                    if (_cookiesBuffer.ContainsKey(name) == true)
                    {
                        _cookiesBuffer.Remove(name);
                    }
                    _cookiesBuffer.Add(name, value);
                    i = i + 1;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private string GetQueryString(XmlNode queryString)
        {
            StringBuilder result = new StringBuilder();
            foreach (XmlNode parm in queryString.ChildNodes)
            {
                result.Append(parm.Attributes["name"].Value).Append("=").Append(System.Web.HttpUtility.UrlEncode(parm.Attributes["value"].Value)).Append("&");
                Parameters.Add(new AppedoLT.Core.Tuple<string, string>(parm.Attributes["name"].Value, parm.Attributes["value"].Value));
            }
            if (result.Length > 0) result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        private List<PostData> GetPostData(XmlNode postData)
        {
            int count = 0;
            List<PostData> postDataBuffer = new List<PostData>();

            if (postData != null && postData.HasChildNodes && (postData.Attributes["type"].Value == "form" || postData.Attributes["type"].Value == "text"))
            {
                #region PostData
                PostData pData = new PostData();
                pData.type = 1;
                foreach (XmlNode parm in postData.ChildNodes)
                {
                    Parameters.Add(new AppedoLT.Core.Tuple<string, string>(parm.Attributes["name"].Value, parm.Attributes["value"].Value));
                    count++;
                    if (count == postData.ChildNodes.Count)
                    {
                        if (postData.Attributes["type"].Value == "text")
                        {
                            pData.value.Append(parm.Attributes["value"].Value);
                        }
                        else
                        {
                            pData.value.Append(parm.Attributes["name"].Value).Append("=").Append(System.Web.HttpUtility.UrlEncode(parm.Attributes["value"].Value));
                        }
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
                    Parameters.Add(new AppedoLT.Core.Tuple<string, string>(parm.Attributes["name"].Value, parm.Attributes["value"].Value));
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

        private MemoryStream ReadResponseBody(string contentType, long contentLength, Stream responseStream, ref MemoryStream responseBody)
        {
            int _bytesRead = 0;
            bool StoreResult = false;

            if (contentLength > 0)
            {
                _buffer = new Byte[contentLength];
                ResponseSize = contentLength;
            }
            else
                _buffer = new Byte[_bufferSize];

            try
            {
                if (RequestNode.SelectSingleNode("./extractor") != null || StoreRequestBody == true)
                {
                    StoreResult = true;
                }

                if (contentLength > 0)
                {
                    #region Read Data
                    while (contentLength > 0)
                    {
                        if (contentLength >= _buffer.Length)
                        {
                            _bytesRead = responseStream.Read(_buffer, 0, _buffer.Length);
                        }
                        else
                        {
                            _bytesRead = responseStream.Read(_buffer, 0, (int)contentLength);
                        }
                        responseBody.Write(_buffer, 0, _bytesRead);
                        contentLength -= _bytesRead;
                    }
                    #endregion
                }
                else if (contentType.ToLower().Contains("Transfer-Encoding: chunked".ToLower()) == true)
                {
                    _bytesRead = responseStream.Read(_buffer, 0, _buffer.Length);
                    ResponseSize += _bytesRead;
                    if (StoreResult) responseBody.Write(_buffer, 0, _bytesRead);
                    while (_bytesRead > 0)
                    {
                        ResponseSize += _bytesRead;
                        _bytesRead = responseStream.Read(_buffer, 0, _buffer.Length);
                        if (StoreResult) responseBody.Write(_buffer, 0, _bytesRead);
                    }
                }
                else if (contentLength == 0)
                {
                    _bytesRead = responseStream.Read(_buffer, 0, _buffer.Length);
                    ResponseSize += _bytesRead;
                    if (StoreResult) responseBody.Write(_buffer, 0, _bytesRead);
                    while (_bytesRead > 0)
                    {
                        ResponseSize += _bytesRead;
                        _bytesRead = responseStream.Read(_buffer, 0, _buffer.Length);
                        if (StoreResult) responseBody.Write(_buffer, 0, _bytesRead);
                    }
                }
                responseStream.Flush();
                if (responseBody.Length > 0)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                }
                responseStream.Close();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            return responseBody;
        }

        private XmlNode CreateRequestNode(XmlNode parentRequest, string url)
        {
            

            XmlNode request = parentRequest.OwnerDocument.CreateElement("request");
            if (url.StartsWith("/") == true)
            {
                request.Attributes.Append(GetAttribute("Address", new StringBuilder().Append(parentRequest.Attributes["Address"].Value).Append(url).ToString(), request.OwnerDocument));

            }
            else if (url.ToLower().StartsWith("http") == true)
            {
                request.Attributes.Append(GetAttribute("Address", url, request.OwnerDocument));
            }
            else if (url.StartsWith("../") == true)
            {
                string strUriString = parentRequest.Attributes["Address"].Value;
                int pos = strUriString.LastIndexOf('/');
                if (pos > 0) { strUriString = strUriString.Substring(0, pos) + "/"; }

                request.Attributes.Append(GetAttribute("Address", new StringBuilder().Append(strUriString).Append(url).ToString(), request.OwnerDocument));
            }
            else if (url.StartsWith("/") == false)
            {
                url = "/" + url;
                request.Attributes.Append(GetAttribute("Address", new StringBuilder().Append(parentRequest.Attributes["Address"].Value).Append(url).ToString(), request.OwnerDocument));
            }
            
            else
            {
                request.Attributes.Append(GetAttribute("Address", new StringBuilder().Append(parentRequest.Attributes["Address"].Value).Append(url).ToString(), request.OwnerDocument));
            }

            request.Attributes.Append(GetAttribute("Path", new Uri(request.Attributes["Address"].Value).AbsolutePath, request.OwnerDocument));
            request.Attributes.Append(GetAttribute("name", new Uri(request.Attributes["Address"].Value).PathAndQuery, request.OwnerDocument));
            request.Attributes.Append(GetAttribute("Method", "GET", request.OwnerDocument));

            XmlNode headers = request.OwnerDocument.CreateElement("headers");
            request.AppendChild(headers);

            return request;
        }

        private XmlAttribute GetAttribute(string name, string value, XmlDocument doc)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        #endregion

    }

    public class PostData
    {
        public int type;
        public StringBuilder value = new StringBuilder();
        public long size;
    }
}