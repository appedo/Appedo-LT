using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;
namespace AppedoLT.Forms
{
    public partial class frmProxyConfiguration : Form
    {
        public frmProxyConfiguration()
        {
            InitializeComponent();
        }
        private void frmProxyConfiguration_Load(object sender, System.EventArgs e)
        {

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            txtProxyHost.Text = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyHost']").Attributes["value"].Value;
            txtProxyPort.Text = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyPort']").Attributes["value"].Value;
            txtProxyUser.Text = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='UserName']").Attributes["value"].Value;
            txtProxyPass.Text = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='Password']").Attributes["value"].Value;

            chkProxy.Checked = bool.Parse(xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='IsProxyEnabled']").Attributes["value"].Value);


        }
        private void btnProxyConfig_Click(object sender, EventArgs e)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyHost']").Attributes["value"].Value = txtProxyHost.Text;
            xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyPort']").Attributes["value"].Value = txtProxyPort.Text;
            xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='UserName']").Attributes["value"].Value = txtProxyUser.Text;
            xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='Password']").Attributes["value"].Value = txtProxyPass.Text;
            xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='IsProxyEnabled']").Attributes["value"].Value = chkProxy.Checked.ToString();
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("configuration/appSettings");
            //string msg = xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyHost']").Attributes["value"].Value+"\n"+xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='ProxyPort']").Attributes["value"].Value+"\n"+xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='UserName']").Attributes["value"].Value+"\n"+xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='Password']").Attributes["value"].Value+"\n"+xmlDoc.SelectSingleNode("//configuration/appSettings/add[@key='IsProxyEnabled']").Attributes["value"].Value;
            MessageBox.Show("Sucessfully Configured");
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

        

        
    }
}
