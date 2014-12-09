using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using System.Diagnostics;

namespace AppedoLT
{
    public partial class UCServer : UserControl
    {
        private static UCServer _instance;
        MonitorXml _monitorxml = MonitorXml.GetInstance();
        XmlNode _serverDetail = null;
        public static UCServer GetInstance(XmlNode serverDetail, RadTreeNode node)
        {
            if (_instance == null)
            {
                _instance = new UCServer();
            }
            _instance.Tag = node;
            _instance._serverDetail = serverDetail;
            _instance.SetValue(serverDetail);
            return _instance;
        }

        private UCServer()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        private void SetValue(XmlNode serverDetail)
        {
            ddlType.SelectedIndex = 0;
            txtName.Text = serverDetail.Attributes["name"].Value;
            txtName.Tag = serverDetail.Attributes["name"];
            txtIPAddress.Text = serverDetail.Attributes["ipaddress"].Value;
            txtIPAddress.Tag = serverDetail.Attributes["ipaddress"];
            txtPassword.Text = serverDetail.Attributes["password"].Value;
            txtPassword.Tag = serverDetail.Attributes["password"];
            txtInterval.Text = serverDetail.Attributes["interval"].Value;
            txtInterval.Tag = serverDetail.Attributes["interval"];
            LoadMappedCounters(serverDetail.SelectSingleNode(".//counters"));

        }

        private void LoadMappedCounters(XmlNode counters)
        {
            tvMappedCounters.Nodes.Clear();

            for (int index = 0; index < counters.ChildNodes.Count; index++)
            {
                RadTreeNode cat;
                if (tvMappedCounters.Nodes.Count > 0 && tvMappedCounters.Nodes[tvMappedCounters.Nodes.Count - 1].Text == counters.ChildNodes[index].Attributes["category"].Value)
                {
                    cat = tvMappedCounters.Nodes[tvMappedCounters.Nodes.Count - 1];
                }
                else
                {
                    cat = new RadTreeNode();
                    cat.Text = counters.ChildNodes[index].Attributes["category"].Value;
                    tvMappedCounters.Nodes.Add(cat);
                }

                if (counters.ChildNodes[index].Attributes["instance"].Value != string.Empty)
                {
                    RadTreeNode instance;
                    if (cat.Nodes.Count == 0)
                    {
                        instance = new RadTreeNode();
                        instance.Text = counters.ChildNodes[index].Attributes["instance"].Value;
                        cat.Nodes.Add(instance);

                        RadTreeNode counter = new RadTreeNode();
                        counter.Text = counters.ChildNodes[index].Attributes["countername"].Value;
                        counter.Tag = counters.ChildNodes[index];
                        instance.Nodes.Add(counter);
                    }
                    else
                    {
                        if (cat.Nodes[cat.Nodes.Count - 1].Text == counters.ChildNodes[index].Attributes["instance"].Value)
                        {
                            instance = cat.Nodes[cat.Nodes.Count - 1];
                        }
                        else
                        {
                            instance = new RadTreeNode();
                            instance.Text = counters.ChildNodes[index].Attributes["instance"].Value;
                            cat.Nodes.Add(instance);
                        }
                        RadTreeNode counter = new RadTreeNode();
                        counter.Text = counters.ChildNodes[index].Attributes["countername"].Value;
                        counter.Tag = counters.ChildNodes[index];
                        instance.Nodes.Add(counter);
                    }
                }
                else
                {
                    RadTreeNode counter = new RadTreeNode();
                    counter.Text = counters.ChildNodes[index].Attributes["countername"].Value;
                    counter.Tag = counters.ChildNodes[index];
                    cat.Nodes.Add(counter);
                }
            }
        }

        private void txt_Validated(object sender, EventArgs e)
        {
            //if (text.Value != ((RadTextBox)sender).Text)
            //{
            //    text.Value=((RadTextBox)sender).Text;
            //}
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            XmlAttribute text = (XmlAttribute)txtName.Tag;
            if (text.Value != txtName.Text)
            {
                ((RadTreeNode)this.Tag).Text = txtName.Text;
                text.Value = txtName.Text;
            }
        }

