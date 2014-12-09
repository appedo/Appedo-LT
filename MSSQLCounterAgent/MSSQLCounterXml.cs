using AgentCore;
using System;
using System.IO;
using System.Text;

namespace MSSQLCounterAgent
{
    class MSSQLCounterXml:XmlFileProccessor
    {
        private MSSQLCounterXml()
        {
            try
            {
                string file = Utility.GetInstance().ExecutingAssemblyLocation + "\\MSSQLCounterXml.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append(@"<counters uid=""""  type=""MSSQL"">");
                        xml.Append(@"<counter id=""26000001"" category=""SQLServer:Buffer Manager"" countername=""Lazy writes/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000002"" category=""SQLServer:Buffer Manager"" countername=""Page life expectancy"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000003"" category=""SQLServer:Buffer Manager"" countername=""Page reads/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000004"" category=""SQLServer:Buffer Manager"" countername=""Page writes/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000005"" category=""SQLServer:Buffer Manager"" countername=""Checkpoint pages/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000006"" category=""SQLServer:Buffer Manager"" countername=""Free list stalls/sec"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000007"" category=""SQLServer:Memory Manager"" countername=""Memory Grants Pending"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000008"" category=""SQLServer:Memory Manager"" countername=""Target Server Memory (KB)"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000009"" category=""SQLServer:Memory Manager"" countername=""Total Server Memory (KB)"" instance="""" scale=""1""/>");
                        xml.Append(@"<counter id=""26000010"" category=""SQLServer:General Statistics"" countername=""User Connections"" instance="""" scale=""1""/>");
                        xml.Append("</counters>");
                        xml.Append("</root>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                doc.Load(file);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static MSSQLCounterXml instance;
        public static MSSQLCounterXml GetInstance()
        {
            if (instance == null)
            {
                instance = new MSSQLCounterXml();
            }
            return instance;
        }
        public override void Save()
        {
            doc.Save(Utility.GetInstance().ExecutingAssemblyLocation + "\\MSSQLCounterXml.xml");
        }
    }
}
