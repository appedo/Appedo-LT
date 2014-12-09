using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Profiler
{
    public abstract class XmlFileProccessor
    {
        public XmlDocument doc = new XmlDocument();
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        public abstract void Save();       
    }

    class ProfilerXml : XmlFileProccessor
    {
        private ProfilerXml()
        {
            try
            {
                string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "\\Profiler.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append(@"<guid value="""" />");
                        xml.Append(@"<uid value="""" />");
                        xml.Append(@"<type value=""MSIIS"" />");
                        xml.Append(@"<clrversion  value="""" />");
                        xml.Append("</root>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                doc.Load(file);
                //_serverId = Convert.ToInt32(doc.SelectSingleNode("root/uniquenumbers/server").Attributes["lastcreatedid"].Value);
            }
            catch (Exception ex)
            {
                //ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static ProfilerXml instance;
        public static ProfilerXml GetInstance()
        {
            if (instance == null)
            {
                instance = new ProfilerXml();
            }
            return instance;
        }
        public override void Save()
        {
            doc.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "\\Profiler.xml");
        }
    }
}
