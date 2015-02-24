using System;
using System.Xml;
using System.Text;
using AppedoLT.Core;
using System.IO;
using System.Threading;

namespace AppedoLT
{
    class RepositoryXml
    {

        public XmlDocument doc = new XmlDocument();
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        private static RepositoryXml instance;
        public static RepositoryXml GetInstance()
        {
            if (instance == null)
            {
                instance = new RepositoryXml();
            }
            return instance;
        }
        private RepositoryXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\VUScripts.xml";

                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");                     
                        xml.Append("<scenarios/>");
                        xml.Append("<flag/>");
                        xml.Append("<loadgens/>");
                        xml.Append("<runs/>");
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
        public void Save()
        {
            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\VUScripts.xml");
        }
        public XmlNode CreateScenario()
        {
            XmlNode scenario = doc.CreateElement("scenario");
            scenario.Attributes.Append(GetAttribute("id", Constants.GetInstance().UniqueID));
            return scenario;
        }
        public void CreateLoadgen(string ipAddress, string hostname, bool isDefaultZone, bool isChecked)
        {
            if (doc.SelectNodes("//loadgens").Count == 0)
            {
                doc.SelectSingleNode("//root").AppendChild(doc.CreateElement("loadgens"));

            }
            XmlNode loadGen = doc.SelectSingleNode("//root//loadgens//loadgen[@ipaddress='" + ipAddress + "']");
            if (loadGen == null)
            {
                loadGen = doc.CreateElement("loadgen");
                loadGen.Attributes.Append(GetAttribute("ipaddress", ipAddress));
                loadGen.Attributes.Append(GetAttribute("hostname", hostname));
                loadGen.Attributes.Append(GetAttribute("isdefaultzone", isDefaultZone.ToString()));
                loadGen.Attributes.Append(GetAttribute("ischecked", isChecked.ToString()));
                doc.SelectSingleNode("//root//loadgens").AppendChild(loadGen);
            }
        }

    }
}
