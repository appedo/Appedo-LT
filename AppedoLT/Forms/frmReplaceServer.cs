using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class frmReplaceServer : Form
    {
        public List<ReplaceHost> HostList = new List<ReplaceHost>();

        public frmReplaceServer(VuscriptXml vuscript)
        {
            InitializeComponent();
            if (vuscript.Doc.SelectSingleNode("//vuscript").Attributes["type"].Value == "http")
            {
                HostList = GetCurrentHttp(vuscript);
                SetTreeHttp(HostList);
            }
            else
            {
                lblSchema.Visible = txtSchema.Visible = false;
                HostList =GetCurrentTcp(vuscript);
                SetTreeTcp(HostList);
            }
        }

        private List<ReplaceHost> GetCurrentHttp(VuscriptXml vuscript)
        {
            List<ReplaceHost> hostlist = new List<ReplaceHost>();
            foreach (XmlNode node in vuscript.Doc.SelectNodes("//request"))
            {
                if (!hostlist.Exists(f => f.NewHost == node.Attributes["Host"].Value))
                {
                    ReplaceHost host = new ReplaceHost();
                    host.NewHost = host.CurrentHost = node.Attributes["Host"].Value;
                    host.NewPort = host.CurrentPort = node.Attributes["Port"].Value;
                    host.NewSchema = host.CurrentSchema = node.Attributes["Schema"].Value;
                    hostlist.Add(host);
                }
            }
            return hostlist;
        }
        private List<ReplaceHost> GetCurrentTcp(VuscriptXml vuscript)
        {
            List<ReplaceHost> hostlist = new List<ReplaceHost>();
            foreach (XmlNode node in vuscript.Doc.SelectNodes("//request"))
            {
                if (!hostlist.Exists(f => f.NewHost == node.Attributes["Host"].Value))
                {
                    ReplaceHost host = new ReplaceHost();
                    host.NewHost = host.CurrentHost = node.Attributes["serverip"].Value;
                    host.NewPort = host.CurrentPort = node.Attributes["port"].Value;
                  
                    hostlist.Add(host);
                }
            }
            return hostlist;
        }
        void SetTreeHttp(List<ReplaceHost> hostlist)
        {
            foreach (ReplaceHost host in hostlist)
            {
                RadTreeNode node = new RadTreeNode();
                node.Tag = host;
                node.Text =host.NewSchema+":\\\\"+ host.CurrentHost + ":" + host.CurrentPort;
                tvHostList.Nodes.Add(node);
            }
            if (tvHostList.Nodes.Count > 0) tvHostList.Nodes[0].Selected = true;
        }
        void SetTreeTcp(List<ReplaceHost> hostlist)
        {
            foreach (ReplaceHost host in hostlist)
            {
                RadTreeNode node = new RadTreeNode();
                node.Tag = host;
                node.Text = host.CurrentHost + ":" + host.CurrentPort;
                tvHostList.Nodes.Add(node);
            }
            if (tvHostList.Nodes.Count > 0) tvHostList.Nodes[0].Selected = true;
        }
        private void tvHostList_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            ReplaceHost host = (ReplaceHost)tvHostList.SelectedNode.Tag;
            txtHost.Text = host.NewHost;
            txtPort.Text = host.NewPort;
            txtSchema.Text = host.NewSchema;
            txtSchema.Tag=txtHost.Tag = txtPort.Tag = host;
        }

        private void txtPort_Validated(object sender, EventArgs e)
        {
            RadTextBox txtBox = (RadTextBox)sender;
            ReplaceHost host = (ReplaceHost)txtBox.Tag;
            if (txtBox.Name == "txtHost")
            {
                if (txtBox.Text != host.NewHost)
                {
                    host.NewHost = txtBox.Text;
                }
            }
            else if (txtBox.Name == "txtPort")
            {
                if (txtBox.Text != host.NewPort)
                {
                    host.NewPort = txtBox.Text;
                }
            }
            else if (txtBox.Name == "txtSchema")
            {
                if (txtBox.Text != host.NewSchema)
                {
                    host.NewSchema = txtBox.Text;
                }
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to replace?", "Replace", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

            }
            else
            {
                this.DialogResult = DialogResult.None;
            }
        }
    }
}