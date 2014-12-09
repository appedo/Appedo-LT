using AgentCore;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MSWindowsCounterAgent
{
    public partial class frmMSWindowsCounterAgent : Form
    {
        NotifyIcon ni = new NotifyIcon();
        Thread DoWorkThread = null;
        MSWindowsCounterXml counterXML = MSWindowsCounterXml.GetInstance();
        Agent agent = null;
        string trayText = "Appedo MS Windows Agent.";
        string trayTipText = "Appedo MS Windows Agent started.";

        public frmMSWindowsCounterAgent()
        {
            InitializeComponent();
            try
            {
                try
                {
                    agent = new Agent(counterXML,true);
                }
                catch(Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    MessageBox.Show("Unable to start agent. \r\nError:" + ex.Message);
                    Application.Exit();
                }
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
                       Thread.Sleep(30000);
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
