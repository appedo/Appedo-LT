using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;

namespace AppedoLT
{
    public partial class userControlReports : UserControl
    {
        public userControlReports()
        {
            InitializeComponent();
            LoadReportName(string.Empty);
            pnlPageSummaryReport.Visible = false;
            pnlRequestSummaryReport.Visible = false;
            pnlContainerSummaryReport.Visible = false;
            pnlTransactionSummaryReport.Visible = false;
            pnlSummaryReport.Dock = DockStyle.Fill;
            ClearData();
        }
        private Result _resultLog = Result.GetInstance();
        public void GenerateReport(string repoerName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = _resultLog.GetSummaryReport(ddlReportNameReport.Text);
                if (dt.Rows.Count > 0)
                {
                    lblStartTime.Text = dt.Rows[0]["start_time"].ToString();
                    lblEndTime.Text = dt.Rows[0]["end_time"].ToString();
                    TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(dt.Rows[0]["duration_sec"]));
                    string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", (t.Days * 24) + t.Hours, t.Minutes, t.Seconds);
                    lblDuration.Text = answer;
                    lblTotalUsers.Text = dt.Rows[0]["user_count"].ToString();
                    lblAvgResponse.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["avg_response"])/1000,3))  + " sec";
                    lblTotalHits.Text = dt.Rows[0]["total_hits"].ToString();
                    lblAvgHits.Text = Convert.ToString(dt.Rows[0]["avg_hits"]);//  Math.Round(Convert.ToDouble(dt.Rows[0]["avg_hits"]) / Convert.ToDouble(dt.Rows[0]["duration_sec"]), 4));
                    lblTotalThroughput.Text = Convert.ToString(Math.Round(((Convert.ToDouble(dt.Rows[0]["total_throughput"])) / 1024) / 1024, 3)) + " MB";
                    lblAvgThroughput.Text = Convert.ToString(Math.Round(((Convert.ToDouble(dt.Rows[0]["avg_throughput"]) * 8) / Convert.ToDouble(dt.Rows[0]["duration_sec"]) / 1024) / 1024, 3)) + " MBps";
                    lblTotalErrors.Text = dt.Rows[0]["total_errors"].ToString();
                    lblTotalPages.Text = dt.Rows[0]["total_page"].ToString();
                    lblAvgPages.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["avg_page_response"])/1000,3)) + " sec";
                    lblresponse200.Text = dt.Rows[0]["reponse_200"].ToString();
                    lblresponse300.Text = dt.Rows[0]["reponse_300"].ToString();
                    lblresponse400.Text = dt.Rows[0]["reponse_400"].ToString();
                    lblresponse500.Text = dt.Rows[0]["reponse_500"].ToString();
                }
                else
                    ClearData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        public void LoadReportName(string repoerName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = _resultLog.GetReportNameList(repoerName);
                ddlReportNameReport.DataSource = dt.Copy();
                ddlReportNameReport.DisplayMember = "reportname";
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        public Boolean IsManyUsers()
        {
            Boolean boolShowAvg = false;
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = _resultLog.GetSummaryReport(ddlReportNameReport.Text);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(dt.Rows[0]["user_count"]) > 1)
                    boolShowAvg = true;
            }
            return boolShowAvg;
        }
        private void btnShowReportReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (trvReportType.SelectedNode == null)
                    trvReportType.SelectedNode = trvReportType.Nodes[0];
                if (trvReportType.SelectedNode.Text == "1.Summary Report")
                {
                    pnlPageSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        GenerateReport(ddlReportNameReport.SelectedText.ToString());
                    }
                    else
                    {
                        ClearData();                        
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlSummaryReport.Visible = true;
                    pnlSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "2.Request ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetRequestSummaryReport(ddlReportNameReport.Text);
                       
                        grdRequestSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdRequestSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlRequestSummaryReport.Visible = true;
                    pnlRequestSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "3.Page ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetPageSummaryReport(ddlReportNameReport.Text);
                      
                        grdPageSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdPageSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlPageSummaryReport.Visible = true;
                    pnlPageSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "4.Container ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetContainerSummaryReport(ddlReportNameReport.Text);
                       
                        grdContainerSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdContainerSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlContainerSummaryReport.Visible = true;
                    pnlContainerSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "5.Transaction Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        grdTransactionSummaryReport.DataSource = _resultLog.GetTransactionReport(ddlReportNameReport.Text);
                    }
                    else
                    {
                        grdTransactionSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlTransactionSummaryReport.Visible = true;
                    pnlTransactionSummaryReport.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        public void ClearData()
        {
            try
            {
                lblStartTime.Text = String.Empty;
                lblEndTime.Text = String.Empty;
                lblDuration.Text = String.Empty;
                lblTotalUsers.Text = String.Empty;
                lblTotalHits.Text = String.Empty;
                lblAvgResponse.Text = String.Empty;
                lblAvgHits.Text = String.Empty;
                lblTotalThroughput.Text = String.Empty;
                lblAvgThroughput.Text = String.Empty;
                lblTotalErrors.Text = String.Empty;
                lblTotalPages.Text = String.Empty;
                lblAvgPages.Text = String.Empty;
                lblresponse200.Text = String.Empty;
                lblresponse300.Text = String.Empty;
                lblresponse400.Text = String.Empty;
                lblresponse500.Text = String.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void trvReportType_SelectedNodeChanged(object sender, Telerik.WinControls.UI.RadTreeViewEventArgs e)
        {
            try
            {
                btnExport.Visible = true;
                if (trvReportType.SelectedNode.Text == "1.Summary Report")
                {
                    btnExport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        GenerateReport(ddlReportNameReport.SelectedText.ToString());
                    }
                    else
                    {
                        ClearData();
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlSummaryReport.Visible = true;
                    pnlSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "2.Request ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetRequestSummaryReport(ddlReportNameReport.Text);
                       
                        grdRequestSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdRequestSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlRequestSummaryReport.Visible = true;
                    pnlRequestSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "3.Page ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetPageSummaryReport(ddlReportNameReport.Text);
                       
                        grdPageSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdPageSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlPageSummaryReport.Visible = true;
                    pnlPageSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "4.Container ResponseTime Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlTransactionSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        DataTable dt = new DataTable();
                        dt = _resultLog.GetContainerSummaryReport(ddlReportNameReport.Text);
                       
                        grdContainerSummaryReport.DataSource = dt;
                    }
                    else
                    {
                        grdContainerSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlContainerSummaryReport.Visible = true;
                    pnlContainerSummaryReport.Dock = DockStyle.Fill;
                }
                else if (trvReportType.SelectedNode.Text == "5.Transaction Summary Report")
                {
                    pnlSummaryReport.Visible = false;
                    pnlPageSummaryReport.Visible = false;
                    pnlRequestSummaryReport.Visible = false;
                    pnlContainerSummaryReport.Visible = false;
                    if (ddlReportNameReport.Text != "-Select-")
                    {
                        grdTransactionSummaryReport.DataSource = _resultLog.GetTransactionReport(ddlReportNameReport.Text);
                    }
                    else
                    {
                        grdTransactionSummaryReport.DataSource = String.Empty;
                        MessageBox.Show("Please Select Report Name");
                    }
                    pnlTransactionSummaryReport.Visible = true;
                    pnlTransactionSummaryReport.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        private void Export_TO_Excel(Telerik.WinControls.UI.RadGridView grdView, String grdName)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Exported Reports";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                int cols;
                FileStream stream = new FileStream(directoryPath + "\\" + ddlReportNameReport.Text + "--" + grdName + ".xls", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                stream.Close();
                StreamWriter wr = new StreamWriter(stream.Name);
                cols = grdView.Columns.Count;
                using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(wr))
                {
                    hw.Write("<table border=1>");
                    hw.Write("<tr>");
                    for (int i = 0; i < cols; i++)
                    {
                        hw.Write("<td>");
                        hw.Write("<b>" + grdView.Columns[i].HeaderText.ToString().ToUpper() + "</b>");
                        hw.Write("</td>");
                    }
                    hw.Write("</tr>");
                    for (int i = 0; i < (grdView.Rows.Count); i++)
                    {
                        hw.Write("<tr>");
                        for (int j = 0; j < cols; j++)
                        {
                            hw.Write("<td>");
                            if (grdView.Rows[i].Cells[j].Value != null)
                                hw.Write(grdView.Rows[i].Cells[j].Value);
                            hw.Write("</td>");
                        }
                        hw.Write("</tr>");
                    }
                    hw.Write("</table>");
                }
                /*for (int i = 0; i < cols; i++)
                {
                    wr.Write(grdView.Columns[i].HeaderText.ToString().ToUpper() + "\t");
                }
                wr.WriteLine();
                for (int i = 0; i < (grdView.Rows.Count); i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (grdView.Rows[i].Cells[j].Value != null)
                            wr.Write(grdView.Rows[i].Cells[j].Value + "\t");
                        else
                        {
                            wr.Write("\t");
                        }
                    }
                    wr.WriteLine();
                }*/
                wr.Close();
                Process.Start(@directoryPath + "\\" + ddlReportNameReport.Text + "--" + grdName + ".xls");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (trvReportType.SelectedNode.Text == "2.Request ResponseTime Summary Report")
                {
                    Export_TO_Excel(grdRequestSummaryReport, "Request_ResponseTime");
                }
                else if (trvReportType.SelectedNode.Text == "3.Page ResponseTime Summary Report")
                {
                    Export_TO_Excel(grdPageSummaryReport, "Page_ResponseTime");
                }
                else if (trvReportType.SelectedNode.Text == "4.Container ResponseTime Summary Report")
                {
                    Export_TO_Excel(grdContainerSummaryReport, "Container_ResponseTime");
                }
                else if (trvReportType.SelectedNode.Text == "5.Transaction Summary Report")
                {
                    Export_TO_Excel(grdTransactionSummaryReport, "Transaction_Summary");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReportName(string.Empty);
        }
  
        private void btnSummarReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlReportNameReport.Text != "-Select-")
                {
                    Process.Start(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + ddlReportNameReport.Text + "\\Report\\summary.xls");
                }
                else
                {
                    ClearData();
                    MessageBox.Show("Please Select Report Name");
                }
            }
            catch
            {

            }
        }

        /*private void grdContainerSummaryReport_ViewCellFormatting(object sender, CellFormattingEventArgs e)
         {
             if (e.CellElement is GridHeaderCellElement)
             {
                 e.CellElement.Font = new Font("Arial", 12f, FontStyle.Bold);
             }
         }*/
    }
}
