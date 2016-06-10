using AppedoLT.BusinessLogic;
using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class frmTCPIPValidation : Telerik.WinControls.UI.RadForm
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

        public frmTCPIPValidation(XmlNode vuScript, RadTreeNode script, int _intCountRequest)
        {
            try
            {
                ListView.CheckForIllegalCrossThreadCalls = false;
                timer.Tick += new EventHandler(timer_tick);
                timer.Interval = 100;
                InitializeComponent();
                lblAddress.Text = string.Empty;
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
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private VUser GetUser()
        {
            //VUser _vUSer = new VUser(1, DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss"), "1", 1, 1, _vuScript, false, Request.GetIPAddress(1),true);
            VUser _vUSer = new VUser(1, DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss"), "1", 1, 1, _vuScript, false, Request.GetIPAddress(1), true, "1", -1);
            _vUSer.IsValidation = true;
            _vUSer.OnLockRequestResponse += _vUSer_OnLockRequestResponse;
            return _vUSer;
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
                newItem.SubItems.AddRange(new string[] { requestResponse.RequestResult.RequestId.ToString(), requestResponse.RequestResult.RequestName, requestResponse.RequestResult.StartTime.ToString(), requestResponse.RequestResult.EndTime.ToString(), requestResponse.RequestResult.ResponseTime.ToString(), requestResponse.RequestResult.ResponseCode.ToString(), requestResponse.RequestResult.Success.ToString() });
                lsvResult.Items.Add(newItem);
            }
            catch(Exception ex)
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

        private void gvRecord_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (lblStatus.Text != "Running")
                {
                    //isBreak = false;
                    //int id = 0;
                    //int.TryParse(gvRecord[0, e.RowIndex].Value.ToString(), out id);
                    //ResponseResponseSummary requestRespose = ResponseResponseSummary.validationResult.Find(f => f.WebRequestResponseId == id);
                    //if (requestRespose != null)
                    //{
                    //    lblAddress.Text = requestRespose.TcpIPRequest;
                    //    lblStartTime.Text = requestRespose.StartTime.ToString("dd/MMM/yyyy hh:mm:ss");
                    //    lblEndTime.Text = requestRespose.EndTime.ToString("dd/MMM/yyyy hh:mm:ss");
                    //    lblDuration.Text = requestRespose.Duration.ToString() + " ms";
                    //    txtResponse.Text = requestRespose.TcpIPResponse;
                    //    strValidatedResponse = requestRespose.TcpIPResponse;
                    //    txtRequest.Text = requestRespose.TcpIPRequest;
                    //    gvParameters.DataSource = requestRespose.Parameters;
                    //}
                    //else
                    //{
                    //    Clear();
                    //}
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public void Clear()
        {
            lblAddress.Text = string.Empty;
            lblStartTime.Text = string.Empty;
            lblEndTime.Text = string.Empty;
            lblDuration.Text = string.Empty;
            txtResponse.Text = string.Empty;
            gvParameters.DataSource = null;
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
                    if (node.Tag != null && node.Tag.GetType().Name == "Request")
                    {
                        //if (errorRequestid.FindAll(f => f == ((Request)node.Tag).requestid).Count > 0)
                        //{
                        //    errorRequestid.FindAll(f => f == ((Request)node.Tag).requestid).Clear();
                        //    node.BackColor = System.Drawing.Color.Red;
                        //}
                        //else
                        //{
                        //    node.BackColor = System.Drawing.Color.White;
                        //}
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
                if (_vUSer.WorkCompleted == true)
                {
                    _vUSer.Stop();
                    timer.Stop();
                    stopWatch.Stop();
                    lblStatus.Text = "Stopped";
                    lblVResult.Text = "Stopped";
                    //if (gvRecord.DataSource != null)
                    //{

                    //    lblAvgResponse.Text = ((DataTable)gvRecord.DataSource).Compute("avg(ResTime)", null).ToString() + " ms";
                    //}
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

                RequestResponse requestRespose = (RequestResponse)e.Item.Tag;
                if (requestRespose != null)
                {
                    if (requestRespose.Request != null)
                    {
                        lblAddress.Text = requestRespose.Request.Address.AbsoluteUri;
                        lblStartTime.Text = requestRespose.StartTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblEndTime.Text = requestRespose.EndTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblDuration.Text = requestRespose.Duration.ToString() + " ms";
                        txtResponse.Text = requestRespose.Response;
                        strValidatedResponse = requestRespose.Response;
                        gvParameters.DataSource = requestRespose.Parameters;
                        
                    }
                    else
                    {
                        lblAddress.Text = requestRespose.RequestResult.RequestName;
                        txtRequest.Text = requestRespose.RequestResult.RequestNode.Attributes["requestcontent"].Value;
                        lblStartTime.Text = requestRespose.RequestResult.StartTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblEndTime.Text = requestRespose.RequestResult.EndTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblDuration.Text = requestRespose.RequestResult.ResponseTime.ToString() + " ms";
                        txtResponse.Text = requestRespose.RequestResult.ResponseStr;
                        strValidatedResponse = requestRespose.RequestResult.ResponseStr;
                        //gvParameters.DataSource = requestRespose.Parameters;
                    }

                    // SelectTreeNode(tvRequest.Nodes[0], requestRespose.RequestId);
                }
                else
                {
                    Clear();
                }
                if (lsvResult.Focused == true)
                {
                    foreach (RadTreeNode node in tvRequest.Nodes)
                    {
                        SelectRequest(node, requestRespose.RequestId);
                    }
                }
            }

            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
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
            else if (((XmlNode)node.Tag).Name == "request" && ((XmlNode)node.Tag).Attributes["id"].Value == requestid)
            {
                node.Selected = true;

            }
        }

        private void frmTCPIPValidation_Load(object sender, EventArgs e)
        {
            Constants.GetInstance().IsValidationScreenOpen = true;
        }

        private void frmTCPIPValidation_FormClosed(object sender, FormClosedEventArgs e)
        {
            Constants.GetInstance().IsValidationScreenOpen = false;
        }
    }
}
