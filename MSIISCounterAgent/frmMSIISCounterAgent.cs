using AgentCore;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace MSIISCounterAgent
{
    public partial class frmMSIISCounterAgent : Form
    {
        NotifyIcon ni = new NotifyIcon();
        Thread DoWorkThread = null;
        MSIISCounterXml counterXML = MSIISCounterXml.GetInstance();
        Agent agent = null;
        string trayText = "Appedo MS IIS Agent.";
        string trayTipText = "Appedo MS IIS Agent started.";

        public frmMSIISCounterAgent()
        {
            InitializeComponent();
            try
            {
                string type = System.Configuration.ConfigurationManager.AppSettings["type"];
                string guid = System.Configuration.ConfigurationManager.AppSettings["guid"];
                if (guid == string.Empty)
                {
                    MessageBox.Show("GUID is missing. Please download again.");
                    Environment.Exit(1);
                }
                else
                {
                    agent = new Agent(counterXML, true, guid, type);
                    DoWorkThread = new Thread(new ThreadStart(DoWork));
                    DoWorkThread.Start();
                    ni.Icon = new Form().Icon;
                    ni.Text = trayText;
                    ni.Visible = true;
                    ni.ContextMenuStrip = contextMenuStrip1;
                    ni.BalloonTipText = trayTipText;
                    ni.ShowBalloonTip(1000);
                    ni.ContextMenuStrip = contextMenuStrip1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void DoWork()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        agent.SendCounter();
                        Thread.Sleep(20000);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Environment.Exit(1);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
    }
}
