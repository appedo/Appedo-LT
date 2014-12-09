using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace AppedoLT
{
    public partial class frmTCPIPRecord : Telerik.WinControls.UI.RadForm
    {
        private TcpClient _tcpclnt =null;
        private bool _isConnected = false;
        private Stream _stream = null;
        private StringBuilder _response = null;
        private XmlNode _scriptNode;
        private XmlNode _containerNode;
        private string _lastCreatedContainer;
        private Design _frm;
        private RepositoryXml _repositoryXml = RepositoryXml.GetInstance();

        public frmTCPIPRecord(Design frm,  XmlNode scriptNode)
        {
            InitializeComponent();
            _frm = frm;
            
            _scriptNode = scriptNode;
            _lastCreatedContainer = txtContainerName.Text;
            _containerNode = CreateNewContainer(_lastCreatedContainer);
            _scriptNode.AppendChild(_containerNode);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isConnected == false)
                {
                    _tcpclnt = new TcpClient();
                    _tcpclnt.Connect(txtServerIP.Text,Convert.ToInt16(txtPort.Text));
                    txtServerIP.ReadOnly = true;
                    txtPort.ReadOnly = true;
                    btnConnect.Text = "&Disconnect";
                    btnConnect.BackColor = Color.Green;
                    _isConnected = true;
                    btnSend.Enabled = true;
                    lblMessage.Text = "Connected";
                    _stream = _tcpclnt.GetStream();
                }
                else
                {
                    btnSend.Enabled = false;
                    _tcpclnt.Close();
                    txtServerIP.ReadOnly = false;
                    txtPort.ReadOnly = false;
                    btnConnect.Text = "&Connect";
                    btnConnect.BackColor = Color.Red;
                    _isConnected = false;
                    lblMessage.Text = "Disconnected";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text =txtResponse.Text = string.Empty;
                if (_isConnected == true)
                {
                    _response = new StringBuilder();
                    Encoding asen = Encoding.Default;
                    byte[] ba = asen.GetBytes(txtRequest.Text);
                    byte[] bb = new byte[1];
                    int responseSize;
                    Stopwatch responseTime = new Stopwatch();
                    responseTime.Start();

                    _tcpclnt.Client.Send(ba, ba.Length, SocketFlags.None);

                    _tcpclnt.Client.ReceiveTimeout = 120000;
                    while ((responseSize=_tcpclnt.Client.Receive(bb)) != 0)
                    {
                        for (int index = 0; index < responseSize; index++)
                            _response.Append(Convert.ToChar(bb[index]));
                        if (_tcpclnt.Available == 0) break;
                    }
                    responseTime.Stop();
                    Thread.Sleep(100);
                    while (_tcpclnt.Available != 0 && (responseSize = _tcpclnt.Client.Receive(bb)) != 0)
                    {
                        responseTime.Start();
                        for (int index = 0; index < responseSize; index++)
                            _response.Append(Convert.ToChar(bb[index]));
                        if (_tcpclnt.Available == 0) break;
                    }
                   if( responseTime.IsRunning==true) responseTime.Stop();
                    txtResponse.Text = _response.ToString();
                    if (_lastCreatedContainer != txtContainerName.Text)
                    {
                        _containerNode = CreateNewContainer(txtContainerName.Text);
                        _scriptNode.AppendChild(_containerNode);
                    }
                    _containerNode.AppendChild(CreateNewRequst(txtRequestName.Text, txtServerIP.Text, txtPort.Text, txtRequest.Text, txtResponse.Text, txtRequest.Text.Length.ToString(), txtResponse.Text.Length.ToString(), responseTime.ElapsedMilliseconds.ToString(),true,true));

                    lblMessage.Text = "Received successfully";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
           txtRequest.Text= txtResponse.Text = string.Empty;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private XmlNode CreateNewContainer(string containername)
        {
            XmlNode container = _repositoryXml.doc.CreateElement("container");
            container.Attributes.Append(_repositoryXml.GetAttribute("name", containername));
            container.Attributes.Append(_repositoryXml.GetAttribute("id", _repositoryXml.ContainerId));
            return container;
        }

        private XmlNode CreateNewRequst(string name, string serverip, string port, string requestcontent, string responsecontent, string requestsize, string responsesize, string responsetime, bool requestsizeconstant,bool responsesizeconstant)
        {
            XmlNode req = _repositoryXml.doc.CreateElement("request");
            req.Attributes.Append(_repositoryXml.GetAttribute("id", _repositoryXml.RequestId));
            req.Attributes.Append(_repositoryXml.GetAttribute("name", name));
            req.Attributes.Append(_repositoryXml.GetAttribute("serverip", serverip));
            req.Attributes.Append(_repositoryXml.GetAttribute("port", port));
            req.Attributes.Append(_repositoryXml.GetAttribute("enable", true.ToString()));
            req.Attributes.Append(_repositoryXml.GetAttribute("requestcontent", requestcontent));
            req.Attributes.Append(_repositoryXml.GetAttribute("responsecontent", responsecontent));
            req.Attributes.Append(_repositoryXml.GetAttribute("requestsize", requestsize));
            req.Attributes.Append(_repositoryXml.GetAttribute("responsesize", responsesize));
            req.Attributes.Append(_repositoryXml.GetAttribute("responsetime", responsetime));
            req.Attributes.Append(_repositoryXml.GetAttribute("requestsizeconstant", requestsizeconstant.ToString()));
            req.Attributes.Append(_repositoryXml.GetAttribute("responsesizeconstant", responsesizeconstant.ToString()));
            XmlNode param =_repositoryXml.doc.CreateElement("params");
            req.AppendChild(param);

            return req;

        }
     
        private void frmTCPIPRecord_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
           // _frm.LoadTreeItem();
            _frm.Visible = true; 
        }
    }
}