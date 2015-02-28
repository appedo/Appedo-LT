using System;
using System.Xml;
using System.Text;
using AppedoLT.Core;
using System.IO;

namespace AppedoLT
{
    class RepositoryXml
    {
        private object _logObj = new object();
        private int _scriptId;
        public int _scenarioId;

        public string ScriptId
        {
            get
            {
                lock (new object())
                {
                   
                    return (++_scriptId).ToString();
                }
            }
        }

        public string ScenarioId
        {
            get
            {
                lock (new object())
                {

                    return (++_scenarioId).ToString();
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
                        xml.Append("<uniquenumbers>");
                        xml.Append("<script lastcreatedid=\"0\" />");
                        xml.Append("<scenario lastcreatedid=\"0\" />");
                        xml.Append("</uniquenumbers>");
                        xml.Append("<vuscripts/>");
                        xml.Append("<scenarios/>");
                        xml.Append("<flag/>");
                        xml.Append("<loadgens/>");
                        xml.Append("<runs/>");
                        xml.Append("</root>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                doc.Load(file);
                _scriptId = Convert.ToInt32(doc.SelectSingleNode("root/uniquenumbers/script").Attributes["lastcreatedid"].Value);
                _scenarioId = Convert.ToInt32(doc.SelectSingleNode("root/uniquenumbers/scenario").Attributes["lastcreatedid"].Value);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public void Save()
        {
            doc.SelectSingleNode("root/uniquenumbers/script").Attributes["lastcreatedid"].Value = _scriptId.ToString();

            doc.SelectSingleNode("root/uniquenumbers/scenario").Attributes["lastcreatedid"].Value = _scenarioId.ToString();

            doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\VUScripts.xml");
        }

        public XmlNode CreateContainer(string scriptid,string name)
        {
            #region NewContainer
            XmlNode container = doc.CreateElement("container");
            container.Attributes.Append(GetAttribute("name", name));
            container.Attributes.Append(GetAttribute("id", GetId(scriptid)));
            return container;
            #endregion
        }
        public XmlNode CreateDelay()
        {
            #region NewContainer
            XmlNode delay = doc.CreateElement("delay");
            delay.Attributes.Append(GetAttribute("delaytime", "0"));
            return delay;
            #endregion
        }
        public XmlNode CreateLog(string scriptid)
        {
            #region NewContainer
            XmlNode log = doc.CreateElement("log");
            log.Attributes.Append(GetAttribute("id", GetId(scriptid)));
            log.Attributes.Append(GetAttribute("name", string.Empty));
            log.Attributes.Append(GetAttribute("message", string.Empty));
            return log;
            #endregion
        }
        public XmlNode CreateIfThenElse()
        {
            XmlNode ifthenelse = doc.CreateElement("if");
            ifthenelse.Attributes.Append(GetAttribute("id", DateTime.Now.Ticks.ToString()));
            ifthenelse.Attributes.Append(GetAttribute("condition", string.Empty));

            XmlNode then = doc.CreateElement("then");
            then.Attributes.Append(GetAttribute("id", DateTime.Now.Ticks.ToString()));

            XmlNode els = doc.CreateElement("else");
            then.Attributes.Append(GetAttribute("id", DateTime.Now.Ticks.ToString()));

            ifthenelse.AppendChild(then);
            ifthenelse.AppendChild(els);

            return ifthenelse;
        }
        public XmlNode CreateLoop(string scriptid, string name)
        {
            XmlNode loop = doc.CreateElement("loop");
            loop.Attributes.Append(GetAttribute("id", GetId(scriptid)));
            loop.Attributes.Append(GetAttribute("loopcount", "0"));
            loop.Attributes.Append(GetAttribute("name", name));

            return loop;
        }
        public XmlNode CreateWhileLoop(string scriptid)
        {
            XmlNode whileLoop = doc.CreateElement("whileloop");
            whileLoop.Attributes.Append(GetAttribute("id", GetId(scriptid)));
            whileLoop.Attributes.Append(GetAttribute("condition", string.Empty));
            return whileLoop;
        }
        public XmlNode CreateJavaScript()
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
            javaScript.Attributes.Append(GetAttribute("id", DateTime.Now.Ticks.ToString()));
            javaScript.Attributes.Append(GetAttribute("script", defaultCode.ToString()));

            return javaScript;
        }
        public XmlNode CreateStartTransaction()
        {
            XmlNode transaction = doc.CreateElement("starttransaction");
            transaction.Attributes.Append(GetAttribute("transactionname", string.Empty));
            return transaction;
        }
        public XmlNode CreateEndTransaction()
        {
            XmlNode transaction = doc.CreateElement("endtransaction");
            transaction.Attributes.Append(GetAttribute("transactionname", string.Empty));

            return transaction;
        }
        public XmlNode CreateScenario()
        {
            XmlNode scenario = doc.CreateElement("scenario");
            scenario.Attributes.Append(GetAttribute("id", ScenarioId));
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
        public string GetId(string scriptid)
        {
            lock (_logObj)
            {
                XmlNode node = RepositoryXml.GetInstance().doc.SelectSingleNode("//vuscripts/vuscript[@id='" + scriptid + "']");
                if (node != null)
                {
                    int id = int.Parse(node.Attributes["autoid"].Value);
                    id++;
                    node.Attributes["autoid"].Value = id.ToString();
                    return id.ToString();
                }
                else
                {
                    return ((int)DateTime.Now.Ticks / 100000).ToString();
                }
            }
        }

    }
}
