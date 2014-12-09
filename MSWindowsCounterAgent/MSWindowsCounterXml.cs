using AgentCore;
using System;
using System.IO;
using System.Text;

namespace MSWindowsCounterAgent
{
    class MSWindowsCounterXml:XmlFileProccessor
    {
        private MSWindowsCounterXml()
        {
            try
            {
                string file = Utility.GetInstance().ExecutingAssemblyLocation + "\\MSWindowsCounterXml.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append(@"<counters uid=""""  type=""WINDOWS"">");
                        xml.Append(@"<counter id=""1000001"" category=""PhysicalDisk"" countername=""% Disk Time"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000002"" category=""PhysicalDisk"" countername=""Disk Reads/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000003"" category=""PhysicalDisk"" countername=""Current Disk Queue Length"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000004"" category=""Memory"" countername=""Available MBytes"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""1000005"" category=""Memory"" countername=""Pages/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""1000006"" category=""Paging File"" countername=""% Usage"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000007"" category=""Processor"" countername=""% Processor Time"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000008"" category=""Processor"" countername=""Interrupts/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000009"" category=""System"" countername=""Processor Queue Length"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""1000010"" category=""PhysicalDisk"" countername=""Disk Read Bytes/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000011"" category=""PhysicalDisk"" countername=""Disk Write Bytes/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000012"" category=""Memory"" countername=""Page Faults/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""1000013"" category=""Network Interface"" countername=""Bytes Received/sec"" instance=""ALL"" scale=""1""/>");
                        xml.Append(@"<counter id=""1000014"" category=""Network Interface"" countername=""Bytes Sent/sec"" instance=""ALL"" scale=""1""/>");
                        xml.Append("</counters>");
                        xml.Append("</root>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                doc.Load(file);
                //_serverId = Convert.ToInt32(doc.SelectSingleNode("root/uniquenumbers/server").Attributes["lastcreatedid"].Value);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static MSWindowsCounterXml instance;
        public static MSWindowsCounterXml GetInstance()
        {
            if (instance == null)
            {
                instance = new MSWindowsCounterXml();
            }
            return instance;
        }
        public override void Save()
        {
            doc.Save(Utility.GetInstance().ExecutingAssemblyLocation + "\\MSWindowsCounterXml.xml");
        }
        
        //public XmlNode CreateCounter(string category,string instance,string counterName)
        //{
        //    XmlNode counter = doc.CreateElement("counter");
           
        //    counter.Attributes.Append(GetAttribute("category", category));
        //    counter.Attributes.Append(GetAttribute("instance", instance));
        //    counter.Attributes.Append(GetAttribute("countername", counterName));
        //    return counter;
        //}
    }
}
