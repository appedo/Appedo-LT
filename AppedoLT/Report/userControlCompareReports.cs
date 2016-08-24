using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using AppedoLT.Report;

namespace AppedoLT
{
    public partial class userControlCompareReports : UserControl
    {
      //  private 
        public userControlCompareReports()
        {
            InitializeComponent();
            radSplitContainer1.Splitters[0].Fixed = true;
            reports = new List<string>();
            this.radGridReport.SelectionChanged += new System.EventHandler(this.radGridView1_SelectionChanged);
        }

        private Result _resultLog = Result.GetInstance();
    
        public void LoadReportName(string reportName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = _resultLog.GetCompareReportList(reportName);
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

        private void LoadScripts()
        {
            try
            {
                List<Script> scripts = new List<Script>();
                scripts.Add( new Script() { Id = "0", Name = "-- Select --" } );
                cmbScript.Items.Clear();
                Dictionary<string, string> scriptObjs = VuscriptXml.GetScriptidAndName();
                foreach (KeyValuePair<string, string> script in scriptObjs)
                {
                    scripts.Add(new Script() { Id = script.Key, Name = script.Value });
                }
                cmbScript.DataSource = scripts;
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
        
        private void LoadResult(string reportName)
        {
            try
            {
                btnSave.Visible = false;

                string type = string.Empty;
                brwReportView.Navigate("about:blank");
                HtmlDocument doc = brwReportView.Document;
                doc.Write(String.Empty);

                string filePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\CompareReports\\" + reportName + "\\summary.html";
                brwReportView.Url = new Uri(filePath);
                brwReportView.Update();
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
            if(this.radGridReport.SelectedRows.Count>0)
                LoadResult((string)this.radGridReport.SelectedRows[0].Cells[0].Value);
        }

        private void cmbScript_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbReport1.Items.Clear();
            cmbReport2.Items.Clear();
            cmbReport3.Items.Clear();

            if (cmbScript.SelectedIndex == 0)
            {
                HideAllReportSelection();
            }
            else
            {
                lblReport1.Visible = true;
                cmbReport1.Visible = true;

                GetReports(cmbScript.SelectedValue.ToString());
            }
        }

        private void HideAllReportSelection()
        {
            lblReport1.Visible = false;
            lblReport2.Visible = false;
            lblReport3.Visible = false;
            cmbReport1.Visible = false;
            cmbReport2.Visible = false;
            cmbReport3.Visible = false;
            btnCompare.Visible = false;
        }

        private void userControlCompareReports_Load(object sender, EventArgs e)
        {
            LoadReportName(string.Empty);
            LoadScripts();
        }

        List<string> reports;
        private void GetReports(string scriptId)
        {
            try
            {
                reports = new List<string>();
                XmlNode runs = RepositoryXml.GetInstance().Doc.SelectSingleNode("//runs");
                if (runs == null)
                    return;

                string dataFolderPath = Constants.GetInstance().DataFolderPath;

                XmlNodeList nodes = runs.SelectNodes(string.Format("//run//scripts//script[@id = '{0}']", scriptId));
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode reportNode = nodes.Item(i).ParentNode.ParentNode;
                    string reportName = reportNode.Attributes["reportname"].Value;
                    if (!reports.Contains(reportName))
                    {
                        if (Directory.Exists(dataFolderPath + "\\" + reportName))
                        {
                            reports.Add(reportName);
                        }
                    }
                }
            }
            catch (Exception excp)
            { }

            if (reports.Count == 0)
            {
                MessageBox.Show("There is no report generated for the selected script, Please select some other script", "AppedoLT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HideAllReportSelection();
                cmbScript.Focus();
                return;
            }

            if (reports.Count == 1)
            {
                MessageBox.Show("Comparision can't be made as there is only one report generated for the selected script, Please select some other script", "AppedoLT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HideAllReportSelection();
                cmbScript.Focus();
                return;
            }

            cmbReport1.Items.Clear();
            cmbReport1.Items.Add("-- Select --");
            foreach (string report in reports)
            {
                cmbReport1.Items.Add(report);
            }
            cmbReport1.SelectedIndex = 0;
            cmbReport1.Focus();
        }

        private void cmbReport1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReport1.SelectedIndex == 0)
            {
                lblReport2.Visible = false;
                lblReport3.Visible = false;
                cmbReport2.Visible = false;
                cmbReport3.Visible = false;
                btnCompare.Visible = false;
            }
            else
            {
                cmbReport2.Items.Clear();
                cmbReport2.Items.Add("-- Select --");
                foreach (string report in reports)
                {
                    if (report != cmbReport1.SelectedItem)
                    {
                        cmbReport2.Items.Add(report);
                    }
                }

                cmbReport2.SelectedIndex = 0;
                cmbReport2.Focus();

                lblReport2.Visible = true;
                cmbReport2.Visible = true;
                btnCompare.Visible = false;
            }
        }

        private void cmbReport2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReport2.SelectedIndex == 0 || cmbReport2.Items.Count <= 2)
            {
                lblReport3.Visible = false;
                cmbReport3.Visible = false;
                btnCompare.Visible = false;
            }
            else
            {
                cmbReport3.Items.Clear();
                cmbReport3.Items.Add("-- Select --");
                foreach (string report in reports)
                {
                    if (report != cmbReport1.SelectedItem && report != cmbReport2.SelectedItem)
                    {
                        cmbReport3.Items.Add(report);
                    }
                }

                cmbReport3.SelectedIndex = 0;
                cmbReport3.Focus();

                lblReport3.Visible = true;
                cmbReport3.Visible = true;
                btnCompare.Visible = true;
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            try
            {
                string scriptId = cmbScript.SelectedValue.ToString();
                string report1 = cmbReport1.SelectedItem.ToString();
                string report2 = cmbReport2.SelectedItem.ToString();
                string report3 = cmbReport3.SelectedItem.ToString();
                if (cmbReport3.SelectedIndex == 0)
                    report3 = string.Empty;

                Result.GetInstance().GenerateCompareReport(scriptId, ((Script)cmbScript.SelectedItem).Name, report1, report2, report3);
                LoadResult("Current");
                btnSave.Visible = true;
            }
            catch (Exception excp)
            {
                MessageBox.Show("Error occurred during report generation", "AppedoLT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            frmReportName reportNameDlg = new frmReportName();
            string reportName = cmbReport1.SelectedItem.ToString() + " - " + cmbReport2.SelectedItem.ToString();
            if (cmbReport3.SelectedIndex > 0)
            {
                reportName += " - " + cmbReport3.SelectedItem.ToString();
            }
            reportNameDlg.ReportName = reportName;
            if (reportNameDlg.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(Constants.GetInstance().CompareReportsFolderPath + "\\Current");

                string compareReportFolder = Constants.GetInstance().CompareReportsFolderPath + "\\" + reportNameDlg.ReportName;
                Directory.CreateDirectory(compareReportFolder);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(compareReportFolder, file.Name), true);
                }
                LoadReportName(string.Empty);
                LoadResult(reportNameDlg.ReportName);
                MessageBox.Show("Report saved successfully", "AppedoLT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    class Script
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
