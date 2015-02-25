using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Configuration;
using System.Security.Authentication;
using System.Text.RegularExpressions;


namespace AppedoLT.Core
{
    public class Connection
    {
        public TcpClient client;
        private Stream _networkStream;
        private string _host;
        private int _port;
        private bool _isHold = false;

        public Stream NetworkStream
        {
            get
            {
                if (client.Connected == false)
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
        public Connection(string host, int port)
        {
            client = new TcpClient();
            _host = host;
            _port = port;
            Connect();
        }
        public void Close()
        {
            if (client.Connected == true)
            {
                client.Close();
            }
        }
        private void Connect()
        {
            if (client.Connected == false)
            {
                client = new TcpClient();
                client.Connect(_host, _port);
                client.ReceiveBufferSize = 8192;
                client.SendBufferSize = 8192;
            }
            if (_port == 443)
            {
                X509Certificate2 _certificate = new X509Certificate2(Constants.GetInstance().CertificatePath, "pass@12345");
                X509CertificateCollection cCollection = new X509CertificateCollection();
                cCollection.Add(_certificate);
                _networkStream = new SslStream(client.GetStream(),false,new RemoteCertificateValidationCallback(ValidateServerCertificate),null);
                ((SslStream)_networkStream).AuthenticateAsClient(_host);
            }
            else
            {
                _networkStream = client.GetStream();
            }
        }
        public void Reconnect()
        {
            client = new TcpClient();
            client.Connect(_host, _port);
            client.ReceiveBufferSize = 8192;
            client.SendBufferSize = 8192;
            if (_port == 443)
            {
                X509Certificate2 _certificate = new X509Certificate2(ConfigurationManager.AppSettings["CertificateFile"], "pass@12345");
                X509CertificateCollection cCollection = new X509CertificateCollection();
                cCollection.Add(_certificate);
                _networkStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                ((SslStream)_networkStream).AuthenticateAsClient(_host);//, cCollection, SslProtocols.Default, false);
            }
            else
            {
                _networkStream = client.GetStream();
            }
        }
        public string ReceiveHeader()
        {
            StringBuilder header = new StringBuilder();

            byte[] bytes = new byte[10];
            //Stream strem = new NetworkStream();

            while (_networkStream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.ASCII.GetString(bytes, 0, 1));
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n\r\n"))
                    break;
            }
            //MatchCollection matches = new Regex("[^\r\n]+").Matches(header.TrimEnd('\r', '\n'));
            //for (int n = 1; n < matches.Count; n++)
            //{
            //    string[] strItem = matches[n].Value.Split(new char[] { ':' }, 2);
            //    if (strItem.Length > 0)
            //        Headers[strItem[0].Trim()] = strItem[1].Trim();
            //}
            return header.ToString();
            //// check if the page should be transfered to another location
            //if (matches.Count > 0 && (
            //    matches[0].Value.IndexOf(" 302 ") != -1 ||
            //    matches[0].Value.IndexOf(" 301 ") != -1))
            //    // check if the new location is sent in the "location" header
            //    if (Headers["Location"] != null)
            //    {
            //        try { ResponseUri = new Uri(Headers["Location"]); }
            //        catch { ResponseUri = new Uri(ResponseUri, Headers["Location"]); }
            //    }
            //ContentType = Headers["Content-Type"];
            //if (Headers["Content-Length"] != null)
            //    ContentLength = int.Parse(Headers["Content-Length"]);
            //KeepAlive = (Headers["Connection"] != null && Headers["Connection"].ToLower() == "keep-alive") ||
            //            (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"].ToLower() == "keep-alive");
        }
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate,X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Accept all certificates
            return true;
        }

    }
}
