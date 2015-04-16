using System;
using System.Xml;
using System.Text;
using AppedoLT.Core;
using System.IO;

namespace AppedoLTLoadGenerator
{
    class LoadTestAgentXml
    {

        public XmlDocument doc = new XmlDocument();
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        private static LoadTestAgentXml instance;
        public static LoadTestAgentXml GetInstance()
        {
            if (instance == null)
            {
                instance = new LoadTestAgentXml();
            }
            return instance;
        }
        private LoadTestAgentXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\loadtestagent.xml";

                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<runs/>");
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

            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\loadtestagent.xml");
        }


        public XmlNode CreadRun(string runid, string reportfoldername, string scenarioname, string totalloadgenused, string currentloadgenid, string souceip, string loadgenname,string distribution)
        {
            XmlNode run=null;
            if(doc.SelectSingleNode("//runs/run[@runid='"+runid+"']")!=null)
            {
               doc.SelectSingleNode("//runs").RemoveChild( doc.SelectSingleNode("//runs/run[@runid='" + runid + "']"));
            }
            run = doc.CreateElement("run");
            run.Attributes.Append(GetAttribute("runid", runid));
            run.Attributes.Append(GetAttribute("scenarioname", scenarioname));
            run.Attributes.Append(GetAttribute("isresultfileSent", false.ToString()));
            run.Attributes.Append(GetAttribute("runcompleted", false.ToString()));
            run.Attributes.Append(GetAttribute("totalloadgenused", totalloadgenused));
            run.Attributes.Append(GetAttribute("currentloadgenid", currentloadgenid));
            run.Attributes.Append(GetAttribute("sourceip", souceip));
            run.Attributes.Append(GetAttribute("reportfoldername", reportfoldername));
            run.Attributes.Append(GetAttribute("loadgenname", loadgenname));
            run.Attributes.Append(GetAttribute("scenariostarttime", DateTime.Now.ToString()));
            run.Attributes.Append(GetAttribute("distribution", distribution));
            return run;
        }


    }
}
