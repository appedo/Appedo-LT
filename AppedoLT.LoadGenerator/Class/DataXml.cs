using System;
using System.Xml;
using System.Text;
using AppedoLT.Core;
using System.IO;

namespace AppedoLTLoadGenerator
{
    class DataXml
    {
        public XmlDocument doc=new XmlDocument();
        public  XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        private static DataXml instance;
        public static DataXml GetInstance()
        {
            if (instance == null)
            {
                instance = new DataXml();
            }
            return instance;
        }
        private DataXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<root>");
                       
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

            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\Data.xml");
        }
        public XmlNode CreateData(string ipadddres,string port,string filePath)
        {
            XmlNode data = doc.CreateElement("data");
            data.Attributes.Append(GetAttribute("ipadddres", ipadddres));
            data.Attributes.Append(GetAttribute("port", port));
            data.Attributes.Append(GetAttribute("filePath", filePath));
            data.Attributes.Append(GetAttribute("issend", "0"));
            return data;

        }
    }
}
