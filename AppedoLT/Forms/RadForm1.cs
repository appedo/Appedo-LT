using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.IO;
using AppedoLT.Core;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using AppedoLT.DataAccessLayer;

namespace AppedoLT
{
    public partial class RadForm1 : Telerik.WinControls.UI.RadForm
    {
        Constants constants = Constants.GetInstance();
        public RadForm1()
        {
            InitializeComponent();
          
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            Result _resultLog = Result.GetInstance();
            dt = _resultLog.GetReportData("VVVVV");
            // dt.Columns.RemoveAt(0);
            // dt.Columns.RemoveAt(0);
            //grdvData.DataSource = dt.Copy();
            this.radGridView1.DataSource = dt.Copy();
            this.radGridView1.EnableFiltering = true;
            //this.radGridView1.MasterTemplate.ShowHeaderCellButtons = true;
            //this.radGridView1.MasterTemplate.ShowFilteringRow = false;
            radGridView1.ThemeName = "Breeze";
            MessageBox.Show("completed");

        }
    }
}
