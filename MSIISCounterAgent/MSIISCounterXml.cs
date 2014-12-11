using AgentCore;
using System;
using System.IO;
using System.Text;

namespace MSIISCounterAgent
{
    class MSIISCounterXml:XmlFileProccessor
    {
        private MSIISCounterXml()
        {
            try
            {
                string file = Utility.GetInstance().ExecutingAssemblyLocation + "\\MSIISCounterXml.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append(@"<counters>");
                        xml.Append(@"<counter id=""10000001"" category=""Web Service"" countername=""Bytes Sent/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""10000002"" category=""Web Service"" countername=""Bytes Received/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""10000003"" category=""Web Service"" countername=""Bytes total/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""10000004"" category=""Web Service"" countername=""Connection attempts/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""10000005"" category=""Web Service"" countername=""Get requests/sec"" instance=""_Total"" scale=""1""/>");
                        xml.Append(@"<counter id=""10000006"" category=""ASP.NET"" countername=""Requests Queued"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""10000007"" category=""ASP.NET Applications"" countername=""Sessions Active"" instance=""__Total__"" scale=""1""/>");
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
        private static MSIISCounterXml instance;
        public static MSIISCounterXml GetInstance()
        {
            if (instance == null)
            {
                instance = new MSIISCounterXml();
            }
            return instance;
        }
        public override void Save()
        {
            doc.Save(Utility.GetInstance().ExecutingAssemblyLocation + "\\MSIISCounterXml.xml");
        }
    }
}
