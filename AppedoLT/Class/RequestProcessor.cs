using AppedoLT.Core;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;


namespace AppedoLT
{
    public interface IRequestProcessor
    {
        string RequestHeader { get; set; }
        string ResponseHeader { get; set; }
        MemoryStream RequestBody { get; set; }
        MemoryStream ResponseBody { get; set; }
        Uri url { get; set; }
        int ResponseCode { get; set; }
        string Requestid { get; set; }
        string ContainerName { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
    }


    /// <summary>
    ///It is act as a proxy between the browser and server.
    ///Read the http request from the browser then send that http request to server then receive response finally send response to browser.
    ///
    /// Author: Rasith
    /// </summary>
    public class RequestProcessor : IRequestProcessor
    {
        #region The private fields

        private Connection _serverConnection = null;
        private int _contentLength = 0;
        private int _bufferSize = 2097152;
        private byte[] _buffer;
        private int _bytesRead = 0;
        private string _host = string.Empty;
        private int _port = -1;
        private Stream _browserStream = null;
        private TcpClient _browserTcpClient = null;
        private ConnectionManager _connectionManager = null;
        private Label _label = null;

        #endregion

        #region The public property

        #region IRequestProcessor Members

        public string RequestHeader
        { get; set; }

        public string ResponseHeader
        { get; set; }

        public MemoryStream RequestBody
        { get; set; }

        public MemoryStream ResponseBody
        { get; set; }

        public Uri url
        { get; set; }

        public int ResponseCode
        { get; set; }

        public string Requestid
        { get; set; }

