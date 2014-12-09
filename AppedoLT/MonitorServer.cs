using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using AppedoLT.DataAccessLayer;

namespace AppedoLT
{
   public class MonitorServer
    {
        string serverName = string.Empty;
        XmlNode _server = null;
        Timer timer = null;
        int interval = 0;
        int sample = 0;

        Dictionary<string, PerformanceCounter> counetrs = null;
        ResultLogMonitor result = ResultLogMonitor.GetInstance();

        public MonitorServer(XmlNode server)
        {
            _server = server;
            serverName = server.Attributes["ipaddress"].Value;
            timer = new Timer();
            interval=Convert.ToInt32(server.Attributes["interval"].Value);
            counetrs = new Dictionary<string, PerformanceCounter>();
            foreach (XmlNode counter in server.SelectNodes(".//counter"))
            {
                try
                {
                    PerformanceCounter cou = new PerformanceCounter(counter.Attributes["category"].Value, counter.Attributes["countername"].Value, counter.Attributes["instance"].Value, serverName);
                    counetrs.Add(counter.Attributes["counterid"].Value, cou);
                }
                catch (Exception ex)
                {

                }
            }
            timer.Enabled = true; 
            timer.Interval = 1000 * interval;
            timer.Tick += new EventHandler(timer_Tick);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            foreach (string key in counetrs.Keys)
            {
                try
                {
                    result.LogResult(key, sample.ToString(), counetrs[key].NextValue().ToString());
                }
                catch
                {

                }
            }
           sample+= interval;
        }
    }
}
