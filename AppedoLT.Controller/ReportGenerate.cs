using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLTController
{
    public partial class ReportGenerate : Form
    {
        public ReportGenerate()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            try
            {
                ReportMaster mas = new ReportMaster(textBox1.Text);
                mas.Executequery(textBox1.Text, AppedoLT.Core.Constants.GetInstance().GetQuery(textBox1.Text, ControllerXml.GetInstance().doc));
                XmlNode runNode = ControllerXml.GetInstance().doc.SelectSingleNode("//run[@reportname='" + textBox1.Text + "']");
                Result.GetInstance().GetSummaryReportByScript(textBox1.Text, runNode);
            }
            catch (Exception ex)
            {

            }
        }
       
    }
}
