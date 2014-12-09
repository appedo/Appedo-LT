using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AppedoLT.Core;
using System.IO;

namespace AppedoLT
{
    class MonitorXml
    {
        private int _serverId;
        public string ServerId
        {
            get
            {
                lock (new object())
                {
                    return (++_serverId).ToString();
                }
            }
        }
        public XmlDocument doc = new XmlDocument();
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        private MonitorXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\Monitor.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append("<uniquenumbers>");
                        xml.Append("<server lastcreatedid=\"0\" />");
                        xml.Append("</uniquenumbers>");
                        xml.Append("<servers/>");
                        xml.Append("</root>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                doc.Load(file);
                _serverId = Convert.ToInt32(doc.SelectSingleNode("root/uniquenumbers/server").Attributes["lastcreatedid"].Value);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static MonitorXml instance;
        public static MonitorXml GetInstance()
        {
            if (instance == null)
            {
                instance = new MonitorXml();
            }
            return instance;
        }
        public XmlNode CreateServer()
        {
            XmlNode server = doc.CreateElement("server");
            string id = ServerId;
            server.Attributes.Append(GetAttribute("id", id));
            server.Attributes.Append(GetAttribute("name", "server" + id));
            server.Attributes.Append(GetAttribute("type", string.Empty));
            server.Attributes.Append(GetAttribute("ipaddress", string.Empty));
            server.Attributes.Append(GetAttribute("username", string.Empty));
            server.Attributes.Append(GetAttribute("password", string.Empty));
            server.Attributes.Append(GetAttribute("interval", "5"));
            XmlNode counters = doc.CreateElement("counters");
            server.AppendChild(counters);

            return server;
        }
        public XmlNode CreateCounter(string category,string instance,string counterName)
        {
            XmlNode counter = doc.CreateElement("counter");
           
            counter.Attributes.Append(GetAttribute("category", category));
            counter.Attributes.Append(GetAttribute("instance", instance));
            counter.Attributes.Append(GetAttribute("countername", counterName));
            return counter;
        }
        public void Save()
        {
            doc.SelectSingleNode("root/uniquenumbers/server").Attributes["lastcreatedid"].Value = _serverId.ToString();
            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\Monitor.xml");
        }

    }
}
