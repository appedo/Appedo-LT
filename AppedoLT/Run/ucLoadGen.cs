using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Threading;
using AppedoLT.Core;
using System.Xml;

namespace AppedoLT
{
    public partial class ucLoadGen : UserControl
    {
        public ucLoadGen()
        {
            InitializeComponent();
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.btnAdd, "Add Load Generator");
            ToolTip1.SetToolTip(this.btnDelete, "Delete Load Generator");
            FormTreeNodes();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeLoadGen.SelectedNode != null)
                {
                    if (treeLoadGen.SelectedNode.Level == 1)
                    {
                        Boolean IsDefaultZone = false;
                        if (treeLoadGen.SelectedNode.Text == "Default Zone")
                            IsDefaultZone = true;
                        frmNewHost objfrmNewHost = new frmNewHost();
                        if (objfrmNewHost.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                String[] hostIPs = objfrmNewHost.strHostName.Split(",".ToCharArray());
                                foreach (String strHostIP in hostIPs)
                                {
                                    // System.Net.IPHostEntry ip = System.Net.Dns.GetHostByAddress(strHostIP);
                                    RepositoryXml.GetInstance().CreateLoadgen(strHostIP, strHostIP, IsDefaultZone, true);
                                }
                                RepositoryXml.GetInstance().Save();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                            }
                            FormTreeNodes();
                        }
                    }
                    else
                        MessageBox.Show("Select Zone");
                }
                else
                    MessageBox.Show("Select Zone from Load Generators");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeLoadGen.SelectedNode != null)
                {
                    if (treeLoadGen.SelectedNode.Level == 2)
                    {
                        RepositoryXml.GetInstance().doc.SelectSingleNode("//root//loadgens").RemoveChild(RepositoryXml.GetInstance().doc.SelectSingleNode("//root//loadgens//loadgen[@hostname='" + treeLoadGen.SelectedNode.Text + "']"));
                        if (treeLoadGen.SelectedNode.Parent.Nodes.Count <= 1)
                        {
                            treeLoadGen.SelectedNode.Parent.Checked = false;
                        }
                        treeLoadGen.SelectedNode.Remove();
                        RepositoryXml.GetInstance().Save();
                      
                    }
                    else
                        MessageBox.Show("Select Host");
                }
                else
                    MessageBox.Show("Select Host from Load Generators");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        public void FormTreeNodes()
        {
            try
            {
                treeLoadGen.Nodes.Clear();


                RadTreeNode treeRootNode = new RadTreeNode();
                treeRootNode.ShowCheckBox = true;
                treeRootNode.Text = "Load Generators";
                treeRootNode.Checked = false;

                RadTreeNode nodeDefaultZone = new RadTreeNode();
                nodeDefaultZone.ShowCheckBox = true;
                nodeDefaultZone.Text = "Default Zone";
                nodeDefaultZone.Checked = false;

                RadTreeNode nodePublicZone = new RadTreeNode();
                nodePublicZone.ShowCheckBox = true;
                nodePublicZone.Text = "Public Zone";
                nodePublicZone.Checked = false;

                foreach (XmlNode objScenarioHost in RepositoryXml.GetInstance().doc.SelectSingleNode("//root//loadgens").ChildNodes)
                {
                    RadTreeNode treeNode = new RadTreeNode();

                    System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                    Image ImageRed = Image.FromFile(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\red.JPG");
                    Image ImageGreen = Image.FromFile(AppedoLT.Core.Constants.GetInstance().ExecutingAssemblyLocation + "\\green.JPG");
                    try
                    {
                        clientSocket.Connect(objScenarioHost.Attributes["ipaddress"].Value, 8888);
                        clientSocket.Client.Send(Encoding.ASCII.GetBytes("TEST: 0"+Environment.NewLine+Environment.NewLine));
                        treeNode.Image = ImageGreen;
                        clientSocket.Close();
                    }
                    catch (Exception)
                    {
                        treeNode.Image = ImageRed;
                    }
                    treeNode.Tag = objScenarioHost;
                    treeNode.Checked = Convert.ToBoolean(objScenarioHost.Attributes["ischecked"].Value);
                    treeNode.ShowCheckBox = true;
                    treeNode.Text = objScenarioHost.Attributes["hostname"].Value;
                    if (Convert.ToBoolean(objScenarioHost.Attributes["isdefaultzone"].Value))
                    {
                        nodeDefaultZone.Nodes.Add(treeNode);
                        treeRootNode.Checked = true;
                        nodeDefaultZone.Checked = true;
                        
                    }
                    else
                    {
                        nodePublicZone.Nodes.Add(treeNode);
                        treeRootNode.Checked = true;
                        nodePublicZone.Checked = true;
                    }
                }
                treeRootNode.Nodes.Add(nodeDefaultZone);
                treeRootNode.Nodes.Add(nodePublicZone);
                treeLoadGen.Nodes.Add(treeRootNode);
                treeRootNode.ExpandAll();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public List<XmlNode> GetLoadGenerators()
        {
            List<XmlNode> loadGen = new List<XmlNode>();
            foreach (RadTreeNode node in treeLoadGen.Nodes[0].Nodes[0].Nodes)
            {
                if (node.Checked == true)
                {
                    loadGen.Add((XmlNode)node.Tag);
                }
            }
            foreach (RadTreeNode node in treeLoadGen.Nodes[0].Nodes[1].Nodes)
            {
                if (node.Checked == true)
                {
                    loadGen.Add((XmlNode)node.Tag);
                }
            }
            return loadGen;
        }
        public bool IsLoadGeneratorSelected()
        {
            if (treeLoadGen.Nodes[0].Checked == true) return true;
            else return false;
        }
        private void treeLoadGen_NodeCheckedChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Level == 2)
                {
                    RepositoryXml.GetInstance().doc.SelectSingleNode("//root//loadgens//loadgen[@hostname='" + treeLoadGen.SelectedNode.Text + "']").Attributes["ischecked"].Value = e.Node.Checked.ToString();
                    RepositoryXml.GetInstance().Save();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FormTreeNodes();
        }
    }
}
