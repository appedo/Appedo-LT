using AppedoLT.Core;
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
    /// <summary>
    /// Used to record tcp transactions.
    /// 
    /// Author: Rasith
    /// </summary>
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
        Common _common = Common.GetInstance();
        
        /// <summary>
        /// To create frmTCPIPRecord object with parent form and script node
        /// </summary>
        /// <param name="frm">Parent form</param>
        /// <param name="scriptNode">To store tcp transactions</param>
        public frmTCPIPRecord(Design frm,  XmlNode scriptNode)
        {
            InitializeComponent();
            
            _frm = frm;
            _scriptNode = scriptNode;
            _lastCreatedContainer = txtContainerName.Text;
            _containerNode = CreateNewContainer(_lastCreatedContainer);
            _scriptNode.AppendChild(_containerNode);
        }

        /// <summary>
        /// If user click connect button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// If user click send button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Used to create new container node
        /// 
        /// </summary>
        /// <param name="containername">Container name</param>
        /// <returns>Container node</returns>
        private XmlNode CreateNewContainer(string containername)
        {
            XmlNode container = _scriptNode.OwnerDocument.CreateElement("container");
            container.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "name", containername));
            container.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "id", Constants.GetInstance().UniqueID));
            return container;
        }

        /// <summary>
        /// To create new request xml node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serverip"></param>
        /// <param name="port"></param>
        /// <param name="requestcontent"></param>
        /// <param name="responsecontent"></param>
        /// <param name="requestsize"></param>
        /// <param name="responsesize"></param>
        /// <param name="responsetime"></param>
        /// <param name="requestsizeconstant"></param>
        /// <param name="responsesizeconstant"></param>
        /// <returns></returns>
        private XmlNode CreateNewRequst(string name, string serverip, string port, string requestcontent, string responsecontent, string requestsize, string responsesize, string responsetime, bool requestsizeconstant,bool responsesizeconstant)
        {
            XmlNode req = _scriptNode.OwnerDocument.CreateElement("request");
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "id", Constants.GetInstance().UniqueID));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "name", name));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "serverip", serverip));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "port", port));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "enable", true.ToString()));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "requestcontent", requestcontent));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "responsecontent", responsecontent));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "requestsize", requestsize));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "responsesize", responsesize));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "responsetime", responsetime));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "requestsizeconstant", requestsizeconstant.ToString()));
            req.Attributes.Append(_common.GetAttribute(_scriptNode.OwnerDocument, "responsesizeconstant", responsesizeconstant.ToString()));
            XmlNode param = _scriptNode.OwnerDocument.CreateElement("params");
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
