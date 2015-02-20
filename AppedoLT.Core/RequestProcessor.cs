using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;


namespace AppedoLT.Core
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

    public class RequestProcessor : IRequestProcessor
    {

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

        private Connection _serverConnection = null;
        private int _contentLength = 0;
        private int _bufferSize = 2097152;
        private byte[] _buffer;
        private int _bytesRead = 0;
        private string _host = string.Empty;
        private int _port = -1;
        private Stream _browserStream = null;
        private TcpClient browserTcpClient = null;
        ConnectionManager connectionManager = null;
        Label _label = null;

        //For Recoding
        public RequestProcessor(TcpClient browserTcpClient1, ConnectionManager connectionManager1, string requestid, string containerName, Label _label1)
        {
            Requestid = requestid;
            browserTcpClient = browserTcpClient1;
            connectionManager = connectionManager1;
            _label = _label1;
            ContainerName = containerName;
        }

        public void Process()
        {
            try
            {
                ResponseHeader = string.Empty;
                RequestBody = new MemoryStream();
                ResponseBody = new MemoryStream();
                _buffer = new byte[_bufferSize];

                _browserStream = browserTcpClient.GetStream();
                RequestHeader = ReceiveRequestHeader();

                SetUrl();

                _contentLength = GetContentLength(RequestHeader);
                if (_contentLength > 0)
                {
                    ReceiveRequestBody(browserTcpClient, _contentLength, RequestBody);
                }
                _label.Text = url.ToString();


                if (url != null)
                {
                    Connection connection = null;

                    try
                    {
                        StartTime = DateTime.Now;
                        lock (connectionManager)
                        {
                            connection = connectionManager.GetConnection(url.Host, url.Port);
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

                browserTcpClient.Close();
            }
        }

        public void ServerProcess()
        {
            lock (_serverConnection)
            {
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
                ReceiveResponseBody(_serverConnection.client, _serverConnection.NetworkStream, _contentLength, ResponseBody);
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
        private string ReceiveRequestHeader()
        {
            string requestHeader = ReceiveHeader(_browserStream);
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
                sslStream.AuthenticateAsServer(Constants.GetInstance().Certificate, false, SslProtocols.Tls, true);
                _browserStream = sslStream;
                requestHeader = ReceiveHeader(_browserStream);
            }
            return requestHeader;
        }
        void ReceiveRequestBody(TcpClient _browserTcpClient, int contentLength, Stream requestBody)
        {
            if (contentLength > 0)
                _buffer = new Byte[contentLength];
            else
                _buffer = new Byte[_bufferSize];

            if (contentLength != 0)
            {
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
                while (_browserTcpClient.Available > 0 && (_bytesRead = _browserStream.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    requestBody.Write(_buffer, 0, _bytesRead);
                    WaitUntilByteReceive(_browserTcpClient, 1000);
                }
            }
        }
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
        private void SendRequest(Stream _server)
        {
            Match firstLine = new Regex("(.*?) (.*?) (.*?)\r\n").Match(RequestHeader);
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
        private string ReceiveResponseHeader(Stream _server)
        {
            return ReceiveHeader(_server);
        }
        private void ReceiveResponseBody(TcpClient _ServerTcpClient, Stream _server, int contentLength, Stream responseBody)
        {
            Match firstLine = new Regex("(.*?) (.*?) (.*?)\r\n").Match(ResponseHeader);
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
        private void WriteToBrowser(byte[] buffer,int offset,int count)
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
