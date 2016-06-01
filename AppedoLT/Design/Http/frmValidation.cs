using AppedoLT.BusinessLogic;
using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    /// <summary>
    /// Used to validate http script.
    /// 
    /// prerequisites: 
    ///  vuScript- Script that need to be validate
    ///  script- UI TreeNode to display right side tree.
    ///  _intCountRequest- Total number of request to calculate percentage for progress bar.
    /// 
    /// Author: Rasith
    /// </summary>
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

        /// <summary>
        /// To create validation form
        /// </summary>
        /// <param name="vuScript">Script to be validated</param>
        /// <param name="script">Tree for UI</param>
        /// <param name="_intCountRequest">Total no of request count</param>
        public frmValidation(XmlNode vuScript, RadTreeNode script, int _intCountRequest)
        {
            try
            {
                //Timer task. Timer will method timer_tick in given interval
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

        /// <summary>
        /// Used to create vuser for validation. this vuser act as single user.
        /// </summary>
        /// <returns>Vuser(single user)</returns>
        private VUser GetUser()
        {
            // For validation set the bandwidth value as -1, which indicates no thrortling, full speed will be utilized
            VUser _vUSer = new VUser(1, DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss"), "1", 1, 1, _vuScript, false, Request.GetIPAddress(1), true,"1", -1);
            _vUSer.IsValidation = true;
            //Mapping event to methods. If vuser has request and response data it will call _vUSer_OnLockRequestResponse method
            _vUSer.OnLockRequestResponse += _vUSer_OnLockRequestResponse;
            //If vuser has Error it will call _vUSer_OnLockError method
            _vUSer.OnLockError += _vUSer_OnLockError;
            //If vuser has Log it will call _vUSer_OnLockLog method
            _vUSer.OnLockLog += _vUSer_OnLockLog;
            return _vUSer;
        }

        /// <summary>
        /// During the validation, it will be called by vuser if log exist. 
        /// </summary>
        /// <param name="data">Log data</param>
        void _vUSer_OnLockLog(Log data)
        {
            _logObj.Add(data);
            btnViewLog.Text = "&Logs(" + _logObj.Count.ToString() + ")";
        }

        /// <summary>
        /// During the validation, it will be called by vuser if error exist. 
        /// </summary>
        /// <param name="data">Error data</param>
        void _vUSer_OnLockError(RequestException data)
        {
            _errorObj.Add(data);
            btnViewError.Text = "&Errors(" + _errorObj.Count.ToString() + ")";
        }

        /// <summary>
        /// During the validation, it will be called by vuser for each request. 
        /// </summary>
        /// <param name="data">Request and corresponding response</param>
        void _vUSer_OnLockRequestResponse(RequestResponse requestResponse)
        {
            try
            {
                ListViewItem newItem = new ListViewItem(requestResponse.WebRequestResponseId.ToString());
                newItem.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                // If request has error response
                if (requestResponse.RequestResult.HasError == true || requestResponse.RequestResult.Success == false)
                {
                    //It show green color
                    newItem.StateImageIndex = 1;
                }
                else
                {
                    //It show red color
                    newItem.StateImageIndex = 0;
                }

                newItem.Tag = requestResponse;
                //Adding details into grid
                newItem.SubItems.AddRange(new string[] { requestResponse.RequestResult.RequestId.ToString(), requestResponse.ContainerName, requestResponse.RequestResult.RequestName, requestResponse.RequestResult.StartTime.ToString(), requestResponse.RequestResult.EndTime.ToString(), requestResponse.RequestResult.ResponseTime.ToString(), requestResponse.RequestResult.ResponseCode.ToString(), requestResponse.RequestResult.Success.ToString() });
                lsvResult.Items.Add(newItem);
                lblHitcoutValue.Text = lsvResult.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// If user click validation button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                // Check for network connectivity has changed 
                if (RequestCountHandler._NetworkConnectedIP != System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString())
                {
                    MessageBox.Show("Looks like you have changed to a different network after you opened the tool. Please Restart the Appedo LT tool");
                    this.Close();
                }
                else
                {
                    AppedoLT.Core.Constants.GetInstance().btnExecutionType = "Validate";
                    //If it is first validation  or no other validation is running.
                    if (firstRun == true || _vUSer.WorkCompleted == true)
                    {
                        _errorObj.Clear();
                        _logObj.Clear();
                        btnViewError.Text = "&Errors(" + _errorObj.Count.ToString() + ")";
                        btnViewLog.Text = "&Logs(" + _logObj.Count.ToString() + ")";
                        lblHitcoutValue.Text = lsvResult.Items.Count.ToString();
                        if (firstRun == true) firstRun = false;
                        //Creating data center for variables
                        VariableManager.dataCenter = new VariableManager();
                        lsvResult.Items.Clear();
                        lblVResult.Text = string.Empty;
                        Clear();
                        lblVResult.Visible = true;
                        //Creating single vuser object.
                        _vUSer = GetUser();
                        _vUSer.Start();
                        timer.Start();
                        // stop watch used to calculate  response time.
                        stopWatch.Reset();
                        stopWatch.Start();
                        lblAvgResponse.Text = string.Empty;
                        lblStatus.Text = "Started";
                        _vUSer.WorkCompleted = false;
                        btnValidate.Enabled = false;

                    }
                    //Another validation is in progress.
                    else if (_vUSer.WorkCompleted == false)
                    {
                        /*thread.Resume();
                        stopWatch.Start();
                        timer.Start();*/
                        MessageBox.Show("Another script is Running");
                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// To clear all data in validation grid.
        /// </summary>
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

        /// <summary>
        /// To notify validation completion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_tick(object sender, EventArgs e)
        {
            try
            {
                //If vuser completed all transactions.
                if (_vUSer.WorkCompleted == false)
                {
                    //Calculating avg response time.
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
                    //Total duration
                    lblTotalDuration.Text = stopWatch.Elapsed.ToString();
                }
                else
                {
                    // Frequently update status until user complete.
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

                    //Getting all failed request to set TreeView node red color.
                    foreach (ListViewItem item in lsvResult.Items)
                    {
                        //Failed response.
                        if (item.SubItems[8].Text == "False")
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

        /// <summary>
        /// To calculate avg response time after validation complete.
        /// </summary>
        /// <returns>Avg response time</returns>
        public double Avg()
        {
            double result = 0;

            //Getting sum of response time.
            foreach (ListViewItem item in this.lsvResult.Items)
            {
                double temp = 0;
                double.TryParse(item.SubItems[6].Text, out temp);
                result += temp;
            }

            if (this.lsvResult.Items.Count > 0)
            {
                //Calculate avg.
                result = result / this.lsvResult.Items.Count;
            }
            return result;
        }

        /// <summary>
        /// Set tree node color as Red if there is any error.
        /// </summary>
        /// <param name="node">Nodes contain all request</param>
        private void SetTreeNodeError(RadTreeNode node)
        {
            try
            {
                if (node.Nodes.Count > 0)
                {
                    node.Expand();
                    foreach (RadTreeNode childNode in node.Nodes)
                    {
                        //Reset default
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
                            //Set red color for failed request
                            node.BackColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            //Set white color for successful request
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

                    //If user selected item in grid
                    if (requestRespose != null)
                    {
                        lblPath.Text = requestRespose.RequestResult.RequestName;
                        lblStartTime.Text = requestRespose.RequestResult.StartTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblEndTime.Text = requestRespose.RequestResult.EndTime.ToString("dd/MMM/yyyy hh:mm:ss");
                        lblDuration.Text = requestRespose.RequestResult.ResponseTime.ToString() + " ms";
                        //If user has valid response
                        if (requestRespose.RequestResult.HasError == true)
                        {
                            //strValidatedResponse = requestRespose.RequestResult.ErrorMessage + Environment.NewLine + requestRespose.RequestResult.ResponseStr;
                            strValidatedResponse = requestRespose.RequestResult.ResponseStr;
                        }
                        else
                        {
                            strValidatedResponse = requestRespose.RequestResult.ResponseStr;
                        }
                        richTextBox1.Text = requestRespose.RequestResult.AssertionFaildMsg.ToString();
                        txtResponse.Text = strValidatedResponse;
                        gvHeader.DataSource = ConvertToTable(requestRespose.RequestResult.RequestNode.SelectSingleNode("headers"));
                        gvParameters.DataSource = ConvertToTable(requestRespose.RequestResult.Parameters);
                        gvVariables.DataSource = ConvertToTable(requestRespose.RequestResult.Variables);
                        gvExtractedVariables.DataSource = ConvertToTable(requestRespose.RequestResult.ExtractedVariables);

                        if (radTabStrip1.SelectedTab == tabItem1) webBrowser1.DocumentText = txtResponse.Text;

                        //Select corresponding tree node request in right side tree view
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

        /// <summary>
        /// Convert xmlnode into data table.
        /// </summary>
        /// <param name="root">xml which contain attribute name and value</param>
        /// <returns>Table that contain name and value</returns>
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

        /// <summary>
        /// Convert key and value collection into table
        /// </summary>
        /// <param name="root">key value collection</param>
        /// <returns>Table that contains name and value column</returns>
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
            // To show logs tables to user
            new frmShowData("Logs", _logObj).ShowDialog();
        }

        private void btnViewError_Click(object sender, EventArgs e)
        {
            // To show Errors tables to user
            new frmShowData("Errors", _errorObj).ShowDialog();
        }

    }
}
