/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2009 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace AppedoLT.Core
{
    /// <summary>
    /// HTTP connection proxy class.  This class implements the HTTP standard proxy protocol.
    /// <para>
    /// You can use this class to set up a connection to an HTTP proxy server.  Calling the
    /// CreateConnection() method initiates the proxy connection and returns a standard
    /// System.Net.Socks.TcpClient object that can be used as normal.  The proxy plumbing is all
    /// handled for you.
    /// </para>
    /// <code>
    /// 
    /// </code>
    /// </summary>
    ///
    /// <remarks>   Veeru, 08-02-2016. </remarks>

    public class HttpProxyClient : IProxyClient
    {
        /// <summary>   The proxy host. </summary>
        private string _proxyHost;
        /// <summary>   The proxy port. </summary>
        private int _proxyPort;
        /// <summary>   The resp code. </summary>
        private HttpResponseCodes _respCode;
        /// <summary>   The resp text. </summary>
        private string _respText;
        /// <summary>   The TCP client. </summary>
        private TcpClient _tcpClient;

        /// <summary>   The HTTP proxy default port. </summary>
        private const int HTTP_PROXY_DEFAULT_PORT = 8080;
        /// <summary>   The HTTP proxy connect command. </summary>
        private const string HTTP_PROXY_CONNECT_CMD = "CONNECT {0}:{1} HTTP/1.0\r\nHost: {0}:{1}\r\n\r\n";
        /// <summary>   50 ms. </summary>
        private const int WAIT_FOR_DATA_INTERVAL = 50;
        /// <summary>   15 seconds. </summary>
        private const int WAIT_FOR_DATA_TIMEOUT = 15000;
        /// <summary>   Name of the proxy. </summary>
        private const string PROXY_NAME = "HTTP";

        /// <summary>   Values that represent HTTP response codes. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>

        private enum HttpResponseCodes
        {
            /// <summary>   An enum constant representing the none option. </summary>
            None = 0,
            /// <summary>   An enum constant representing the continue option. </summary>
            Continue = 100,
            /// <summary>   An enum constant representing the switching protocols option. </summary>
            SwitchingProtocols = 101,
            /// <summary>   An enum constant representing the ok option. </summary>
            OK = 200,
            /// <summary>   An enum constant representing the created option. </summary>
            Created = 201,
            /// <summary>   An enum constant representing the accepted option. </summary>
            Accepted = 202,
            /// <summary>   An enum constant representing the non authoritive information option. </summary>
            NonAuthoritiveInformation = 203,
            /// <summary>   An enum constant representing the no content option. </summary>
            NoContent = 204,
            /// <summary>   An enum constant representing the reset content option. </summary>
            ResetContent = 205,
            /// <summary>   An enum constant representing the partial content option. </summary>
            PartialContent = 206,
            /// <summary>   An enum constant representing the multiple choices option. </summary>
            MultipleChoices = 300,
            /// <summary>   An enum constant representing the moved permanetly option. </summary>
            MovedPermanetly = 301,
            /// <summary>   An enum constant representing the found option. </summary>
            Found = 302,
            /// <summary>   An enum constant representing the see other option. </summary>
            SeeOther = 303,
            /// <summary>   An enum constant representing the not modified option. </summary>
            NotModified = 304,
            /// <summary>   An enum constant representing the user proxy option. </summary>
            UserProxy = 305,
            /// <summary>   An enum constant representing the temporary redirect option. </summary>
            TemporaryRedirect = 307,
            /// <summary>   An enum constant representing the bad request option. </summary>
            BadRequest = 400,
            /// <summary>   An enum constant representing the unauthorized option. </summary>
            Unauthorized = 401,
            /// <summary>   An enum constant representing the payment required option. </summary>
            PaymentRequired = 402,
            /// <summary>   An enum constant representing the forbidden option. </summary>
            Forbidden = 403,
            /// <summary>   An enum constant representing the not found option. </summary>
            NotFound = 404,
            /// <summary>   An enum constant representing the method not allowed option. </summary>
            MethodNotAllowed = 405,
            /// <summary>   An enum constant representing the not acceptable option. </summary>
            NotAcceptable = 406,
            /// <summary>   An enum constant representing the proxy authenticantion required option. </summary>
            ProxyAuthenticantionRequired = 407,
            /// <summary>   An enum constant representing the request timeout option. </summary>
            RequestTimeout = 408,
            /// <summary>   An enum constant representing the conflict option. </summary>
            Conflict = 409,
            /// <summary>   An enum constant representing the gone option. </summary>
            Gone = 410,
            /// <summary>   An enum constant representing the precondition failed option. </summary>
            PreconditionFailed = 411,
            /// <summary>   An enum constant representing the request entity too large option. </summary>
            RequestEntityTooLarge = 413,
            /// <summary>   An enum constant representing the request URI too long option. </summary>
            RequestURITooLong = 414,
            /// <summary>   An enum constant representing the unsupported media type option. </summary>
            UnsupportedMediaType = 415,
            /// <summary>   An enum constant representing the requested range not satisfied option. </summary>
            RequestedRangeNotSatisfied = 416,
            /// <summary>   An enum constant representing the expectation failed option. </summary>
            ExpectationFailed = 417,
            /// <summary>   An enum constant representing the internal server error option. </summary>
            InternalServerError = 500,
            /// <summary>   An enum constant representing the not implemented option. </summary>
            NotImplemented = 501,
            /// <summary>   An enum constant representing the bad gateway option. </summary>
            BadGateway = 502,
            /// <summary>   An enum constant representing the service unavailable option. </summary>
            ServiceUnavailable = 503,
            /// <summary>   An enum constant representing the gateway timeout option. </summary>
            GatewayTimeout = 504,
            /// <summary>   An enum constant representing the HTTP version not supported option. </summary>
            HTTPVersionNotSupported = 505
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Rasith, 08-02-2016. </remarks>

        public HttpProxyClient() { }

        /// <summary>
        /// Creates a HTTP proxy client object using the supplied TcpClient object connection.
        /// </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="tcpClient">    A TcpClient connection object. </param>

        public HttpProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
        }

        /// <summary>   Constructor.  The default HTTP proxy port 8080 is used. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="proxyHost">    Host name or IP address of the proxy. </param>

        public HttpProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = HTTP_PROXY_DEFAULT_PORT;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="proxyHost">    Host name or IP address of the proxy server. </param>
        /// <param name="proxyPort">    Port number for the proxy server. </param>

        public HttpProxyClient(string proxyHost, int proxyPort)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        /// <summary>   Gets or sets host name or IP address of the proxy server. </summary>
        ///
        /// <value> The proxy host. </value>

        public string ProxyHost
        {
            get { return _proxyHost; }
            set { _proxyHost = value; }
        }

        /// <summary>   Gets or sets port number for the proxy server. </summary>
        ///
        /// <value> The proxy port. </value>

        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        /// <summary>   Gets String representing the name of the proxy. </summary>
        ///
        /// <remarks>   This property will always return the value 'HTTP'. </remarks>
        ///
        /// <value> The name of the proxy. </value>

        public string ProxyName
        {
            get { return PROXY_NAME; }
        }

        /// <summary>
        /// Gets or sets the TcpClient object. This property can be set prior to executing
        /// CreateConnection to use an existing TcpClient connection.
        /// </summary>
        ///
        /// <value> The TCP client. </value>

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        /// <summary>
        /// Creates a remote TCP connection through a proxy server to the destination host on the
        /// destination port.
        /// </summary>
        ///
        /// <remarks>
        /// This method creates a connection to the proxy server and instructs the proxy server to make a
        /// pass through connection to the specified destination host on the specified port.  
        /// </remarks>
        ///
        /// <exception cref="ProxyException">   Thrown when a Proxy error condition occurs. </exception>
        ///
        /// <param name="destinationHost">  Destination host name or IP address. </param>
        /// <param name="destinationPort">  Port number to connect to on the destination host. </param>
        ///
        /// <returns>
        /// Returns an open TcpClient object that can be used normally to communicate with the
        /// destination server.
        /// </returns>

        public TcpClient CreateConnection(string destinationHost, int destinationPort)
        {
            try
            {
                // if we have no connection, create one
                if (_tcpClient == null)
                {
                    if (String.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");

                    if (_proxyPort <= 0 || _proxyPort > 65535)
                        throw new ProxyException("ProxyPort value must be greater than zero and less than 65535");

                    //  create new tcp client object to the proxy server
                    _tcpClient = new TcpClient();

                    // attempt to open the connection
                    _tcpClient.Connect(_proxyHost, _proxyPort);
                }

                //  send connection command to proxy host for the specified destination host and port
                SendConnectionCommand(destinationHost, destinationPort);

                //connectViaHTTPProxy(destinationHost, destinationPort,_proxyHost, _proxyPort,"","");

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (SocketException ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }

        /// <summary>   Connects a via HTTP proxy. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <param name="targetHost">       Target host. </param>
        /// <param name="targetPort">       Target port. </param>
        /// <param name="httpProxyHost">    The HTTP proxy host. </param>
        /// <param name="httpProxyPort">    The HTTP proxy port. </param>
        /// <param name="proxyUserName">    Name of the proxy user. </param>
        /// <param name="proxyPassword">    The proxy password. </param>
        ///
        /// <returns>   A TcpClient. </returns>

        static TcpClient connectViaHTTPProxy(string targetHost,int targetPort, string httpProxyHost, int httpProxyPort, string proxyUserName, string proxyPassword)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = httpProxyHost,
                Port = httpProxyPort
            };

            var proxyUri = uriBuilder.Uri;

            var request = WebRequest.Create("https://" + targetHost + ":" + targetPort);

            var webProxy = new WebProxy(proxyUri);

            request.Proxy = webProxy;
            request.Method = "CONNECT";

            var credentials = new NetworkCredential(proxyUserName, proxyPassword);

            webProxy.Credentials = credentials;
            webProxy.BypassProxyOnLocal = true;

            var response = request.GetResponse();

            var responseStream = response.GetResponseStream();
            //Debug.Assert(responseStream != null);

            const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var rsType = responseStream.GetType();
            var connectionProperty = rsType.GetProperty("Connection", Flags);

            var connection = connectionProperty.GetValue(responseStream, null);
            var connectionType = connection.GetType();
            var networkStreamProperty = connectionType.GetProperty("NetworkStream", Flags);

            var networkStream = networkStreamProperty.GetValue(connection, null);
            var nsType = networkStream.GetType();
            var socketProperty = nsType.GetProperty("Socket", Flags);
            var socket = (Socket)socketProperty.GetValue(networkStream, null);

            return new TcpClient { Client = socket };
        }

        /// <summary>   Sends a connection command. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <param name="host"> The host. </param>
        /// <param name="port"> The port. </param>

        private void SendConnectionCommand(string host, int port)
        {


            NetworkStream stream = _tcpClient.GetStream(); 

            // PROXY SERVER REQUEST
            // =======================================================================
            //CONNECT starksoft.com:443 HTTP/1.0 <CR><LF>
            //HOST starksoft.com:443<CR><LF>
            //[... other HTTP header lines ending with <CR><LF> if required]>
            //<CR><LF>    // Last Empty Line

            string connectCmd = String.Format(CultureInfo.InvariantCulture, HTTP_PROXY_CONNECT_CMD, host, port.ToString(CultureInfo.InvariantCulture));
            byte[] request = ASCIIEncoding.ASCII.GetBytes(connectCmd);

            // send the connect request
            stream.Write(request, 0, request.Length);

            // wait for the proxy server to respond
            WaitForData(stream);

            // PROXY SERVER RESPONSE
            // =======================================================================
            //HTTP/1.0 200 Connection Established<CR><LF>
            //[.... other HTTP header lines ending with <CR><LF>..
            //ignore all of them]
            //<CR><LF>    // Last Empty Line

            // create an byte response array  
            byte[] response = new byte[_tcpClient.ReceiveBufferSize];
            StringBuilder sbuilder = new StringBuilder();
            int bytes = 0;
            long total = 0;

            do
            {
                bytes = stream.Read(response, 0, _tcpClient.ReceiveBufferSize);
                total += bytes;
                sbuilder.Append(System.Text.ASCIIEncoding.UTF8.GetString(response, 0, bytes));
            } while (stream.DataAvailable);

            ParseResponse(sbuilder.ToString());
            
            //  evaluate the reply code for an error condition
            if (_respCode != HttpResponseCodes.OK)
                HandleProxyCommandError(host, port);
        }

        /// <summary>   Handles the proxy command error. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ProxyException">   Thrown when a Proxy error condition occurs. </exception>
        ///
        /// <param name="host"> The host. </param>
        /// <param name="port"> The port. </param>

        private void HandleProxyCommandError(string host, int port)
        {
            string msg;

            switch (_respCode)
            {
                case HttpResponseCodes.None:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} failed to return a recognized HTTP response code.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;                   

                case HttpResponseCodes.BadGateway:
                    //HTTP/1.1 502 Proxy Error (The specified Secure Sockets Layer (SSL) port is not allowed. ISA Server is not configured to allow SSL requests from this port. Most Web browsers use port 443 for SSL requests.)
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a 502 code - Bad Gateway.  If you are connecting to a Microsoft ISA destination please refer to knowledge based article Q283284 for more information.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;

                default:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a {2} code - {3}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), ((int)_respCode).ToString(CultureInfo.InvariantCulture), _respText);
                    break;
            }

            //  throw a new application exception 
            throw new ProxyException(msg);
        }

        /// <summary>   Wait for data. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ProxyException">   Thrown when a Proxy error condition occurs. </exception>
        ///
        /// <param name="stream">   The stream. </param>

        private void WaitForData(NetworkStream stream)
        {
            int sleepTime = 0;
            while (!stream.DataAvailable)
            {
                Thread.Sleep(WAIT_FOR_DATA_INTERVAL);
                sleepTime += WAIT_FOR_DATA_INTERVAL;
                if (sleepTime > WAIT_FOR_DATA_TIMEOUT)
                    throw new ProxyException(String.Format("A timeout while waiting for the proxy server at {0} on port {1} to respond.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient) ));
            }
        }

        /// <summary>   Parse response. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <param name="response"> The response. </param>

        private void ParseResponse(string response)
        {
            string[] data = null;

            //  get rid of the LF character if it exists and then split the string on all CR
            data = response.Replace('\n', ' ').Split('\r');
            
            ParseCodeAndText(data[0]);
        }

        /// <summary>   Parse code and text. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <exception cref="ProxyException">   Thrown when a Proxy error condition occurs. </exception>
        ///
        /// <param name="line"> The line. </param>

        private void ParseCodeAndText(string line)
        {
            int begin = 0;
            int end = 0;
            string val = null;

            if (line.IndexOf("HTTP") == -1)
                throw new ProxyException(String.Format("No HTTP response received from proxy destination.  Server response: {0}.", line));

            begin = line.IndexOf(" ") + 1;
            end = line.IndexOf(" ", begin);

            val = line.Substring(begin, end - begin);
            Int32 code = 0;

            if (!Int32.TryParse(val, out code))
                throw new ProxyException(String.Format("An invalid response code was received from proxy destination.  Server response: {0}.", line));

            _respCode = (HttpResponseCodes)code;
            _respText = line.Substring(end + 1).Trim();
        }



#region "Async Methods"

        /// <summary>   The asynchronous worker. </summary>
        private BackgroundWorker _asyncWorker;
        /// <summary>   The asynchronous exception. </summary>
        private Exception _asyncException;
        /// <summary>   true if asynchronous cancelled. </summary>
        bool _asyncCancelled;

        /// <summary>   Gets a value indicating whether an asynchronous operation is running. </summary>
        ///
        /// <remarks>   Returns true if an asynchronous operation is running; otherwise, false. </remarks>
        ///
        /// <value> true if this object is busy, false if not. </value>

        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }

        /// <summary>   Gets a value indicating whether an asynchronous operation is cancelled. </summary>
        ///
        /// <remarks>   Returns true if an asynchronous operation is cancelled; otherwise, false. </remarks>
        ///
        /// <value> true if this object is asynchronous cancelled, false if not. </value>

        public bool IsAsyncCancelled
        {
            get { return _asyncCancelled; }
        }

        /// <summary>   Cancels any asychronous operation that is currently active. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>

        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCancelled = true;
                _asyncWorker.CancelAsync();
            }
        }

        /// <summary>   Creates asynchronous worker. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>

        private void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncException = null;
            _asyncWorker = null;
            _asyncCancelled = false;
            _asyncWorker = new BackgroundWorker();
        }

        /// <summary>   Event handler for CreateConnectionAsync method completed. </summary>
        public event EventHandler<CreateConnectionAsyncCompletedEventArgs> CreateConnectionAsyncCompleted;

        /// <summary>
        /// Asynchronously creates a remote TCP connection through a proxy server to the destination host
        /// on the destination port.
        /// </summary>
        ///
        /// <remarks>
        /// This method creates a connection to the proxy server and instructs the proxy server to make a
        /// pass through connection to the specified destination host on the specified port.  
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="destinationHost">  Destination host name or IP address. </param>
        /// <param name="destinationPort">  Port number to connect to on the destination host. </param>

        public void CreateConnectionAsync(string destinationHost, int destinationPort)
        {
            if (_asyncWorker != null && _asyncWorker.IsBusy)
                throw new InvalidOperationException("The HttpProxy object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            _asyncWorker.WorkerSupportsCancellation = true;
            _asyncWorker.DoWork += new DoWorkEventHandler(CreateConnectionAsync_DoWork);
            _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateConnectionAsync_RunWorkerCompleted);
            Object[] args = new Object[2];
            args[0] = destinationHost;
            args[1] = destinationPort;
            _asyncWorker.RunWorkerAsync(args);
        }

        /// <summary>   Event handler. Called by CreateConnectionAsync for do work events. </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Do work event information. </param>

        private void CreateConnectionAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = (Object[])e.Argument;
                e.Result = CreateConnection((string)args[0], (int)args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        /// <summary>
        /// Event handler. Called by CreateConnectionAsync for run worker completed events.
        /// </summary>
        ///
        /// <remarks>   Veeru, 08-02-2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Run worker completed event information. </param>

        private void CreateConnectionAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CreateConnectionAsyncCompleted != null)
                CreateConnectionAsyncCompleted(this, new CreateConnectionAsyncCompletedEventArgs(_asyncException, _asyncCancelled, (TcpClient)e.Result));
        }



#endregion

    }
}
