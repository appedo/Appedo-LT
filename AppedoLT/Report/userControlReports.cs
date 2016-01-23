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
      //  private 
        public userControlReports()
        {
            InitializeComponent();
            //brwReportView.IsWebBrowserContextMenuEnabled = false;
            //brwReportView.ContextMenuStrip = cntmSave;
            LoadReportName(string.Empty);
            // this event will fire when user select the report 
            this.radGridReport.SelectionChanged += new System.EventHandler(this.radGridView1_SelectionChanged);
          
        }
        private Result _resultLog = Result.GetInstance();
    
        public void LoadReportName(string reportName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = _resultLog.GetReportNameList(reportName);
              // radListBox1.DisplayMember = "reportname";
            //  radListBox1.DataSource = dt.Copy();
                this.radGridReport.DataSource = dt.Copy();
                this.radGridReport.Columns[0].Width = 170;
                this.radGridReport.Columns[1].Width = 110;
                this.radGridReport.MultiSelect = false;

                LoadResult((string)this.radGridReport.CurrentRow.Cells[0].Value);
              
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        public Boolean IsManyUsers()
        {
            Boolean boolShowAvg = false;
            //System.Data.DataTable dt = new System.Data.DataTable();
            //dt = _resultLog.GetSummaryReport(ddlReportNameReport.Text);
            //if (dt.Rows.Count > 0)
            //{
            //    if (Convert.ToInt32(dt.Rows[0]["user_count"]) > 1)
            //        boolShowAvg = true;
            //}
            return boolShowAvg;
        }
  
  

     

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            try
            {
                //if (trvReportType.SelectedNode.Text == "2.Request ResponseTime Summary Report")
                //{
                //    Export_TO_Excel(grdRequestSummaryReport, "Request_ResponseTime");
                //}
                //else if (trvReportType.SelectedNode.Text == "3.Page ResponseTime Summary Report")
                //{
                //    Export_TO_Excel(grdPageSummaryReport, "Page_ResponseTime");
                //}
                //else if (trvReportType.SelectedNode.Text == "4.Container ResponseTime Summary Report")
                //{
                //    Export_TO_Excel(grdContainerSummaryReport, "Container_ResponseTime");
                //}
                //else if (trvReportType.SelectedNode.Text == "5.Transaction Summary Report")
                //{
                //    Export_TO_Excel(grdTransactionSummaryReport, "Transaction_Summary");
                //}
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
            //try
            //{
            //    if (ddlReportNameReport.Text != "-Select-")
            //    {
            //        Process.Start(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + ddlReportNameReport.Text + "\\Report\\summary.xls");
            //    }
            //    else
            //    {
            //        ClearData();
            //        MessageBox.Show("Please Select Report Name");
            //    }
            //}
            //catch
            //{

            //}
        }

        private void btnLogReport_Click(object sender, EventArgs e)
        {
            //    try
            //    {
            //        if (ddlReportNameReport.Text != "-Select-")
            //        {
            //            Process.Start(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + ddlReportNameReport.Text + "\\Report\\log.html");
            //        }
            //        else
            //        {
            //            ClearData();
            //            MessageBox.Show("Please Select Report Name");
            //        }
            //    }
            //    catch
            //    {

            //    }
        }

        private void btnErrorReport_Click(object sender, EventArgs e)
        {
            try
            {
                //if (ddlReportNameReport.Text != "-Select-")
                //{
                //    Process.Start(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + ddlReportNameReport.Text + "\\Report\\error.html");
                //}
                //else
                //{
                //    ClearData();
                //    MessageBox.Show("Please Select Report Name");
                //}
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



        //private void ddl_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (ddlReportNameReport.SelectedIndex > 0 && ddlType.SelectedIndex > 0)
        //        {
        //            string type = string.Empty;
        //            if (ddlType.SelectedIndex == 1) type = "summary.html";
        //            else if (ddlType.SelectedIndex == 2) type = "error.html";
        //            else if (ddlType.SelectedIndex == 3) type = "log.html";
        //            string filePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + ddlReportNameReport.Text + "\\Report\\" + type;
        //            if (File.Exists(filePath))
        //            {
        //                using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
        //                {
        //                    brwReportView.DocumentText = reader.ReadToEnd();
        //                }
        //            }
        //            else
        //            {
        //                brwReportView.DocumentText = string.Empty;
        //            }
        //        }

        //        else
        //        {
        //            brwReportView.DocumentText = string.Empty;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        brwReportView.DocumentText = string.Empty;
        //    }

        //}

        private void ddlType_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void radListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          //  LoadResult(radListBox1.SelectedText);
        }

        private void tabsReport_TabSelected(object sender, Telerik.WinControls.UI.TabEventArgs args)
        {
           
        }
        private void LoadResult(string reportName)
        {
            try
            {

                string type = string.Empty;

                string filePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportName + "\\Report\\summary.html";
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
                    {
                        brwReportView.DocumentText = reader.ReadToEnd();
                    }
                }
                else
                {
                    brwReportView.DocumentText = string.Empty;
                }

                filePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportName + "\\Report\\error.html";
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
                    {
                        brwErrors.DocumentText = reader.ReadToEnd();
                    }
                }
                else
                {
                    brwErrors.DocumentText = string.Empty;
                }

                filePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportName + "\\Report\\log.html";
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
                    {
                        brwLogs.DocumentText = reader.ReadToEnd();
                    }
                }
                else
                {
                    brwLogs.DocumentText = string.Empty;
                }

            }
            catch (Exception ex)
            {
                brwReportView.DocumentText = string.Empty;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void brwReportView_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        
        /// <summary>
        /// This event function will trigger when the user select the report 
        /// and the it will generate report on right hand side panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radGridView1_SelectionChanged(object sender, EventArgs e)
        {            
            LoadResult((string)this.radGridReport.SelectedRows[0].Cells[0].Value);
        }

       
    }
}
