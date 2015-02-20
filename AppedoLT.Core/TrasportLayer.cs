using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AppedoLT.Core
{
    public class Trasport
    {
        private int ReadBufferSize = 8192;
        private string _ipaddress = string.Empty;
        public string IPAddressStr { get { return _ipaddress; } set { _ipaddress = value; } }
        public TcpClient tcpClient;
        public bool Connected
        {
            get
            {
                if (tcpClient.Connected == true) return true;
                else return false;
            }
            private set { }
        }
        public Trasport(string ipaddress, string port)
        {
            _ipaddress = ipaddress;
            tcpClient = Connect(ipaddress, port);
            tcpClient.ReceiveTimeout = 600000;
        }
        public Trasport(string ipaddress, string port, int requesttimeout)
        {
            _ipaddress = ipaddress;
            tcpClient = Connect(ipaddress, port);
            tcpClient.ReceiveTimeout = requesttimeout;
        }
        public Trasport(TcpClient client)
        {
            _ipaddress=((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            tcpClient = client;
        }
        private  TcpClient Connect(string ipaddress, string port)
        {
            TcpClient client = new TcpClient();
            var result = client.BeginConnect(IPAddress.Parse(ipaddress), int.Parse(port), null, null);
            result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));
           
            if(client.Connected==false)
            {
                 throw new Exception("Failed to connect " + ipaddress );
            }
            client.ReceiveTimeout = 600000;
            client.SendTimeout = 600000;
            return client;
        }
        public void Close()
        {
            if (tcpClient.Connected == true) tcpClient.Close();
        }
        public  TrasportData Receive()
        {
            Stream stream = tcpClient.GetStream();
            TrasportData objTrasportData = new TrasportData();

            StringBuilder header = new StringBuilder();
            StringBuilder response = new StringBuilder();
            int readCount = 0;
            int contentLength = 0;

            header.Append(ReadHeader(stream));
            objTrasportData.Operation = new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[1].Value;
           
            foreach (Match match in (new Regex("(.*)= (.*)\r\n").Matches(header.ToString())))
            {
                objTrasportData.Header.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            contentLength = Convert.ToInt32(new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[2].Value);
            byte[] bytes = new byte[contentLength];

            while (readCount < contentLength)
            {
                readCount += stream.Read(bytes, readCount, contentLength - readCount);
               
            }
             objTrasportData.DataStream.Write(bytes,0,contentLength);
             if (objTrasportData.DataStream.Length > 0) objTrasportData.DataStream.Seek(0, SeekOrigin.Begin);
            return objTrasportData;
        }
        public TrasportData Receive(string filePath)
        {
            Stream stream = tcpClient.GetStream();
            TrasportData objTrasportData = new TrasportData();

            StringBuilder header = new StringBuilder();
            StringBuilder response = new StringBuilder();
            int readCount = 0;
            int contentLength = 0;

            header.Append(ReadHeader(stream));
            objTrasportData.Operation = new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[1].Value;

            foreach (Match match in (new Regex("(.*)= (.*)\r\n").Matches(header.ToString())))
            {
                objTrasportData.Header.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            contentLength = Convert.ToInt32(new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[2].Value);

            byte[] bytes = new byte[ReadBufferSize];

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                while (contentLength>0)
                {
                    readCount = 0;
                    if (contentLength >= ReadBufferSize)
                    {
                        readCount = stream.Read(bytes, 0, ReadBufferSize);
                        file.Write(bytes, 0, readCount);
                        contentLength = contentLength - readCount;
                    }
                    else
                    {
                        readCount = stream.Read(bytes, 0, contentLength);
                        file.Write(bytes, 0, readCount);
                        contentLength = contentLength - readCount;
                    }
                }
            }
            objTrasportData.FilePath = filePath;         
            return objTrasportData;
        }
        public TrasportData Receive(string filePath, ref long totalByte, ref long totalByteRecceived, ref bool success)
        {
            Stream stream = tcpClient.GetStream();
            TrasportData objTrasportData = new TrasportData();

            StringBuilder header = new StringBuilder();
            StringBuilder response = new StringBuilder();
            int readCount = 0;
            int contentLength = 0;

            header.Append(ReadHeader(stream));
            objTrasportData.Operation = new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[1].Value;

            foreach (Match match in (new Regex("(.*)= (.*)\r\n").Matches(header.ToString())))
            {
                objTrasportData.Header.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            contentLength = Convert.ToInt32(new Regex("(.*): ([0-9]*)").Match(header.ToString()).Groups[2].Value);
            totalByte = contentLength;

            byte[] bytes = new byte[ReadBufferSize];

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                while (contentLength > 0)
                {
                    if (success == false)
                    {
                        break;
                    }
                    readCount = 0;
                    if (contentLength >= ReadBufferSize)
                    {
                        readCount = stream.Read(bytes, 0, ReadBufferSize);
                        totalByteRecceived += readCount;
                        file.Write(bytes, 0, readCount);
                        contentLength = contentLength - readCount;
                    }
                    else
                    {
                        readCount = stream.Read(bytes, 0, contentLength);
                        totalByteRecceived += readCount;
                        file.Write(bytes, 0, readCount);
                        contentLength = contentLength - readCount;
                    }
                }
            }
            objTrasportData.FilePath = filePath;
            return objTrasportData;
        }
        public void Send(TrasportData objTrasportData, ref long totalByte, ref long totalByteUploaded, ref bool success)
        {
            Socket socket = tcpClient.Client;
            socket.Send(objTrasportData.GetHeaderBytes());

            if (objTrasportData.FilePath == string.Empty)
            {
                totalByte = objTrasportData.DataStream.Length;
                totalByteUploaded = socket.Send(objTrasportData.DataStream.ToArray());

            }
            else
            {
                byte[] buffer = new byte[8192];
                int readCount = 0;

                using (FileStream file = new FileStream(objTrasportData.FilePath, FileMode.Open, FileAccess.Read))
                {
                    totalByte = file.Length;
                    while ((readCount = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalByteUploaded += socket.Send(buffer, 0, readCount, SocketFlags.None);
                    }
                }
            }
        }
        public  void Send( TrasportData objTrasportData)
        {
            Socket socket = tcpClient.Client;
            socket.Send(objTrasportData.GetHeaderBytes());
            if (objTrasportData.FilePath == string.Empty)
            {
                socket.Send(objTrasportData.DataStream.ToArray());
            }
            else
            {
                byte[] buffer = new byte[8192];
                int readCount = 0;
                int sum = 0;
                using (FileStream file = new FileStream(objTrasportData.FilePath, FileMode.Open, FileAccess.Read))
                {
                    while ((readCount = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        socket.Send(buffer, 0, readCount,SocketFlags.None);
                        sum += readCount;
                    }
                }
            }
        }
        public void SendGZipFile(TrasportData objTrasportData)
        {
            Socket socket = tcpClient.Client;
            socket.Send(objTrasportData.GetHeaderBytes());
            if (objTrasportData.FilePath == string.Empty)
            {
                socket.Send(objTrasportData.DataStream.ToArray());
            }
            else
            {
                byte[] buffer = new byte[8192];
                int readCount = 0;
                int sum = 0;
                using (Stream file =new GZipStream (new FileStream(objTrasportData.FilePath, FileMode.Open, FileAccess.Read),CompressionMode.Compress))
                {
                    while ((readCount = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        socket.Send(buffer, 0, readCount, SocketFlags.None);
                        sum += readCount;
                    }
                }
            }
        }
        private static string ReadHeader(Stream stream)
        {
            StringBuilder header = new StringBuilder();
            byte[] bytes = new byte[10];
            StringBuilder response = new StringBuilder();

            while (stream.Read(bytes, 0, 1) > 0)
            {
                header.Append(Encoding.Default.GetString(bytes, 0, 1));
                response.Append(Encoding.Default.GetString(bytes, 0, 1));
               
                if (bytes[0] == '\n' && header.ToString().EndsWith("\r\n\r\n"))
                    break;
            }
            return header.ToString();
        }
    }
    public class TrasportData
    {
        public MemoryStream DataStream = new MemoryStream();

        public string Operation { get; set; }
        public Dictionary<string, string> Header = new Dictionary<string, string>();
        public string DataStr
        {
            get {
                if (DataStream.Length == 0) return string.Empty;
                else return ASCIIEncoding.ASCII.GetString(DataStream.ToArray());
            }

            set
            {
                if (value != null && value != string.Empty)
                {
                    DataStream.Write(ASCIIEncoding.ASCII.GetBytes(value), 0, value.Length);
                }
            }
        }
        public string FilePath = string.Empty;

        public TrasportData() { }
        public TrasportData(string operation,  Dictionary<string, string> header,string filePath)
        {
            Operation = operation;
            Header = header;
            FilePath = filePath;
        }

        public TrasportData(string operation, string data, Dictionary<string, string> header)
        {
            Operation = operation;
            DataStr = data;
            Header = header;
        }

        public byte[] GetHeaderBytes()
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine(string.Format("{0}: {1}", Operation, FilePath==string.Empty?DataStream.Length.ToString():new FileInfo(FilePath).Length.ToString()));

            if (Header != null && Header.Count > 0)
            {
                foreach (string key in Header.Keys)
                {
                    header.AppendLine(string.Format("{0}= {1}", key, Header[key]));
                }
            }
            header.AppendLine();
            return ASCIIEncoding.ASCII.GetBytes(header.ToString());
        }
        public void Save(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    DataStream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
