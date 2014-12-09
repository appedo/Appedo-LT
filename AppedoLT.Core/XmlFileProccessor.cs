using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AppedoLT.Core
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
}
