using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class frmSingleChart : Telerik.WinControls.UI.RadForm
    {
        public frmSingleChart(RadChart chart)
        {
            InitializeComponent();
            this.Text = chart.ChartTitle.TextBlock.Text;
           
            chart.Dock = DockStyle.Fill;
            this.Controls.Add(chart);
            //radChart1 = chart;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
