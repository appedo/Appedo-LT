using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web.UI;

namespace AgentCore
{
    public class Agent
    {
        Utility constants = Utility.GetInstance();
        
        Dictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>();
        Dictionary<string, List<PerformanceCounter>> CountersAllInstance = new Dictionary<string, List<PerformanceCounter>>();
        private string _guid = string.Empty;
        private string _type = string.Empty;
        string path = string.Empty;
        string dataSendUrl = string.Empty;
      
        string totalPhysicalMemory = "0";
        bool IsWindowsCounter = false;
       
        
        public Agent(XmlFileProccessor xml, bool isWindowsCounter,string guid,string type)
        {
            IsWindowsCounter = isWindowsCounter;
            _guid = guid;
            _type=type;
            if (IsWindowsCounter == true) SetTotalPhysicalMemory();
            path = GetPath();
            dataSendUrl = path + "/collectCounters";
            foreach (XmlNode counterWithIndance in xml.doc.SelectNodes("/root/counters/counter"))
            {
                try
                {
                    if (counterWithIndance.Attributes["instance"].Value.Trim() == string.Empty)
                    {
                        PerformanceCounter counter = new PerformanceCounter(counterWithIndance.Attributes["category"].Value, counterWithIndance.Attributes["countername"].Value);
                        counter.NextValue();
                        Counters.Add(counterWithIndance.Attributes["id"].Value, counter);
                    }
                    else if (counterWithIndance.Attributes["instance"].Value.Trim() == "ALL")
                    {
                        List<PerformanceCounter> countersList = new List<PerformanceCounter>();
                        System.Diagnostics.PerformanceCounterCategory mycat = new System.Diagnostics.PerformanceCounterCategory(counterWithIndance.Attributes["category"].Value);
                        foreach (string instanceName in mycat.GetInstanceNames())
                        {
                            try
                            {
                                PerformanceCounter counter = new PerformanceCounter(counterWithIndance.Attributes["category"].Value, counterWithIndance.Attributes["countername"].Value, instanceName);
                                counter.NextValue();
                                countersList.Add(counter);
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                            }
                        }
                        if (countersList.Count > 0)
                        {
                            CountersAllInstance.Add(counterWithIndance.Attributes["id"].Value, countersList);
                        }
                    }
                    else
                    {
                        PerformanceCounter counter = new PerformanceCounter(counterWithIndance.Attributes["category"].Value, counterWithIndance.Attributes["countername"].Value, counterWithIndance.Attributes["instance"].Value);
                        counter.NextValue();
                        Counters.Add(counterWithIndance.Attributes["id"].Value, counter);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }

        }
        public Agent(string guid, string type)
        {
            _guid = guid;
            _type = type;
            dataSendUrl = GetPath() + "/collectCounters";
            CountersDetail detail = Utility.GetInstance().Deserialise<CountersDetail>(constants.GetPageContent(path = GetPath() + "/getConfigurations", string.Format("guid={0}&command=firstrequest", guid)));
            SetCounterList(detail);
        }
        public void SendCounter()
        {
            try
            {
                string responseStr= constants.GetPageContent(dataSendUrl, GetCountersValue());
                if(responseStr.Contains("newCounterSet"))
                {
                    CountersDetail detail = Utility.GetInstance().Deserialise<CountersDetail>(responseStr);
                    SetCounterList(detail);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        public void SaveCounters()
        {
            try
            {
                ExceptionHandler.WritetoEventLog(GetCountersValue());
            }
            catch
            {

            }
        }
        public void SetCounterList(CountersDetail details)
        {
          
            string[] detail = null;
            Counters.Clear();
            foreach (newCounterSet counterWithIndance in details.newCounterSet)
            {
                try
                {
                    detail = counterWithIndance.query.Split(',');
                    if (detail[0].ToLower() == "false")
                    {
                        PerformanceCounter counter = new PerformanceCounter(detail[1], detail[2]);
                        counter.NextValue();
                        Counters.Add(counterWithIndance.counter_id.ToString(), counter);
                    }
                    else if (detail[3].ToLower().StartsWith("_"))
                    {
                        PerformanceCounter counter = new PerformanceCounter(detail[1], detail[2], detail[3]);
                        counter.NextValue();
                        Counters.Add(counterWithIndance.counter_id.ToString(), counter);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
        }
        private string GetCountersValue()
        {
            StringBuilder data = new StringBuilder();
            data.Append("counter_params_json=");
            data.Append("{");
            foreach (string key in Counters.Keys)
            {
                try
                {
                    data.Append(key).Append("=").Append(Convert.ToDecimal(Counters[key].NextValue()).ToString("0.00")).Append(",");
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }

            }
            if (CountersAllInstance.Count > 0)
            {
                float result;
                foreach (string key in CountersAllInstance.Keys)
                {
                    result = 0;
                    foreach (PerformanceCounter counter in CountersAllInstance[key])
                    {
                        try
                        {
                            result += counter.NextValue();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }

                    }
                    data.Append(key).Append("=").Append(Convert.ToDecimal(result).ToString("0.00")).Append(",");
                }
            }
            if (IsWindowsCounter == true) data.Append("1000015").Append("=").Append(totalPhysicalMemory).Append(",");
            data.Append(1001).Append("=\"").Append(_guid).Append("\",");
            data.Append(1002).Append("=\"").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("\"}").Append("&");
            data.Append("agent_type=").Append(_type);
            return data.ToString();
        }
        private string GetPath()
        {
            return string.Format("{0}://{1}:{2}/{3}", System.Configuration.ConfigurationManager.AppSettings["protocol"], System.Configuration.ConfigurationManager.AppSettings["server"], System.Configuration.ConfigurationManager.AppSettings["port"], System.Configuration.ConfigurationManager.AppSettings["path"]);
        }
        private void SetTotalPhysicalMemory()
        {
            try
            {
                totalPhysicalMemory = Convert.ToDecimal(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024).ToString("0.00");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
    }
}