        public string ContainerName
        { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        #endregion

        #endregion

        #region The constructor

        public RequestProcessor(TcpClient browserTcpClient1, ConnectionManager connectionManager1, string containerName, Label _label1)
        {

            _browserTcpClient = browserTcpClient1;
            _connectionManager = connectionManager1;
            _label = _label1;
            ContainerName = containerName;
        }

        #endregion

        /// <summary>
        /// Process request from browser.
        /// </summary>
        public void Process()
        {
            try
            {
                ResponseHeader = string.Empty;
                RequestBody = new MemoryStream();
                ResponseBody = new MemoryStream();
                _buffer = new byte[_bufferSize];

                _browserStream = _browserTcpClient.GetStream();
                RequestHeader = ReceiveRequestHeader();

                //To url to end user
                SetUrl();

                //Get request content length
                _contentLength = GetContentLength(RequestHeader);
                if (_contentLength > 0)
                {
                    ReceiveRequestBody(_browserTcpClient, _contentLength, RequestBody);
                }
                _label.Text = url.ToString();


                if (url != null)
                {
                    Connection connection = null;

                    try
                    {
                        StartTime = DateTime.Now;
                        lock (_connectionManager)
                        {
                            //Get server connection
                            connection = _connectionManager.GetConnection(url.Host, url.Port);
                        }
                        if (connection != null)
                        {
                            _serverConnection = connection;
                            ServerProcess();
                            connection.IsHold = false;
                            EndTime = DateTime.Now;
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (connection != null) connection.IsHold = false;
                        if (ResponseHeader == string.Empty)
                        {
                            //Send failed response to browser
                            StringBuilder test = new StringBuilder();
                            test.AppendLine("HTTP/1.0 " + 400 + "");
                            test.AppendLine("Content-Type: text/html");
                            test.AppendLine("Content-Length: " + ex.Message.Length).AppendLine();
                            ResponseHeader = test.ToString();
                            test.Append(ex.Message);
                            ResponseBody.Write(Encoding.Default.GetBytes(test.ToString()), 0, test.Length);
                            SendResponse();
                        }
                        else
                        {
                            SendResponse();
                        }
                    }
                    finally
                    {
                        if (EndTime == null)
                        {
                            EndTime = DateTime.Now;
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {

                _browserTcpClient.Close();
            }
        }

        /// <summary>
        /// Send request to server and get response from server.
        /// </summary>
        private void ServerProcess()
        {
            lock (_serverConnection)
            {
                SendRequest(_serverConnection.NetworkStream);
                ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                GetResponseCode(ResponseHeader);

                //if Response code  is 100 then read response from server. 
                if (ResponseCode == 100 || ResponseHeader.StartsWith("HTTP") == false)
                {
                    ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                    GetResponseCode(ResponseHeader);
                    if (ResponseCode == 100 || ResponseHeader.StartsWith("HTTP") == false)
                    {
                        ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                        GetResponseCode(ResponseHeader);
                    }
                }

                //If response header is empty then we try to send request again.
                if (ResponseHeader == string.Empty)
                {
                    _serverConnection.Close();
                    _serverConnection = new Connection(_serverConnection.Host, _port == -1 ? 80 : _port);
                    SendRequest(_serverConnection.NetworkStream);
                    ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                    GetResponseCode(ResponseHeader);
                    if (ResponseCode == 100 || ResponseHeader.StartsWith("HTTP") == false)
                    {
                        ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                        GetResponseCode(ResponseHeader);
                        if (ResponseCode == 100 || ResponseHeader.StartsWith("HTTP") == false)
                        {
                            ResponseHeader = ReceiveResponseHeader(_serverConnection.NetworkStream);
                            GetResponseCode(ResponseHeader);
                        }
                    }
                }

                _contentLength = GetContentLength(ResponseHeader);
                //Receive response body from server
                ReceiveResponseBody(_serverConnection.Client, _serverConnection.NetworkStream, _contentLength, ResponseBody);

                //Server wants to close connection
                if (ResponseHeader.Contains("Connection: Close"))
                {
                    _serverConnection.Close();
                }
                if (RequestHeader.Contains("Connection: keep-alive") == false)
                {
                    _serverConnection.Close();
                }
            }
        }

        #region Browser Operations

        /// <summary>
        /// Receive request header from browser stream.
        /// </summary>
        /// <returns>Http request header from browser</returns>
        private string ReceiveRequestHeader()
        {
            string requestHeader = ReceiveHeader(_browserStream);

            //If it is secure connection that is ssl
            if (requestHeader.ToUpper().StartsWith("CONNECT") == true)
            {

                MatchCollection matches = new Regex("(.*) (.*):([0-9]*) (.*)").Matches(requestHeader);
                if (matches.Count > 0)
                {
                    _host = matches[0].Groups[2].Value;
                    _port = int.Parse(matches[0].Groups[3].Value);
                }
                SslStream sslStream = new SslStream(_browserStream, true);
                #region Send Conncected Msg to Browser
                StreamWriter connectStreamWriter = new StreamWriter(_browserStream);
                connectStreamWriter.WriteLine("HTTP/1.0 200 Connection established");
                connectStreamWriter.WriteLine(String.Format("Timestamp: {0}", DateTime.Now.ToString()));
                connectStreamWriter.WriteLine();
                connectStreamWriter.Flush();
                #endregion
                //Create ssl stream for browser
                sslStream.AuthenticateAsServer(Constants.GetInstance().Certificate, false, SslProtocols.Tls, true);
                _browserStream = sslStream;
                requestHeader = ReceiveHeader(_browserStream);
            }
            return requestHeader;
        }

        /// <summary>
        /// Read request body from browser stream.
        /// </summary>
        /// <param name="_browserTcpClient">Tcp client from browser</param>
        /// <param name="contentLength">Content length</param>
        /// <param name="requestBody">To store content</param>
        private void ReceiveRequestBody(TcpClient _browserTcpClient, int contentLength, Stream requestBody)
        {
            if (contentLength > 0)
                _buffer = new Byte[contentLength];
            else
                _buffer = new Byte[_bufferSize];

            if (contentLength != 0)
            {
                //Read all content 
                while (contentLength > 0)
                {
                    _bytesRead = _browserStream.Read(_buffer, 0, _buffer.Length);
                    requestBody.Write(_buffer, 0, _bytesRead);
                    contentLength -= _bytesRead;
                }
            }
            else if (_browserTcpClient.Available > 0)
            {
                _browserStream.ReadTimeout = 1000;
                //Read all content 
                while (_browserTcpClient.Available > 0 && (_bytesRead = _browserStream.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    requestBody.Write(_buffer, 0, _bytesRead);
                    WaitUntilByteReceive(_browserTcpClient, 1000);
                }
            }
            else
            {
                _browserStream.ReadTimeout = 1000;
                WaitUntilByteReceive(_browserTcpClient, 2000);
                //Read all content 
                while (_browserTcpClient.Available > 0 && (_bytesRead = _browserStream.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    requestBody.Write(_buffer, 0, _bytesRead);
                    WaitUntilByteReceive(_browserTcpClient, 1000);
                }
            }
        }

        //Send response content to browser.
        private void SendResponse()
        {

            ResponseBody.Seek(0, SeekOrigin.Begin);
            while ((_bytesRead = ResponseBody.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                WriteToBrowser(_buffer, 0, _bytesRead);
            }
            ResponseBody.Seek(0, SeekOrigin.Begin);
        }
        #endregion

        #region Server Operations

        /// <summary>
        /// Send request to the server.
        /// </summary>
        /// <param name="_server">Server stream</param>
        private void SendRequest(Stream _server)
        {
            Match firstLine = new Regex("(.*?) (.*?) (.*?)\r\n").Match(RequestHeader);
            //Check http first line
            if (firstLine.Success == true)
            {
                string header = RequestHeader.Remove(0, firstLine.Value.Length);
                _server.Write(Encoding.Default.GetBytes(firstLine.Value), 0, firstLine.Value.Length);
                _server.Write(Encoding.Default.GetBytes(header), 0, header.Length);
            }
            else
            {
                _server.Write(Encoding.Default.GetBytes(RequestHeader), 0, RequestHeader.Length);
            }

            if (RequestBody.Length > 0)
            {
                RequestBody.Seek(0, SeekOrigin.Begin);

                while ((_bytesRead = RequestBody.Read(_buffer, 0, _buffer.Length)) > 0)
                {

                    _server.Write(_buffer, 0, _bytesRead);
                }
                RequestBody.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Receive response header from server stream.
        /// </summary>
        /// <param name="_server">Server stream</param>
        /// <returns></returns>
        private string ReceiveResponseHeader(Stream _server)
        {
            return ReceiveHeader(_server);
        }

        /// <summary>
        /// Receive response body
        /// </summary>
        /// <param name="_ServerTcpClient">Server tcp client</param>
        /// <param name="_server">Server stream</param>
        /// <param name="contentLength">content length</param>
        /// <param name="responseBody">To store content</param>
        private void ReceiveResponseBody(TcpClient _ServerTcpClient, Stream _server, int contentLength, Stream responseBody)
        {
            Match firstLine = new Regex("(.*?) (.*?) (.*?)\r\n").Match(ResponseHeader);
            //Read first line
            if (firstLine.Success == true)
            {
                string header = ResponseHeader.Remove(0, firstLine.Value.Length);
                WriteToBrowser(Encoding.Default.GetBytes(firstLine.Value), 0, firstLine.Value.Length);
                WriteToBrowser(Encoding.Default.GetBytes(header), 0, header.Length);
            }
            else
            {
                WriteToBrowser(Encoding.Default.GetBytes(ResponseHeader), 0, ResponseHeader.Length);
            }

            if (contentLength > 0)
                _buffer = new Byte[contentLength];
            else
                _buffer = new Byte[_bufferSize];
            try
            {
                if (contentLength != 0)
                {
                    while (contentLength > 0)
                    {
                        _bytesRead = _server.Read(_buffer, 0, _buffer.Length);
                        responseBody.Write(_buffer, 0, _bytesRead);
                        WriteToBrowser(_buffer, 0, _bytesRead);
                        contentLength -= _bytesRead;
                    }
                }
                //If response content is chunked
                else if (ResponseHeader.ToLower().Contains("Transfer-Encoding: chunked".ToLower()) == true)
                {

                    while (true)
                    {
                        string length = ReceiveGZipHeader(_server);

                        responseBody.Write(Encoding.Default.GetBytes(length), 0, length.Length);

                        WriteToBrowser(Encoding.Default.GetBytes(length), 0, length.Length);

                        contentLength = int.Parse(length.Trim(), System.Globalization.NumberStyles.HexNumber);

                        if (contentLength == 0)
                        {
                            _bytesRead = _server.Read(_buffer, 0, 2);
                            responseBody.Write(_buffer, 0, _bytesRead);
                            WriteToBrowser(_buffer, 0, _bytesRead);
                            break;
                        }
                        while (contentLength > 0)
                        {
                            _bytesRead = _server.Read(_buffer, 0, contentLength);
                            responseBody.Write(_buffer, 0, _bytesRead);
                            WriteToBrowser(_buffer, 0, _bytesRead);
                            contentLength -= _bytesRead;
                        }
                        _bytesRead = _server.Read(_buffer, 0, 2);
                        responseBody.Write(_buffer, 0, _bytesRead);
                        WriteToBrowser(_buffer, 0, _bytesRead);
                    }
                }
                else if (_ServerTcpClient.Available > 0)
                {
                    _server.ReadTimeout = 10000;
                    while (_ServerTcpClient.Available > 0 && (_bytesRead = _server.Read(_buffer, 0, _buffer.Length)) > 0)
                    {
                        responseBody.Write(_buffer, 0, _bytesRead);
                        WriteToBrowser(_buffer, 0, _bytesRead);
                        if (_ServerTcpClient.Available == 0) WaitUntilByteReceive(_ServerTcpClient, 1000);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                _server.Flush();
            }

        }

        /// <summary>
        /// Write response to browser stream.
        /// </summary>
        /// <param name="buffer">Data</param>
        /// <param name="offset">Starting point</param>
        /// <param name="count">Length</param>
        private void WriteToBrowser(byte[] buffer, int offset, int count)
        {
            try
            {
                _browserStream.Write(buffer, offset, count);
            }
            catch
            {

            }
        }
        #endregion

        #region utility
        /// <summary>
        /// Set URL variable value from response header.
        /// </summary>
        private void SetUrl()
        {
            object obj = new object();
            lock (obj)
            {
                if (_host == string.Empty && _port == -1)
                {
                    MatchCollection matches = new Regex("(.*) (.*) (.*)\r\n").Matches(RequestHeader);
                    if (matches.Count > 0)
                    {
                        try
                        {
                            string tempUrl = matches[0].Groups[2].Value;
                            if (tempUrl.ToLower().StartsWith("http") == true)
                            {
                                url = new Uri(matches[0].Groups[2].Value);
                            }
                            else
                            {
                                MatchCollection matches1 = new Regex("Host: (.*)\r\n").Matches(RequestHeader);
                                string host = matches1[0].Groups[1].Value;
                                url = new Uri(string.Format("http://{0}{1}", host, matches[0].Groups[2].Value));
                            }
                            _port = url.Port;
                            RequestHeader = RequestHeader.Replace(RequestHeader.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0], string.Format("{0} {1} {2}", matches[0].Groups[1].Value, url.PathAndQuery, matches[0].Groups[3].Value));
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    MatchCollection matches = new Regex("(.*) (.*) (.*)\r\n").Matches(RequestHeader);
                    if (matches.Count > 0)
                    {
                        string tempUrl = matches[0].Groups[2].Value;
                        if (tempUrl.ToLower().StartsWith("http") == true)
                        {
                            url = new Uri(string.Format("https://{0}:{1}{2}", _host, _port, matches[0].Groups[2].Value));
                        }
                        else
                        {
                            MatchCollection matches1 = new Regex("Host: (.*)\r\n").Matches(RequestHeader);
                            string host = matches1[0].Groups[1].Value;
                            if (host.Contains(":") == false)
                            {
                                url = new Uri(string.Format("https://{0}:{1}{2}", host, _port, matches[0].Groups[2].Value));
                            }
                            else
                            {
                                url = new Uri(string.Format("https://{0}{1}", host, matches[0].Groups[2].Value));
                            }
                        }
                        _port = url.Port;
                    }
                }
            }
        }

        /// <summary>
        /// Read http header from given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string ReceiveHeader(Stream stream)
        {
            StringBuilder header = new StringBuilder();
            byte[] bytes = new byte[10];
            while (stream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.Default.GetString(bytes, 0, 1));
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n\r\n"))
                    break;
            }
            return header.ToString();
        }

        /// <summary>
        ///  Read http header from given GZip stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Parse content length from http header.
        /// </summary>
        /// <param name="header">http header</param>
        /// <returns>Content length</returns>
        private int GetContentLength(string header)
        {
            int contentLen = 0;
            MatchCollection matches = new Regex("Content-Length: (.*)\r\n").Matches(header);
            if (matches.Count > 0)
            {
                int.TryParse(matches[0].Groups[1].Value, out contentLen);
            }
            return contentLen;
        }

        /// <summary>
        /// Parse response code from http header.
        /// </summary>
        /// <param name="header">http header</param>
        /// <returns>Content length</returns>
        private int GetResponseCode(string header)
        {
            MatchCollection matches = new Regex("HTTP/[0-9][.][0-9] ([0-9][0-9][0-9])").Matches(header);
            if (matches.Count > 0)
            {
                int temp = 0;
                int.TryParse(matches[0].Groups[1].Value, out temp);
                ResponseCode = temp;
            }
            return ResponseCode;
        }

        /// <summary>
        /// Waiting method until byte receive in given tcp client.
        /// </summary>
        /// <param name="client">Tcp client</param>
        /// <param name="milisecond">waiting mili second</param>
        private void WaitUntilByteReceive(TcpClient client, int milisecond)
        {
            while (client.Available <= 0)
            {
                if (milisecond == 0) break;
                Thread.Sleep(100);
                milisecond -= 100;
            }
        }
        #endregion
    }
}
