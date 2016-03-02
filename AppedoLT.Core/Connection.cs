using System.Configuration;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AppedoLT.Core
{
    /// <summary>
    /// Used to manage connection sockets.
    /// 
    /// Author: Rasith
    /// </summary>
    public class Connection
    {

        #region The private fields

        private string _host;
        private int _port;
        private bool _isHold = false;
        private TcpClient _client;
        private Stream _networkStream;

        #endregion

        #region The public property

        public TcpClient Client
        {
            get { return _client; }
            private set { }
        }

        public Stream NetworkStream
        {
            get
            {
                if (_client.Connected == false)
                {
                    Connect();
                }
                return _networkStream;
            }
            private set { }
        }

        public string Host
        {
            set { _host = value; }
            get { return _host; }
        }

        public int Port
        {
            set { _port = value; }
            get { return _port; }
        }

        public bool IsHold
        {
            set
            {
                _isHold = value;
            }
            get
            {
                return _isHold;
            }
        }

        #endregion

        #region The constructor

        public Connection(string host, int port)
        {
            //_client = new TcpClient();
            TcpClient tcpClient = new TcpClient();
            if (bool.Parse(ConfigurationManager.AppSettings["IsProxyEnabled"].ToString()))
            {
                ProxyClientFactory pcf = new ProxyClientFactory();

                //IProxyClient proxy = pcf.CreateProxyClient(ProxyType.Http, ConfigurationManager.AppSettings["ProxyHost"].ToString(), int.Parse(ConfigurationManager.AppSettings["ProxyPort"].ToString()), ConfigurationManager.AppSettings["UserName"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                IProxyClient proxy = pcf.CreateProxyClient(ProxyType.Http, ConfigurationManager.AppSettings["ProxyHost"].ToString(), int.Parse(ConfigurationManager.AppSettings["ProxyPort"].ToString()));
                tcpClient = proxy.CreateConnection(host, port);
                //_client = new TcpClient();
                _client = tcpClient;
            }
            else
            {
                _client = new TcpClient();
            }
            _host = host;
            _port = port;
            Connect();
        }

        #endregion

        #region The public methods
        //Close socket connection
        public void Close()
        {
            if (_client.Connected == true)
            {
                _client.Close();
            }
        }

        //Connect to given ip and port
        private void Connect()
        {
            //If client is  not connected
            if (_client.Connected == false)
            {
                _client = new TcpClient();
                _client.Connect(_host, _port);
                _client.ReceiveBufferSize = 8192;
                _client.SendBufferSize = 8192;
            }
            //If it is ssl transaction
            if (_port == Constants.GetInstance().RecodingHttpsPort)
            {
                X509Certificate2 _certificate = new X509Certificate2(Constants.GetInstance().CertificatePath, "pass@12345");
                X509CertificateCollection cCollection = new X509CertificateCollection();
                cCollection.Add(_certificate);
                _networkStream = new SslStream(_client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                //Send authentication to given host(server)
                ((SslStream)_networkStream).AuthenticateAsClient(_host);
            }
            else
            {
                _networkStream = _client.GetStream();
            }
        }

        //Reconnect socket
        public void Reconnect()
        {
            _client = new TcpClient();
            _client.Connect(_host, _port);
            _client.ReceiveBufferSize = 8192;
            _client.SendBufferSize = 8192;
            //If it is ssl transaction
            if (_port == Constants.GetInstance().RecodingHttpsPort)
            {
                X509Certificate2 _certificate = new X509Certificate2(ConfigurationManager.AppSettings["CertificateFile"], "pass@12345");
                X509CertificateCollection cCollection = new X509CertificateCollection();
                cCollection.Add(_certificate);
                _networkStream = new SslStream(_client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                //Send authentication to given host(server)
                ((SslStream)_networkStream).AuthenticateAsClient(_host);
            }
            else
            {
                _networkStream = _client.GetStream();
            }
        }

        //Receive http header. double newline is indicates end of header
        public string ReceiveHeader()
        {
            StringBuilder header = new StringBuilder();

            byte[] bytes = new byte[10];

            while (_networkStream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.ASCII.GetString(bytes, 0, 1));
                //double newline is indicates end of header
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n\r\n"))
                    break;
            }

            return header.ToString();
        }

        #endregion

        #region The static varialbles and methods

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion

    }
}
