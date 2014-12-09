using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;


namespace AgentCore
{
    public class Agent
    {
        Utility constants = Utility.GetInstance();

        Dictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>();
        Dictionary<string, List<PerformanceCounter>> CountersAllInstance = new Dictionary<string, List<PerformanceCounter>>();
        string uid = string.Empty;
        string path = string.Empty;
        string dataSendUrl = string.Empty;
        string type = string.Empty;        string totalPhysicalMemory = "0";
        bool IsWindowsCounter = false;

        public Agent(XmlFileProccessor xml, bool isWindowsCounter)
        {
            IsWindowsCounter = isWindowsCounter;
            if (IsWindowsCounter == true) SetTotalPhysicalMemory();
            path = GetPath();
            dataSendUrl = path + "/collectCounters";
            if (xml.doc.SelectSingleNode("/root/counters").Attributes["uid"].Value == string.Empty || xml.doc.SelectSingleNode("/root/counters").Attributes["uid"].Value == "null")
            {
                path = path + "/getConfigurations";
                string data = string.Format("agent_type={0}&guid={1}", xml.doc.SelectSingleNode("/root/counters").Attributes["type"].Value, ConfigurationSettings.AppSettings["guid"]);
                data = constants.GetPageContent(path, data);
                uid = new Regex(".* \"uid\": \"(.*?)\"").Match(data).Groups[1].Value;
                xml.doc.SelectSingleNode("/root/counters").Attributes["uid"].Value = uid;
                xml.Save();
            }
            else
            {
                uid = xml.doc.SelectSingleNode("/root/counters").Attributes["uid"].Value;
            }
            type = xml.doc.SelectSingleNode("/root/counters").Attributes["type"].Value;
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
        public void SendCounter()
        {
            try
            {
                constants.GetPageContent(dataSendUrl, GetCountersValue());
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
            data.Append(1001).Append("=\"").Append(uid).Append("\",");
            data.Append(1002).Append("=\"").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("\"}").Append("&");
            data.Append("agent_type=").Append(type);
            return data.ToString();
        }
        private string GetPath()
        {
            return string.Format("{0}://{1}:{2}/{3}", ConfigurationSettings.AppSettings["protocol"], ConfigurationSettings.AppSettings["server"], ConfigurationSettings.AppSettings["port"], ConfigurationSettings.AppSettings["path"]);
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
