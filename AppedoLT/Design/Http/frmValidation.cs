using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using AppedoLT.BusinessLogic;
using Telerik.WinControls.UI;
using System.Drawing;
using System.ComponentModel;

namespace AppedoLT
{
    public partial class frmValidation : Telerik.WinControls.UI.RadForm
    {
        XmlNode _vuScript;
        AppedoLT.BusinessLogic.VUser _vUSer = null;
        bool isBreak = false;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Stopwatch stopWatch = new Stopwatch();
        List<string> errorRequestid = new List<string>();
        String strValidatedResponse = String.Empty;
        String strRecordedResponse = String.Empty;
        int intCountRequest;
        Thread backgroundThread1;
        bool firstRun;

        BindingList<Log> _logObj = new BindingList<Log>();
        BindingList<RequestException> _errorObj = new BindingList<RequestException>();

       

        public frmValidation(XmlNode vuScript, RadTreeNode script, int _intCountRequest)
        {
            try
            {
                timer.Tick += new EventHandler(timer_tick);
                timer.Interval = 100;
                InitializeComponent();
                lblPath.Text = string.Empty;
                lblStartTime.Text = string.Empty;
                lblEndTime.Text = string.Empty;
                lblDuration.Text = string.Empty;
                txtResponse.Text = string.Empty;
                lblStatus.Text = string.Empty;
                lblVResult.Text = string.Empty;
                lblTotalDuration.Text = string.Empty;
                lblAvgResponse.Text = string.Empty;
                _vuScript = vuScript;

                tvRequest.Nodes.Clear();
                tvRequest.Nodes.Add(script);

                lsvResult.Items.Clear();

                intCountRequest = _intCountRequest;
                firstRun = true;
                
                btnViewError.Text = "&Errors(" + _errorObj.Count.ToString() + ")";
                btnViewLog.Text = "&Logs(" + _logObj.Count.ToString() + ")";

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private VUser GetUser()
        {
            VUser _vUSer = new VUser(1, DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss"), "1", 1, 1, _vuScript, false, Request.GetIPAddress(1));
            _vUSer.IsValidation = true;
            _vUSer.OnLockRequestResponse += _vUSer_OnLockRequestResponse;
            _vUSer.OnLockError += _vUSer_OnLockError;
            _vUSer.OnLockLog += _vUSer_OnLockLog;
            return _vUSer;
        }

        void _vUSer_OnLockLog(Log data)
        {
            _logObj.Add(data);
            btnViewLog.Text = "&Logs(" + _logObj.Count.ToString() + ")";
        }

        void _vUSer_OnLockError(RequestException data)
        {
            _errorObj.Add(data);
            btnViewError.Text = "&Errors(" + _errorObj.Count.ToString() + ")";
        }

        void _vUSer_OnLockRequestResponse(RequestResponse requestResponse)
        {
            try
            {
                ListViewItem newItem = new ListViewItem(requestResponse.WebRequestResponseId.ToString());
                newItem.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                if (requestResponse.RequestResult.HasError == true || requestResponse.RequestResult.Success == false)
                {
                    newItem.StateImageIndex = 1;
                }
                else
                {
                    newItem.StateImageIndex = 0;
                }

                newItem.Tag = requestResponse;
                newItem.SubItems.AddRange(new string[] {requestResponse.ContainerName, requestResponse.RequestResult.RequestId.ToString(), requestResponse.RequestResult.RequestName, requestResponse.RequestResult.StartTime.ToString(), requestResponse.RequestResult.EndTime.ToString(), requestResponse.RequestResult.ResponseTime.ToString(), requestResponse.RequestResult.ResponseCode.ToString() });
                lsvResult.Items.Add(newItem);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                if (firstRun == true || _vUSer.WorkCompleted == true)
                {
                    _errorObj.Clear();
                    _logObj.Clear();
                    btnViewError.Text = "&Errors(" + _errorObj.Count.ToString() + ")";
                    btnViewLog.Text = "&Logs(" + _logObj.Count.ToString() + ")";
                    if (firstRun == true) firstRun = false;
                    VariableManager.dataCenter = new VariableManager();
                    lsvResult.Items.Clear();
                    lblVResult.Text = string.Empty;
                    Clear();
                    lblVResult.Visible = true;
                    _vUSer = GetUser();
                    _vUSer.Start();
                    timer.Start();
                    stopWatch.Reset();
                    stopWatch.Start();
                    lblAvgResponse.Text = string.Empty;
                    lblStatus.Text = "Started";
                    _vUSer.WorkCompleted = false;
                    btnValidate.Enabled = false;

                }
                else if (_vUSer.WorkCompleted == false)
                {
                    /*thread.Resume();
                    stopWatch.Start();
                    timer.Start();*/
                    MessageBox.Show("Another script is Running");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public void Clear()
        {
            lblPath.Text = string.Empty;
            lblStartTime.Text = string.Empty;
            lblEndTime.Text = string.Empty;
            lblDuration.Text = string.Empty;
            txtResponse.Text = string.Empty;
            gvParameters.DataSource = null;
            gvHeader.DataSource = null;

        }

        private void timer_tick(object sender, EventArgs e)
        {
            try
            {
                if (_vUSer.WorkCompleted == false)
                {
                    int inGridVal = Int32.Parse(lsvResult.Items.Count.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                    int inintCountRequest = Int32.Parse(intCountRequest.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                    String a = inGridVal.ToString();
                    String b = inintCountRequest.ToString();
                    try
                    {
                        progressValidation.Value1 = Convert.ToInt32(Convert.ToDecimal(a) / Convert.ToDecimal(b) * 100);
                    }
                    catch
                    {
                    }
                    lblStatus.Text = "Running";
                    lblTotalDuration.Text = stopWatch.Elapsed.ToString();
                }
                else
                {
                    progressValidation.Value1 = 100;
                    stopWatch.Stop();
                    try
                    {
                        lblAvgResponse.Text = Avg().ToString() + " ms";
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    lblTotalDuration.Text = stopWatch.Elapsed.ToString();
                    Thread.Sleep(100);
                    timer.Stop();
                    lblStatus.Text = "Completed";
                    btnValidate.Enabled = true;
                    bool isError = false;
                    errorRequestid.Clear();
                    foreach (ListViewItem item in lsvResult.Items)
                    {
                        if (item.SubItems[7].Text == "False")
                        {
                            errorRequestid.Add(item.SubItems[1].Text.ToString());
                        }
                    }

                    if (errorRequestid.Count > 0) lblVResult.Text = "Failed";
                    else if (lblStatus.Text == "Completed") lblVResult.Text = "Success";
                    SetTreeNodeError(tvRequest.Nodes[0]);
                }
                if (_vUSer == null)
                {
                    progressValidation.Value1 = 0;
                    lblStatus.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public double Avg()
        {
            double result = 0;
            foreach (ListViewItem item in this.lsvResult.Items)
            {
                double temp = 0;
                double.TryParse(item.SubItems[5].Text, out temp);
                result += temp;
            }
            if (this.lsvResult.Items.Count > 0)
            {
                result = result / this.lsvResult.Items.Count;
            }
            return result;
        }

        private void SetTreeNodeError(RadTreeNode node)
        {
            try
            {
                if (node.Nodes.Count > 0)
                {
                    node.Expand();
                    foreach (RadTreeNode childNode in node.Nodes)
                    {
                        SetTreeNodeError(childNode);
                    }
                }
                else
                {

                    if (node.Tag != null && ((XmlNode)node.Tag).Name == "request")
                    {

                        if (errorRequestid.FindAll(f => f == (((XmlNode)node.Tag).Attributes["id"].Value)).Count > 0)
                        {
                            errorRequestid.FindAll(f => f == ((XmlNode)node.Tag).Attributes["id"].Value).Clear();
                            node.BackColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            node.BackColor = System.Drawing.Color.White;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void tvRequest_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (tvRequest.Focused == true)
                {
                    XmlNode request = (XmlNode)e.Node.Tag;
                    string requestid = request.Attributes["id"].Value;
                    strRecordedResponse = String.Empty;
                    lock (this.lsvResult)
                    {
                        int index1;
                        for (int index = 0; index < lsvResult.Items.Count; index++)
                        {
                            if (lsvResult.Items[index].SubItems[1].Text == requestid)
                            {
                                this.lsvResult.Items[index].Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }

        }

        private void txtResponse_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
                {
                    FindDig fDig = new FindDig();
                    fDig.SetTextBox(txtResponse);
                    fDig.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void radTabStrip1_TabSelected(object sender, TabEventArgs args)
        {
            if (args.TabItem == tabItem1) webBrowser1.DocumentText = txtResponse.Text;
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvRequest.SelectedNode != null)
            {
                tvRequest.SelectedNode.ExpandAll();
            }
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvRequest.SelectedNode != null)
            {
                tvRequest.SelectedNode.CollapseAll();
            }
        }

        private void txtResponse_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            try
            {
                RadTreeNodeCollection tnc;
                tnc = tvRequest.Nodes;
                // FindNodeInHierarchy(tnc, (gvRecord.CurrentRow.Cells["Address"].Value).ToString(), (gvRecord.CurrentRow.Cells["Requestid"].Value).ToString());

                frmCompareResponse objFrmCompare = new frmCompareResponse(strRecordedResponse, strValidatedResponse);
                objFrmCompare.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_vUSer.WorkCompleted == false)
                {
                    _vUSer.Stop();
                    timer.Stop();
                    stopWatch.Stop();
                    lblStatus.Text = "Stopped";
                    lblVResult.Text = "Stopped";
                    _vUSer.Break = false;
                    btnValidate.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void frmValidation_Load(object sender, EventArgs e)
        {
            Constants.GetInstance().IsValidationScreenOpen = true;
        }

        private void frmValidation_FormClosed(object sender, FormClosedEventArgs e)
        {
            Constants.GetInstance().IsValidationScreenOpen = false;
            btnStop_Click(null, null);
        }

        private void lsvResult_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.IsSelected == true)
                {
                    RequestResponse requestRespose = (RequestResponse)e.Item.Tag;
                    if (requestRespose != null)
                    {
                        lblPath.Text = requestRespose.RequestResult.RequestName;
                        lblStartTime.Text = requestRespose.RequestResult.StartTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblEndTime.Text = requestRespose.RequestResult.EndTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblDuration.Text = requestRespose.RequestResult.ResponseTime.ToString() + " ms";
                        if (requestRespose.RequestResult.HasError == true)
                        {
                            strValidatedResponse = requestRespose.RequestResult.ErrorMessage + Environment.NewLine + requestRespose.RequestResult.ResponseStr;
                        }
                        else
                        {
                            strValidatedResponse = requestRespose.RequestResult.ResponseStr;
                        }
                        txtResponse.Text = strValidatedResponse;
                        gvHeader.DataSource = ConvertToTable(requestRespose.RequestResult.RequestNode.SelectSingleNode("headers"));
                        gvParameters.DataSource = ConvertToTable(requestRespose.RequestResult.Parameters);
                        gvVariables.DataSource = ConvertToTable(requestRespose.RequestResult.Variables);
                        gvExtractedVariables.DataSource = ConvertToTable(requestRespose.RequestResult.ExtractedVariables);

                        if (radTabStrip1.SelectedTab == tabItem1) webBrowser1.DocumentText = txtResponse.Text;
                        SelectRequest(tvRequest.Nodes[0], requestRespose.RequestResult.RequestId.ToString());

                        // SelectTreeNode(tvRequest.Nodes[0], requestRespose.RequestId);
                    }
                    else
                    {
                        Clear();
                    }
                    if (lsvResult.Focused == true && requestRespose.RequestId != "0")
                    {
                        foreach (RadTreeNode node in tvRequest.Nodes)
                        {
                            SelectRequest(node, requestRespose.RequestId);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private DataTable ConvertToTable(XmlNode root)
        {
            DataTable resut = new DataTable();
            resut.Columns.Add("Name");
            resut.Columns.Add("Value");
            if (root != null)
            {
                foreach (XmlNode key in root.ChildNodes)
                {
                    resut.Rows.Add(key.Attributes["name"].Value, key.Attributes["value"].Value);
                }
            }
            return resut;
        }

        private DataTable ConvertToTable(List<AppedoLT.Core.Tuple<string, string>> root)
        {
            DataTable resut = new DataTable();
            resut.Columns.Add("Name");
            resut.Columns.Add("Value");
            if (root != null)
            {
                foreach (AppedoLT.Core.Tuple<string, string> key in root)
                {
                    resut.Rows.Add(key.Key, key.Value);
                }
            }
            return resut;
        }
        private void SelectRequest(RadTreeNode node, string requestid)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (RadTreeNode nodes in node.Nodes)
                {
                    SelectRequest(nodes, requestid);
                }
            }
            else if (((XmlNode)node.Tag).Name == "request" && ((XmlNode)node.Tag).Attributes["id"].Value == requestid && requestid != "0")
            {
                node.Selected = true;

            }
        }

        private void lsvResult_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnValidate_EnabledChanged(object sender, EventArgs e)
        {
            if (btnValidate.Enabled == true)
            {
                btnStop.Enabled = false;
            }
            else
            {
                btnStop.Enabled = true;
            }
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            new frmShowData("Logs", _logObj).ShowDialog();
        }

        private void btnViewError_Click(object sender, EventArgs e)
        {
            new frmShowData("Errors", _errorObj).ShowDialog();
        }

    }
}
