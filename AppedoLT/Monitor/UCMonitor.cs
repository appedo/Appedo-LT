using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class UCMonitor : UserControl
    {

        MonitorXml _monitorXml = MonitorXml.GetInstance();
        Constants _constant = Constants.GetInstance();
        List<MonitorServer> _runningMonitors = new List<MonitorServer>();
        DataTable _reports = new DataTable();

        public UCMonitor()
        {
            InitializeComponent();
           
        }

        public void LoadSetting()
        {
            try
            {
                _reports = ResultLogMonitor.GetInstance().GetReportName();
                ddlReportName.SelectedIndex = 0;
                ddlReportName.DataSource = _reports;
                tabsMonitor.ItemsOffset = (tabsMonitor.Size.Width / 2) + 20;
                tabiConfiguration.Select();
                LoadServers();
                this.LoadServersForRun();
                timer1.Enabled = true;
                timer1.Start();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void LoadServers()
        {
            try
            {
                XmlNode serversNode = _monitorXml.doc.SelectSingleNode("//servers");
                RadTreeNode servers = new RadTreeNode();
                servers.Text = "Servers";
                servers.Tag = serversNode;
                foreach (XmlNode node in serversNode.ChildNodes)
                {
                    RadTreeNode server = new RadTreeNode();
                    server.Text = node.Attributes["name"].Value;
                    server.Tag = node;
                    servers.Nodes.Add(server);
                }
                servers.Expand();
                tvServers.Nodes.Add(servers);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void tabsMonitor_Resize(object sender, EventArgs e)
        {
            double itemSize = 0;
            foreach (TabItem item in tabsMonitor.Items)
            {
                itemSize += item.Size.Width;
            }

            if (itemSize != 0) tabsMonitor.ItemsOffset = Convert.ToInt16((tabsMonitor.Size.Width / 2) - (itemSize / 2));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                XmlNode serverNode = _monitorXml.CreateServer();
                RadTreeNode server = new RadTreeNode();
                server.Text = serverNode.Attributes["name"].Value;
                server.Tag = serverNode;
                ((XmlNode)tvServers.Nodes[0].Tag).AppendChild(serverNode);
                tvServers.Nodes[0].Nodes.Add(server);
                tvServers.Nodes[0].Expand();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvServers.SelectedNode != null && tvServers.SelectedNode.Level == 1 && MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ((XmlNode)tvServers.Nodes[0].Tag).RemoveChild((XmlNode)tvServers.SelectedNode.Tag);
                    tvServers.SelectedNode.Remove();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void tvServers_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (tvServers.SelectedNode != null && tvServers.SelectedNode.Level == 1)
                {
                    splitPanel3.Controls.Clear();
                    splitPanel3.Controls.Add(UCServer.GetInstance((XmlNode)tvServers.SelectedNode.Tag, tvServers.SelectedNode));
                }
                else
                {
                    splitPanel3.Controls.Clear();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _monitorXml.Save();
                MessageBox.Show("Saved successfully");
                this.LoadServersForRun();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public void LoadServersForRun()
        {
            try
            {
                tvServersWithCounters.Nodes.Clear();
                XmlNode serversNode = _monitorXml.doc.SelectSingleNode("//servers");
                RadTreeNode servers = new RadTreeNode();
                servers.Text = "Servers";
                servers.Tag = serversNode;
                servers.Checked = true;
                foreach (XmlNode serverNode in serversNode.ChildNodes)
                {
                    RadTreeNode server = new RadTreeNode();
                    server.Text = serverNode.Attributes["name"].Value;
                    server.ShowCheckBox = true;
                    server.Checked = true;
                    server.Tag = serverNode;
                    servers.Nodes.Add(server);

                    XmlNode counterNode = serverNode.SelectSingleNode(".//counters");
                    for (int index = 0; index < counterNode.ChildNodes.Count; index++)
                    {
                        RadTreeNode cat;
                        if (server.Nodes.Count > 0 && server.Nodes[server.Nodes.Count - 1].Text == counterNode.ChildNodes[index].Attributes["category"].Value)
                        {
                            cat = server.Nodes[server.Nodes.Count - 1];
                        }
                        else
                        {
                            cat = new RadTreeNode();
                            cat.Text = counterNode.ChildNodes[index].Attributes["category"].Value;
                            cat.Checked = true;
                            cat.ShowCheckBox = true;
                            server.Nodes.Add(cat);
                        }

                        if (counterNode.ChildNodes[index].Attributes["instance"].Value != string.Empty)
                        {
                            RadTreeNode instance;
                            if (cat.Nodes.Count == 0)
                            {
                                instance = new RadTreeNode();
                                instance.Text = counterNode.ChildNodes[index].Attributes["instance"].Value;
                                instance.ShowCheckBox = true;
                                instance.Checked = true;
                                cat.Nodes.Add(instance);

                                RadTreeNode counter = new RadTreeNode();
                                counter.Text = counterNode.ChildNodes[index].Attributes["countername"].Value;
                                counter.Tag = counterNode.ChildNodes[index];
                                counter.ShowCheckBox = true;
                                counter.Checked = true;
                                instance.Nodes.Add(counter);
                            }
                            else
                            {
                                if (cat.Nodes[cat.Nodes.Count - 1].Text == counterNode.ChildNodes[index].Attributes["instance"].Value)
                                {
                                    instance = cat.Nodes[cat.Nodes.Count - 1];
                                }
                                else
                                {
                                    instance = new RadTreeNode();
                                    instance.Text = counterNode.ChildNodes[index].Attributes["instance"].Value;
                                    instance.ShowCheckBox = true;
                                    instance.Checked = true;
                                    cat.Nodes.Add(instance);
                                }
                                RadTreeNode counter = new RadTreeNode();
                                counter.Text = counterNode.ChildNodes[index].Attributes["countername"].Value;
                                counter.Tag = counterNode.ChildNodes[index];
                                counter.ShowCheckBox = true;
                                counter.Checked = true;
                                instance.Nodes.Add(counter);
                            }
                        }
                        else
                        {
                            RadTreeNode counter = new RadTreeNode();
                            counter.Text = counterNode.ChildNodes[index].Attributes["countername"].Value;
                            counter.Tag = counterNode.ChildNodes[index];
                            counter.ShowCheckBox = true;
                            counter.Checked = true;
                            cat.Nodes.Add(counter);
                        }
                    }
                }
                tvServersWithCounters.Nodes.Add(servers);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public void LoadServersForMonitor(string reportName)
        {
            try
            {
                tvChart.Nodes.Clear();

                XmlDocument doc = new XmlDocument();
                doc.Load(_constant.DataFolderPathMonitor + "\\" + reportName + "\\monitor.xml");
                XmlNode serversNode = doc.SelectSingleNode("//monitor");
                RadTreeNode servers = new RadTreeNode();
                servers.Text = "Servers";
                servers.Tag = serversNode;
                servers.ShowCheckBox = false;
                foreach (XmlNode serverNode in serversNode.ChildNodes)
                {
                    RadTreeNode server = new RadTreeNode();
                    server.Text = serverNode.Attributes["name"].Value;
                    server.Tag = serverNode;
                    servers.Nodes.Add(server);

                    for (int index = 0; index < serverNode.ChildNodes.Count; index++)
                    {
                        RadTreeNode cat;
                        if (server.Nodes.Count > 0 && server.Nodes[server.Nodes.Count - 1].Text == serverNode.ChildNodes[index].Attributes["category"].Value)
                        {
                            cat = server.Nodes[server.Nodes.Count - 1];
                        }
                        else
                        {
                            cat = new RadTreeNode();
                            cat.Text = serverNode.ChildNodes[index].Attributes["category"].Value;
                            server.Nodes.Add(cat);
                        }

                        if (serverNode.ChildNodes[index].Attributes["instance"].Value != string.Empty)
                        {
                            RadTreeNode instance;
                            if (cat.Nodes.Count == 0)
                            {
                                instance = new RadTreeNode();
                                instance.Text = serverNode.ChildNodes[index].Attributes["instance"].Value;
                                cat.Nodes.Add(instance);

                                RadTreeNode counter = new RadTreeNode();
                                counter.Text = serverNode.ChildNodes[index].Attributes["countername"].Value;
                                counter.Tag = serverNode.ChildNodes[index];
                                instance.Nodes.Add(counter);
                            }
                            else
                            {
                                if (cat.Nodes[cat.Nodes.Count - 1].Text == serverNode.ChildNodes[index].Attributes["instance"].Value)
                                {
                                    instance = cat.Nodes[cat.Nodes.Count - 1];
                                }
                                else
                                {
                                    instance = new RadTreeNode();
                                    instance.Text = serverNode.ChildNodes[index].Attributes["instance"].Value;
                                    instance.ShowCheckBox = true;
                                    instance.Checked = true;
                                    cat.Nodes.Add(instance);
                                }
                                RadTreeNode counter = new RadTreeNode();
                                counter.Text = serverNode.ChildNodes[index].Attributes["countername"].Value;
                                counter.Tag = serverNode.ChildNodes[index];
                                instance.Nodes.Add(counter);
                            }
                        }
                        else
                        {
                            RadTreeNode counter = new RadTreeNode();
                            counter.Text = serverNode.ChildNodes[index].Attributes["countername"].Value;
                            counter.Tag = serverNode.ChildNodes[index];
                            cat.Nodes.Add(counter);
                        }
                    }
                }
                tvChart.Nodes.Add(servers);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                string monitorDataDirectory = _constant.DataFolderPathMonitor;
                if (Directory.Exists(monitorDataDirectory) == false)
                {
                    Directory.CreateDirectory(monitorDataDirectory);
                }
                if (txtReportName.Text != String.Empty)
                {
                    string folderPath = monitorDataDirectory + "\\" + txtReportName.Text;
                    if (btnRun.Text == "&Run")
                    {
                        if (Directory.Exists(folderPath))
                        {
                            MessageBox.Show("Name Alredy Exists");
                        }
                        else
                        {
                            XmlDocument doc = GetRunMonitorXml();

                            if (doc.SelectSingleNode(".//counter") != null)
                            {
                                Directory.CreateDirectory(folderPath);
                                _reports.Rows.Add(txtReportName.Text);
                                doc.Save(File.Create(folderPath + "\\monitor.xml"));
                                File.Copy(Constants.GetInstance().ExecutingAssemblyLocation + "\\databasemonitor.db", folderPath + "\\databasemonitor.db");
                                File.Delete(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute_monitor.bat");
                                File.WriteAllText(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute_monitor.bat", @"sqlite3 .\DataMonitor\" + txtReportName.Text + @"\databasemonitor.db" + " < commands_monitor.txt");
                                _runningMonitors.Add(new MonitorServer(doc.SelectSingleNode(".//monitor/server")));

                                foreach (MonitorServer server in _runningMonitors)
                                {
                                    server.Start();
                                }
                                btnRun.Text = "&Stop";
                            }
                            else
                            {
                                MessageBox.Show("Please select counter");
                            }
                        }
                    }
                    else
                    {
                        foreach (MonitorServer server in _runningMonitors)
                        {
                            server.Stop();
                        }
                        _runningMonitors.Clear();
                        btnRun.Text = "&Run";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        public XmlDocument GetRunMonitorXml()
        {
            int counterid = 0;
            XmlDocument monitorDoc = new XmlDocument();
            monitorDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><monitor></monitor>");
            XmlNode monitorNode = monitorDoc.SelectSingleNode(".//monitor");

            foreach (RadTreeNode server in tvServersWithCounters.Nodes[0].Nodes)
            {
                if (server.Checked == true)
                {
                    XmlNode serveerNode = monitorDoc.ImportNode((XmlNode)server.Tag, false);
                    monitorNode.AppendChild(serveerNode);

                    foreach (RadTreeNode cat in server.Nodes)
                    {
                        if (cat.Checked == true)
                        {
                            foreach (RadTreeNode instst in cat.Nodes)
                            {
                                if (instst.Checked == true)
                                {
                                    if (instst.Nodes.Count > 0)
                                    {
                                        foreach (RadTreeNode counter in instst.Nodes)
                                        {
                                            if (counter.Checked == true)
                                            {
                                                XmlNode counterNode = monitorDoc.ImportNode((XmlNode)counter.Tag, true);
                                                counterNode.Attributes.Append(GetAttribute(monitorDoc, "counterid", (++counterid).ToString()));
                                                serveerNode.AppendChild(counterNode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        XmlNode counterNode = monitorDoc.ImportNode((XmlNode)instst.Tag, true);
                                        counterNode.Attributes.Append(GetAttribute(monitorDoc, "counterid", (++counterid).ToString()));
                                        serveerNode.AppendChild(counterNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return monitorDoc;
        }

        private XmlAttribute GetAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        private void CopyNode(XmlNode source, XmlNode destination)
        {
            foreach (XmlAttribute att in source.Attributes)
            {
                destination.Attributes.Append(att);
            }
        }

        private void ddlReportName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlReportName.SelectedIndex > 0)
            {
                LoadServersForMonitor(ddlReportName.Text);
            }
            else
            {
                tvChart.Nodes.Clear();
            }
        }

        private void tvChart_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Tag != null && e.Node.Nodes.Count == 0)
                {
                    radGridView1.Visible = true;
                    XmlNode counter = (XmlNode)e.Node.Tag;
                    DataTable dt = ResultLogMonitor.GetInstance().GetCounterData(ddlReportName.Text, counter.Attributes["counterid"].Value);
                    radGridView1.DataSource = dt;
                }
                else
                {
                    radGridView1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        //public void LoadCounterCategory()
        //{
        //    if (isLoaded == false)
        //    {
        //        tvMonitor.Nodes.Clear();
        //        bool isChild = false;
        //        foreach (PerformanceCounterCategory cat in System.Diagnostics.PerformanceCounterCategory.GetCategories("192.168.1.118"))
        //        {
        //            isChild = false;
        //            RadTreeNode category = new RadTreeNode();
        //            category.Text = cat.CategoryName;

        //            if (cat.CategoryType != PerformanceCounterCategoryType.SingleInstance)
        //            {
        //                string[] names = cat.GetInstanceNames();
        //                foreach (string name in names)
        //                {
        //                    RadTreeNode instance = new RadTreeNode();
        //                    instance.Text = name;

        //                    try
        //                    {
        //                        foreach (var counter in cat.GetCounters(name))
        //                        {
        //                            isChild = true;
        //                            RadTreeNode counterNode = new RadTreeNode();
        //                            counterNode.Text = counter.CounterName;
        //                            counterNode.Tag = counter;
        //                            instance.Nodes.Add(counterNode);
        //                        }
        //                    }
        //                    catch
        //                    {
        //                    }

        //                    if (isChild == true)
        //                    {
        //                        category.Nodes.Add(instance);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (var counter in cat.GetCounters())
        //                {
        //                    isChild = true;
        //                    RadTreeNode counterNode = new RadTreeNode();
        //                    counterNode.Text = counter.CounterName;
        //                    counterNode.Tag = counter;
        //                    category.Nodes.Add(counterNode);
        //                }
        //            }

        //            //foreach (PerformanceCounter counter in cat.GetCounters("_Total"))
        //            //{
        //            //    RadTreeNode counterNode = new RadTreeNode();
        //            //    counterNode.Text = counter.CounterName;
        //            //    counterNode.Tag = counter;
        //            //    category.Nodes.Add(counterNode);
        //            //}
        //            if (isChild == true) tvMonitor.Nodes.Add(category);
        //        }
        //        isLoaded = true;
        //    }
        //}

        //private void UCMonitor_Load(object sender, EventArgs e)
        //{

        //}

        //private void UCMonitor_Enter(object sender, EventArgs e)
        //{

        //}

        //private void radButton1_Click(object sender, EventArgs e)
        //{


        //}

        //private void radListBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cout == null) cout = (PerformanceCounter)((RadListBoxItem)radListBox1.SelectedItem).Tag;
        //        lock (cout)
        //        {
        //            cout = (PerformanceCounter)((RadListBoxItem)radListBox1.SelectedItem).Tag;
        //            listView1.Items.Clear();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cout != null)
        //        {
        //            lock (cout)
        //            {
        //                ListViewItem item = new ListViewItem();
        //                item.Text = cout.CounterName;

        //                item.SubItems.AddRange(new string[] { DateTime.Now.ToString(),cout.NextValue().ToString() });
        //                listView1.Items.Add(item);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void tvMonitor_DoubleClick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (tvMonitor.SelectedNode != null && tvMonitor.SelectedNode.Nodes.Count == 0)
        //        {
        //            RadListBoxItem item = new RadListBoxItem();
        //            if (tvMonitor.SelectedNode.Parent.Parent == null)
        //            {
        //                item.Text = tvMonitor.SelectedNode.Parent.Text + "_" + tvMonitor.SelectedNode.Text;
        //            }
        //            else
        //            {
        //                item.Text = tvMonitor.SelectedNode.Parent.Parent.Text + "_" + tvMonitor.SelectedNode.Parent.Text + "_" + tvMonitor.SelectedNode.Text;

        //            }

        //            item.Tag = tvMonitor.SelectedNode.Tag;
        //            radListBox1.Items.Add(item);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        //private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        //private void radListBox1_DoubleClick(object sender, EventArgs e)
        //{
        //    if (radListBox1.SelectedItem != null)
        //    {
        //        radListBox1.Items.Remove((RadListBoxItem)radListBox1.SelectedItem);
        //        if (radListBox1.Items.Count == 0) cout = null;
        //    }
        //}

        //private void radPanel1_Paint(object sender, PaintEventArgs e)
        //{

        //}
    }
}
