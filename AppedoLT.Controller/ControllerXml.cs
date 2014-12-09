using System;
using System.Xml;
using System.Text;
using AppedoLT.Core;
using System.IO;

namespace AppedoLTController
{
    class ControllerXml
    {

        public XmlDocument doc=new XmlDocument();
        public  XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        private static ControllerXml instance;
        public static ControllerXml GetInstance()
        {
            if (instance == null)
            {
                instance = new ControllerXml();
            }
            return instance;
        }
        private ControllerXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\Controller.xml";

                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                        xml.Append("<runs lastrequestsourceip=\"\"/>");
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
           
            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\Controller.xml");
        }

        public XmlNode CreateRun(string reportname, XmlNode scenario,string sourceip)
        {
            XmlNode run = doc.CreateElement("run");
            run.Attributes.Append(GetAttribute("reportname", reportname));
            run.Attributes.Append(GetAttribute("sourceip", sourceip));
            run.AppendChild(CreateScriptInfo(scenario));
            
            return run;

        }
        public XmlNode CreateScriptInfo(XmlNode scenario)
        {
            XmlNode scripts = doc.CreateElement("scripts");
            foreach (XmlNode scenarioScript in scenario.SelectNodes("./scenario/script"))
            {
                XmlNode script = doc.CreateElement("script");
                foreach (XmlAttribute att in scenarioScript.Attributes)
                {
                    script.Attributes.Append(GetAttribute(att.Name, att.Value));
                }
                scripts.AppendChild(script);
            }
            return scripts;
        }
        public XmlNode CreadLoadGen(string ipaddress,string name)
        {
            XmlNode loadgen = doc.CreateElement("loadgen");
            loadgen.Attributes.Append(GetAttribute("ipaddress", ipaddress));
            loadgen.Attributes.Append(GetAttribute("hostname", name));
            loadgen.Attributes.Append(GetAttribute("resultfilereceived", false.ToString()));
            loadgen.Attributes.Append(GetAttribute("chartresultfilereceived", false.ToString()));
            loadgen.Attributes.Append(GetAttribute("runstarted", false.ToString()));
            
            return loadgen;
        }

    }
}