        private void txtIPAddress_Validated(object sender, EventArgs e)
        {
            XmlAttribute text = (XmlAttribute)txtIPAddress.Tag;
            if (text.Value != txtIPAddress.Text)
            {
                text.Value = txtIPAddress.Text;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.PerformanceCounterCategory.GetCategories(txtIPAddress.Text);
                MessageBox.Show("Connection status: Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection status: failed"+Environment.NewLine+"Error: "+ex.Message);
            }
        }

        private void btnShowCounters_Click(object sender, EventArgs e)
        {
            tvAvailableCounters.Nodes.Clear();
            bool isChild = false;
            System.Diagnostics.PerformanceCounterCategory[] perList=null;
            try
            {
                perList=System.Diagnostics.PerformanceCounterCategory.GetCategories(txtIPAddress.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            foreach (PerformanceCounterCategory cat in perList)
            {
                isChild = false;
                RadTreeNode category = new RadTreeNode();
                category.Text = cat.CategoryName;

                if (cat.CategoryType != PerformanceCounterCategoryType.SingleInstance)
                {
                    string[] names = cat.GetInstanceNames();
                    foreach (string name in names)
                    {
                        RadTreeNode instance = new RadTreeNode();
                        instance.Text = name;
                        instance.ShowCheckBox = true;
                        try
                        {
                            foreach (var counter in cat.GetCounters(name))
                            {
                                isChild = true;
                                RadTreeNode counterNode = new RadTreeNode();
                                counterNode.Text = counter.CounterName;
                                counterNode.Tag = counter;
                                counterNode.ShowCheckBox = true;
                                if (_serverDetail.SelectSingleNode(".//counter[@countername='" + counterNode.Text + "' and @instance='" + instance.Text + "' and @category='" + category.Text + "']") != null)
                                {
                                    counterNode.Checked = true;
                                    category.Checked = true;
                                    instance.Checked = true;
                                }
                                instance.Nodes.Add(counterNode);
                            }
                        }
                        catch
                        {
                        }

                        if (isChild == true)
                        {
                            category.Nodes.Add(instance);
                        }
                    }
                }
                else
                {
                    foreach (var counter in cat.GetCounters())
                    {
                        isChild = true;
                        RadTreeNode counterNode = new RadTreeNode();
                        counterNode.Text = counter.CounterName;
                        counterNode.Tag = counter;
                        counterNode.ShowCheckBox = true;
                        if (_serverDetail.SelectSingleNode(".//counter[@countername='" + counterNode.Text + "' and @category='" + category.Text + "']") != null)
                        {
                            counterNode.Checked = true;
                            category.Checked = true;
                        }
                        category.Nodes.Add(counterNode);
                    }
                }
                if (isChild == true) tvAvailableCounters.Nodes.Add(category);
            }
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            tvMappedCounters.Nodes.Clear();
            foreach (RadTreeNode cat in tvAvailableCounters.Nodes)
            {
                if (cat.Checked == true)
                {
                    RadTreeNode mappedCat = new RadTreeNode();
                    mappedCat.Text = cat.Text;
                    mappedCat.ShowCheckBox = true;
                    foreach (RadTreeNode instst in cat.Nodes)
                    {
                        if (instst.Checked == true)
                        {
                            if (instst.Nodes.Count > 0)
                            {
                                RadTreeNode mappedInst = new RadTreeNode();
                                mappedInst.Text = instst.Text;
                                mappedInst.ShowCheckBox = true;
                                foreach (RadTreeNode counter in instst.Nodes)
                                {
                                    if (counter.Checked == true)
                                    {
                                        RadTreeNode mappedCounter = new RadTreeNode();
                                        mappedCounter.Text = counter.Text;
                                        mappedCounter.ShowCheckBox = true;
                                        mappedCounter.Tag = _monitorxml.CreateCounter(cat.Text, instst.Text, counter.Text);
                                        mappedInst.Nodes.Add(mappedCounter);
                                    }
                                }
                                mappedCat.Nodes.Add(mappedInst);
                            }
                            else
                            {
                                RadTreeNode mappedCounter = new RadTreeNode();
                                mappedCounter.Text = instst.Text;
                                mappedCounter.ShowCheckBox = true;
                                mappedCounter.Tag = _monitorxml.CreateCounter(cat.Text,string.Empty , instst.Text);
                                mappedCat.Nodes.Add(mappedCounter);
                            }
                        }
                    }
                    tvMappedCounters.Nodes.Add(mappedCat);
                }
            }
            UpdateCounters();
        }

        private void btnUnMap_Click(object sender, EventArgs e)
        {
            List<RadTreeNode> selectedNodes = new List<RadTreeNode>();

            foreach (RadTreeNode cat in tvMappedCounters.Nodes)
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
                                        selectedNodes.Add(counter);
                                    }
                                }
                            }
                            else
                            {
                                selectedNodes.Add(instst);
                            }
                        }
                    }
                }
            }
            for (int index = 0; index < selectedNodes.Count; index++)
            {
                selectedNodes[index].Remove();
               // selectedNodes.Remove(selectedNodes[index]);
            }
            selectedNodes.Clear();

            foreach (RadTreeNode cat in tvMappedCounters.Nodes)
            {
                if (cat.Nodes.Count == 0)
                {
                    selectedNodes.Add(cat);
                }
                else
                {
                    foreach (RadTreeNode instst in cat.Nodes)
                    {
                        if (instst.Tag == null)
                        {
                            if (instst.Nodes.Count == 0)
                            {
                                selectedNodes.Add(instst);
                            }
                        }

                    }
                }
            }

            for (int index = 0; index < selectedNodes.Count; index++)
            {
                selectedNodes[index].Remove();
            }
            selectedNodes.Clear();

            foreach (RadTreeNode cat in tvMappedCounters.Nodes)
            {
                if (cat.Nodes.Count == 0)
                {
                    selectedNodes.Add(cat);
                }
                else
                {
                    foreach (RadTreeNode instst in cat.Nodes)
                    {
                        if (instst.Tag == null)
                        {
                            if (instst.Nodes.Count == 0)
                            {
                                selectedNodes.Add(instst);
                            }
                        }

                    }
                }
            }

            for (int index = 0; index < selectedNodes.Count; index++)
            {
                selectedNodes[index].Remove();
            }
            selectedNodes.Clear();
            UpdateCounters();
        }

        private List<XmlNode> GetMappedNode()
        {
            List<XmlNode> mappedNodes = new List<XmlNode>();
            
            foreach (RadTreeNode cat in tvMappedCounters.Nodes)
            {
                foreach (RadTreeNode instst in cat.Nodes)
                {
                    if (instst.Tag == null)
                    {
                        foreach (RadTreeNode counter in instst.Nodes)
                        {
                            mappedNodes.Add((XmlNode)counter.Tag);
                        }
                    }
                    else
                    {
                       mappedNodes.Add((XmlNode)instst.Tag);
                    }

                }
            }
            return mappedNodes;
        }

        private void UpdateCounters()
        {
            try
            {
                XmlNode server = (XmlNode)((RadTreeNode)_instance.Tag).Tag;
                server.ChildNodes[0].RemoveAll();
                foreach (XmlNode counter in GetMappedNode())
                {
                    server.ChildNodes[0].AppendChild(counter);
                }
            }
            catch
            {
            }
        }

      

        private void txtInterval_Validated(object sender, EventArgs e)
        {
            XmlAttribute text = (XmlAttribute)txtInterval.Tag;
            if (text.Value != txtInterval.Text)
            {
                text.Value = txtInterval.Text;
            }
        }


        private void txtInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
    }
    class CounterInfo
    {
        public string Category { get; set; }
        public string Instance { get; set; }
        public string CounterName { get; set; }
        public CounterInfo(string category, string instance, string counterName)
        {
            Category = category;
            Instance = instance;
            CounterName = counterName;
        }
    }
}
