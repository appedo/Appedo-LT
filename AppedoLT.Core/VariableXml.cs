using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AppedoLT.Core
{

    public class VariableXml
    {
        public XmlDocument doc = new XmlDocument();
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        private VariableXml()
        {
            try
            {
                string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\variables.xml";
                if (File.Exists(file) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<variables>");
                        xml.Append("</variables>");
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
        private static VariableXml instance;
        public static VariableXml GetInstance()
        {
            if (instance == null)
            {
                instance = new VariableXml();
            }
            return instance;
        }
        public void Save()
        {
            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\variables.xml");
        }
    }

}
