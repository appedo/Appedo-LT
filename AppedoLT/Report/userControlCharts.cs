using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using Telerik.Charting;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class userControlCharts : UserControl
    {
        String strYField = String.Empty;
        DataTable dtPages = new DataTable();
        RadChart chtUsers = new RadChart();
        private Result _resultLog = Result.GetInstance();
        public userControlCharts()
        {
            InitializeComponent();
          //  LoadReportName(string.Empty);
      
        }
        public void LoadReportName(string repoerName)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = _resultLog.GetReportNameList(repoerName);
                ddlReportNameReport.DataSource = dt.Copy();
                ddlReportNameReport.DisplayMember = "reportname";
                ddlChartName.Items.Clear();
                ddlChartName.Items.Insert(0, new RadComboBoxItem("-Select Chart-"));
                ddlChartName.Items.Insert(1, new RadComboBoxItem("Hits"));
                ddlChartName.Items.Insert(2, new RadComboBoxItem("AvgRequestResponse"));
                ddlChartName.Items.Insert(3, new RadComboBoxItem("ErrorCount"));
                ddlChartName.Items.Insert(4, new RadComboBoxItem("Throughput"));
                ddlChartName.Items.Insert(5, new RadComboBoxItem("Response time Against Load"));
                ddlChartName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
        private void btnShowReportReport_Click(object sender, EventArgs e)
        {
            if(chtUsers.Visible ==true) chtUsers.Visible = false;
            if (ddlReportNameReport.SelectedIndex != 0)
            {
                if (ddlChartName.SelectedIndex != 0)
                {
                    try
                    {
                        chtName.Visible = true;
                        DataTable dtTemp = new DataTable();
                        chtName.Clear();
                        chtName.Series.RemoveSeries();
                        chtName.DataSource = null;

                        #region XAxisSetting
                        chtName.PlotArea.XAxis.AxisLabel.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                        chtName.PlotArea.XAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.Green;
                        chtName.PlotArea.XAxis.AxisLabel.Visible = true;
                        #endregion

                        #region YAxisSetting
                        chtName.PlotArea.YAxis.AxisLabel.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                        chtName.PlotArea.YAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.BlueViolet;
                        chtName.PlotArea.YAxis.AxisLabel.Visible = true;
                        #endregion

                        #region YAxis2Setting
                        chtName.PlotArea.YAxis2.AxisLabel.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                        chtName.PlotArea.YAxis2.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.Black;
                        chtName.PlotArea.YAxis2.AxisLabel.Visible = true;
                        chtName.PlotArea.YAxis2.Visible = Telerik.Charting.Styles.ChartAxisVisibility.True;
                        chtName.PlotArea.YAxis2.YAxisType = ChartYAxisType.Secondary;
                        chtName.PlotArea.YAxis2.AxisLabel.TextBlock.Text = "Users";
                        #endregion
                        chtName.PlotArea.YAxis.AutoScale = true;

                        if (ddlChartName.Text == "Hits")
                        {
                            chtName.ChartTitle.TextBlock.Text = "Hits";
                            dtTemp = _resultLog.GetChartSummary(ddlReportNameReport.Text);
                            dtTemp.Columns.Remove("avgrequestresponse");
                            dtTemp.Columns.Remove("avgthroughput");
                            dtTemp.Columns.Remove("errorcount");

                            chtName.PlotArea.XAxis.LabelStep = Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) > 0 ? Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) : 1;
                            if (dtTemp.Rows.Count > 0)
                            {
                                strYField = "hitcount";
                                //if (dtTemp.Rows.Count > 0)
                                //{
                                //    double result = Convert.ToDouble(dtTemp.Compute("max(" + strYField + ")", ""));
                                //    if (chtName.PlotArea.YAxis.MaxValue < result)
                                //    {
                                //        chtName.PlotArea.YAxis.MaxValue = result;
                                //    }
                                //}     
                                                              chtName.DataSource = dtTemp;
                                chtName.PlotArea.XAxis.DataLabelsColumn = "scenariotime";
                                chtName.DataBound += new EventHandler<EventArgs>(chtName_DataBound);
                                chtName.DataBind();

                                chtName.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Scenario Time";
                                chtName.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Hit Count";
                                chtName.PlotArea.XAxis.Items.Clear();
                                for (int i = 0; i < dtTemp.Rows.Count; i++)
                                {
                                    ChartAxisItem axisItem = new ChartAxisItem(dtTemp.Rows[i]["scenariotime"].ToString());
                                    chtName.PlotArea.XAxis.Items.Add(axisItem);
                                }
                            }
                            else
                            {
                                chtName.Clear();
                                chtName.DataSource = null;
                            }
                        }
                        else if (ddlChartName.Text == "AvgRequestResponse")
                        {
                            chtName.ChartTitle.TextBlock.Text = "Avg Request Response Time";
                            dtTemp = _resultLog.GetChartSummary(ddlReportNameReport.Text);
                            dtTemp.Columns.Remove("hitcount");
                            dtTemp.Columns.Remove("avgthroughput");
                            dtTemp.Columns.Remove("errorcount");
                            chtName.PlotArea.XAxis.LabelStep = Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) > 0 ? Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) : 1;
                            if (dtTemp.Rows.Count > 0)
                            {
                                strYField = "avgresponse";
                                //if (dtTemp.Rows.Count > 0)
                                //{
                                //    double result = Convert.ToDouble(dtTemp.Compute("max(avgrequestresponse)", ""));
                                //    if (chtName.PlotArea.YAxis.MaxValue < result)
                                //    {
                                //        chtName.PlotArea.YAxis.MaxValue = result;
                                //    }
                                //}               
                                chtName.DataSource = dtTemp;
                                chtName.PlotArea.XAxis.DataLabelsColumn = "scenariotime";
                                chtName.DataBound += new EventHandler<EventArgs>(chtName_DataBound);
                                chtName.DataBind();
                                chtName.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Scenario Time";
                                chtName.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Avg Response(ms)";
                                chtName.PlotArea.XAxis.Items.Clear();
                                for (int i = 0; i < dtTemp.Rows.Count; i++)
                                {
                                    ChartAxisItem axisItem = new ChartAxisItem(dtTemp.Rows[i]["scenariotime"].ToString());
                                    chtName.PlotArea.XAxis.Items.Add(axisItem);
                                }
                            }
                            else
                            {
                                chtName.Clear();
                                chtName.DataSource = null;
                            }
                        }
                        else if (ddlChartName.Text == "ErrorCount")
                        {
                            chtName.ChartTitle.TextBlock.Text = "ErrorCount";
                            dtTemp = _resultLog.GetChartSummary(ddlReportNameReport.Text);
                            dtTemp.Columns.Remove("hitcount");
                            dtTemp.Columns.Remove("avgthroughput");
                            dtTemp.Columns.Remove("avgrequestresponse");
                            chtName.PlotArea.XAxis.LabelStep = Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) > 0 ? Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) : 1;
                            if (dtTemp.Rows.Count > 0)
                            {
                                strYField = "errorcount";
                                if (dtTemp.Rows.Count > 0)
                                {
                                    int result = Convert.ToInt32(dtTemp.Compute("max(" + strYField + ")", ""));
                                    if (result == 0)
                                    {
                                        chtName.PlotArea.YAxis.AutoScale = false;
                                    }
                                   
                                }
                               
                                chtName.DataSource = dtTemp;
                                chtName.PlotArea.XAxis.DataLabelsColumn = "scenariotime";
                                chtName.DataBound += new EventHandler<EventArgs>(chtName_DataBound);
                                chtName.DataBind();
                                chtName.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Scenario Time";
                                chtName.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Error Count";
                                chtName.PlotArea.XAxis.Items.Clear();
                              
                                for (int i = 0; i < dtTemp.Rows.Count; i++)
                                {
                                    ChartAxisItem axisItem = new ChartAxisItem(dtTemp.Rows[i]["scenariotime"].ToString());
                                    chtName.PlotArea.XAxis.Items.Add(axisItem);
                                }
                            }
                            else
                            {
                                chtName.Clear();
                                chtName.DataSource = null;
                            }
                        }
                        else if (ddlChartName.Text == "Throughput")
                        {
                            chtName.ChartTitle.TextBlock.Text = "Throughput";
                            dtTemp = _resultLog.GetChartSummary(ddlReportNameReport.Text);
                            dtTemp.Columns.Remove("hitcount");
                            dtTemp.Columns.Remove("avgrequestresponse");
                            dtTemp.Columns.Remove("errorcount");
                            chtName.PlotArea.XAxis.LabelStep = Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) > 0 ? Convert.ToInt32(Math.Round((Double)dtTemp.Rows.Count / 10, 0)) : 1;
                            if (dtTemp.Rows.Count > 0)
                            {
                                strYField = "throughput";
                                //if (dtTemp.Rows.Count > 0)
                                //{
                                //    double result = Convert.ToDouble(dtTemp.Compute("max(avgthroughput)", ""));
                                //    if (chtName.PlotArea.YAxis.MaxValue < result)
                                //    {
                                //        chtName.PlotArea.YAxis.MaxValue = result;
                                //    }
                                //}
                                chtName.DataSource = dtTemp;
                                chtName.PlotArea.XAxis.DataLabelsColumn = "scenariotime";
                                chtName.DataBound += new EventHandler<EventArgs>(chtName_DataBound);
                                chtName.DataBind();
                                chtName.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Scenario Time";
                                chtName.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Throughput(Mbps)";
                                chtName.PlotArea.XAxis.Items.Clear();
                                for (int i = 0; i < dtTemp.Rows.Count; i++)
                                {
                                    ChartAxisItem axisItem = new ChartAxisItem(dtTemp.Rows[i]["scenariotime"].ToString());
                                    chtName.PlotArea.XAxis.Items.Add(axisItem);
                                }
                            }
                            else
                            {
                                chtName.Clear();
                                chtName.DataSource = null;
                            }
                        }
                      
                        else if (ddlChartName.Text == "Response time Against Load")
                        {
                            chtUsers.Visible = true;
                            CreateChart();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                }
                else
                {
                    chtName.ChartTitle.TextBlock.Text = String.Empty;
                    chtName.Clear();
                    chtName.DataSource = null;
                    MessageBox.Show("Please select chart name");
                }
            }
            else
            {
                chtName.ChartTitle.TextBlock.Text = String.Empty;
                chtName.Clear();
                chtName.DataSource = null;
                MessageBox.Show("Please select report name");
            }
        }

        private static DataTable CreateTableFromData(int[][] data)
        {
            int columnCount = data.Length;
            int recordCount = int.MaxValue;

            if (columnCount <= 0) return null;

            DataTable table = new DataTable();

            for (int i = 0; i < columnCount; i++)
            {
                table.Columns.Add(string.Format("data_{0}", i));
                recordCount = Math.Min(recordCount, data[i].Length);
            }

            if (recordCount <= 0) return null;


            for (int i = 0; i < recordCount; i++)
            {
                DataRow row = table.NewRow();

                for (int j = 0; j < columnCount; j++)
                    row[string.Format("data_{0}", j)] = data[j][i];

                table.Rows.Add(row);
            }

            return table;
        }
        void chtName_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (chtName.DataSource != null)
                {
                   
                    chtName.Series[0].YAxisType = ChartYAxisType.Secondary;
                    chtName.Series[0].DataYColumn = "usercount";
                    chtName.Series[0].Type = ChartSeriesType.Line;
                    chtName.Series[0].Appearance.LabelAppearance.Visible = false;
                    chtName.Series[0].Appearance.LineSeriesAppearance.Color = System.Drawing.Color.Black;

                    chtName.Series[1].DataYColumn = strYField;
                    chtName.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 20;
                    chtName.Series[1].Type = ChartSeriesType.Line;
                    chtName.Series[1].Appearance.LabelAppearance.Visible = false;
                    chtName.Series[1].Appearance.LineSeriesAppearance.Color = System.Drawing.Color.BlueViolet;
                    chtName.Series[1].YAxisType = ChartYAxisType.Primary;

                    chtName.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
                    chtName.PlotArea.Appearance.Dimensions.Margins.Bottom = Telerik.Charting.Styles.Unit.Percentage(25);
                    chtName.PlotArea.Appearance.Dimensions.Margins.Left = Telerik.Charting.Styles.Unit.Percentage(20);
                    this.AutoSize = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        void chtNameMultiSeries_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (chtName.DataSource != null)
                {
                    if (chtName.Series.Count > dtPages.Rows.Count)
                    {
                        chtName.Series.RemoveAt(0);
                    }
                    String[] colorArray = { "#FFCC66", "#3366FF", "#C3159D", "#15C39A", "#80C315", "#C3A015", "#1538C3", "#9515C3" };
                    for (int j = 0; j < Convert.ToInt32(dtPages.Rows.Count); j++)
                    {
                        chtName.Series[j].DataYColumn = "page-" + dtPages.Rows[j]["Page"].ToString();
                        chtName.Series[j].Type = ChartSeriesType.Line;
                        chtName.Series[j].Appearance.LabelAppearance.Visible = false;
                        chtName.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 20;
                        chtName.Series[j].Appearance.LineSeriesAppearance.Color = System.Drawing.ColorTranslator.FromHtml(colorArray[j]);
                        chtName.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
                        chtName.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.BlueViolet;
                        chtName.PlotArea.Appearance.Dimensions.Margins.Bottom = Telerik.Charting.Styles.Unit.Percentage(25);
                        chtName.PlotArea.Appearance.Dimensions.Margins.Left = Telerik.Charting.Styles.Unit.Percentage(20);
                        this.AutoSize = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (ddlReportNameReport.SelectedIndex != 0)
            {
                if (ddlChartName.SelectedIndex != 0)
                {
                    if (chtUsers.Visible == true && chtUsers.ChartTitle.TextBlock.Text != String.Empty)
                    {
                        try
                        {
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                            saveFileDialog1.FileName = chtUsers.ChartTitle.TextBlock.Text;
                            saveFileDialog1.Filter = "JPEG Image (.jpeg)|*.jpeg |Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Wmf Image (.wmf)|*.wmf";
                            DialogResult result = saveFileDialog1.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                chtUsers.Save(saveFileDialog1.FileName);
                                Process.Start(@saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                    else if (chtName.ChartTitle.TextBlock.Text != String.Empty)
                    {
                        try
                        {
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                            saveFileDialog1.FileName = chtName.ChartTitle.TextBlock.Text;
                            saveFileDialog1.Filter = "JPEG Image (.jpeg)|*.jpeg |Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif |Png Image (.png)|*.png |Tiff Image (.tiff)|*.tiff |Wmf Image (.wmf)|*.wmf";
                            DialogResult result = saveFileDialog1.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                chtName.Save(saveFileDialog1.FileName);
                                Process.Start(@saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }

                    else
                        MessageBox.Show("Please Select View Graph");
                }
                else
                    MessageBox.Show("Please Select Chart Name");
            }
            else
                MessageBox.Show("Please Select Report Name");
        }
        private void CreateChart()
        {
            try
            {
                chtUsers.Clear();
                chtUsers.DataSource = null;

                DataTable dtTemp1 = _resultLog.GetAvgUsers(ddlReportNameReport.Text);
                chtName.Visible = false;
                chtUsers.DefaultType = Telerik.Charting.ChartSeriesType.Line;
                chtUsers.Size = chtName.Size;
                chtUsers.Anchor = chtName.Anchor;
                chtUsers.Parent = chtName.Parent;
                chtUsers.Location = chtName.Location;
                chtUsers.Skin = chtName.Skin;
                chtUsers.Legend.Appearance.Visible = false;
                chtUsers.Legend.Visible = false;
                chtUsers.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.Color.White;
                chtUsers.PlotArea.Appearance.FillStyle.SecondColor = System.Drawing.Color.White;
                chtUsers.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
                chtUsers.PlotArea.XAxis.Appearance.TextAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
                chtUsers.PlotArea.XAxis.Appearance.Visible = Telerik.Charting.Styles.ChartAxisVisibility.True;
                chtUsers.PlotArea.XAxis.LayoutMode = Telerik.Charting.Styles.ChartAxisLayoutMode.Inside;
                chtUsers.PlotArea.XAxis.MinValue = 1;
                chtUsers.PlotArea.XAxis.Visible = Telerik.Charting.Styles.ChartAxisVisibility.True;
                chtUsers.PlotArea.YAxis.AxisMode = Telerik.Charting.ChartYAxisMode.Extended;
                chtUsers.PlotArea.YAxis.MaxValue = 120;
                chtUsers.PlotArea.YAxis.Step = 20;
                chtUsers.ChartTitle.TextBlock.Text = "Response time Against Load";
                pnlChart.Controls.Remove(chtUsers);
                pnlChart.Controls.Add(chtUsers);
                chtUsers.Visible = true;
                chtUsers.PlotArea.XAxis.AxisLabel.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                chtUsers.PlotArea.XAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.Green;
                chtUsers.PlotArea.YAxis.AxisLabel.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                chtUsers.PlotArea.YAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.BlueViolet;
                chtUsers.PlotArea.XAxis.AxisLabel.Visible = true;
                chtUsers.PlotArea.YAxis.AxisLabel.Visible = true;
                chtUsers.PlotArea.XAxis.LabelStep = Convert.ToInt32(Math.Round((Double)dtTemp1.Rows.Count / 10, 0)) > 0 ? Convert.ToInt32(Math.Round((Double)dtTemp1.Rows.Count / 10, 0)) : 1;
                if (dtTemp1.Rows.Count > 0)
                {
                    strYField = "avg_response";
                    chtUsers.DataSource = dtTemp1;
                    chtUsers.PlotArea.XAxis.DataLabelsColumn = "usercount";
                    chtUsers.DataBound += new EventHandler<EventArgs>(chtUsers_DataBound);
                    chtUsers.DataBind();
                    chtUsers.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Users";
                    chtUsers.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Response Time(s)";
                }
                else
                {
                    chtUsers.Clear();
                    chtUsers.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        void chtUsers_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (chtUsers.DataSource != null)
                {
                    if (chtUsers.Series.Count > 1)
                        chtUsers.Series.RemoveAt(0);
                    chtUsers.Series[0].DataYColumn = strYField;
                    chtUsers.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 20;
                    chtUsers.Series[0].Type = ChartSeriesType.Line;
                    chtUsers.Series[0].Appearance.LabelAppearance.Visible = false;
                    chtUsers.Series[0].Appearance.LineSeriesAppearance.Color = System.Drawing.Color.BlueViolet;
                    chtUsers.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
                    chtUsers.PlotArea.Appearance.Dimensions.Margins.Bottom = Telerik.Charting.Styles.Unit.Percentage(25);
                    chtUsers.PlotArea.Appearance.Dimensions.Margins.Left = Telerik.Charting.Styles.Unit.Percentage(20);
                    this.AutoSize = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

    }
}
