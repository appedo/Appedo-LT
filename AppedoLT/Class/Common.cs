using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AppedoLT
{
    class Common
    {
        private static Common instance;
        public static Common GetInstance()
        {
            if (instance == null)
            {
                instance = new Common();
            }
            return instance;
        }
        public XmlAttribute GetAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        public XmlNode CreateContainer(XmlDocument doc, string name)
        {
            #region NewContainer
            XmlNode container = doc.CreateElement("container");
            container.Attributes.Append(GetAttribute(doc, "name", name));
            container.Attributes.Append(GetAttribute(doc, "id", Constants.GetInstance().UniqueID));
            return container;
            #endregion
        }
        public XmlNode CreateDelay(XmlDocument doc)
        {
            #region NewContainer
            XmlNode delay = doc.CreateElement("delay");
            delay.Attributes.Append(GetAttribute(doc, "delaytime", "0"));
            return delay;
            #endregion
        }
        public XmlNode CreateLog(XmlDocument doc)
        {
            #region NewContainer
            XmlNode log = doc.CreateElement("log");
            log.Attributes.Append(GetAttribute(doc, "id", Constants.GetInstance().UniqueID));
            log.Attributes.Append(GetAttribute(doc, "name", string.Empty));
            log.Attributes.Append(GetAttribute(doc, "message", string.Empty));
            return log;
            #endregion
        }
        public XmlNode CreateIfThenElse(XmlDocument doc)
        {
            XmlNode ifthenelse = doc.CreateElement("if");
            ifthenelse.Attributes.Append(GetAttribute(doc, "id", DateTime.Now.Ticks.ToString()));
            ifthenelse.Attributes.Append(GetAttribute(doc, "condition", string.Empty));

            XmlNode then = doc.CreateElement("then");
            then.Attributes.Append(GetAttribute(doc, "id", DateTime.Now.Ticks.ToString()));

            XmlNode els = doc.CreateElement("else");
            then.Attributes.Append(GetAttribute(doc, "id", DateTime.Now.Ticks.ToString()));

            ifthenelse.AppendChild(then);
            ifthenelse.AppendChild(els);

            return ifthenelse;
        }
        public XmlNode CreateLoop(XmlDocument doc, string name)
        {
            XmlNode loop = doc.CreateElement("loop");
            loop.Attributes.Append(GetAttribute(doc, "id", Constants.GetInstance().UniqueID));
            loop.Attributes.Append(GetAttribute(doc, "loopcount", "0"));
            loop.Attributes.Append(GetAttribute(doc, "name", name));

            return loop;
        }
        public XmlNode CreateWhileLoop(XmlDocument doc)
        {
            XmlNode whileLoop = doc.CreateElement("whileloop");
            whileLoop.Attributes.Append(GetAttribute(doc, "id", Constants.GetInstance().UniqueID));
            whileLoop.Attributes.Append(GetAttribute(doc, "condition", string.Empty));
            return whileLoop;
        }
        public XmlNode CreateJavaScript(XmlDocument doc)
        {
            StringBuilder defaultCode = new StringBuilder();
            defaultCode.Append("  //To get parameter value use window.external.GetValue('variableName')").Append(Environment.NewLine);
            defaultCode.Append("  // To set parameter value use window.external.SetValue('variableName',JavaScriptVariable)").Append(Environment.NewLine).Append(" //or window.external.SetValue('variableName','Value')").Append(Environment.NewLine);
            defaultCode.Append(Environment.NewLine);
            defaultCode.Append(" //Starting point").Append(Environment.NewLine);
            defaultCode.Append("function main()").Append(Environment.NewLine);
            defaultCode.Append("{").Append(Environment.NewLine);
            defaultCode.Append("// Your Code").Append(Environment.NewLine).Append(" ").Append(Environment.NewLine);
            defaultCode.Append("}");

            XmlNode javaScript = doc.CreateElement("javascript");
            javaScript.Attributes.Append(GetAttribute(doc, "id", DateTime.Now.Ticks.ToString()));
            javaScript.Attributes.Append(GetAttribute(doc, "script", defaultCode.ToString()));

            return javaScript;
        }
        public XmlNode CreateStartTransaction(XmlDocument doc)
        {
            XmlNode transaction = doc.CreateElement("starttransaction");
            transaction.Attributes.Append(GetAttribute(doc, "transactionname", string.Empty));
            return transaction;
        }
        public XmlNode CreateEndTransaction(XmlDocument doc)
        {
            XmlNode transaction = doc.CreateElement("endtransaction");
            transaction.Attributes.Append(GetAttribute(doc, "transactionname", string.Empty));

            return transaction;
        }
        private Common()
        {
            //try
            //{
            //    string file = Constants.GetInstance().ExecutingAssemblyLocation + "\\VUScripts.xml";

            //    if (File.Exists(file) == false)
            //    {
            //        using (StreamWriter streamWriter = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            //        {
            //            StringBuilder xml = new StringBuilder();
            //            xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            //            xml.Append("<root>");                     
            //            xml.Append("<vuscripts/>");
            //            xml.Append("<scenarios/>");
            //            xml.Append("<flag/>");
            //            xml.Append("<loadgens/>");
            //            xml.Append("<runs/>");
            //            xml.Append("</root>");
            //            streamWriter.Write(xml.ToString());
            //        }
            //    }
            //    doc.Load(file);

            //}
            //catch (Exception ex)
            //{
            //    ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            //}
        }
    }
}
