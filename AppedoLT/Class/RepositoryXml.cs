using AppedoLT.Core;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AppedoLT
{
    /// <summary>
    /// Single instance class for VUScripts.xml file. It is used to store and retrive data from VUScripts.xml file.
    /// 
    /// Author: Rasith
    /// </summary>
    class RepositoryXml
    {
        #region The static varialbles and methods

        private static RepositoryXml instance;

        public static RepositoryXml GetInstance()
        {
            if (instance == null)
            {
                instance = new RepositoryXml();
            }
            return instance;
        }

        #endregion

        #region The private fields

        private XmlDocument _doc = new XmlDocument();
        
        #endregion

        #region The public property

        public XmlDocument Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }

        #endregion

        #region The constructor

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
                _doc.Load(file);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion

        #region The public methods

        /// <summary>
        /// Save content into VUScripts.xml after any change.
        /// </summary>
        public void Save()
        {
            _doc.Save(Constants.GetInstance().ExecutingAssemblyLocation + "\\VUScripts.xml");
        }

        /// <summary>
        /// Create attribute in VUScripts.xml for give name and value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = _doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        /// <summary>
        /// To create scenario xml node.
        /// </summary>
        /// <returns></returns>
        public XmlNode CreateScenario()
        {
            XmlNode scenario = _doc.CreateElement("scenario");
            scenario.Attributes.Append(GetAttribute("id", Constants.GetInstance().UniqueID));
            return scenario;
        }

        /// <summary>
        /// To create load gen info node.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="hostname"></param>
        /// <param name="isDefaultZone"></param>
        /// <param name="isChecked"></param>
        public void CreateLoadgen(string ipAddress, string hostname, bool isDefaultZone, bool isChecked)
        {
            if (_doc.SelectNodes("//loadgens").Count == 0)
            {
                _doc.SelectSingleNode("//root").AppendChild(_doc.CreateElement("loadgens"));

            }
            XmlNode loadGen = _doc.SelectSingleNode("//root//loadgens//loadgen[@ipaddress='" + ipAddress + "']");
            if (loadGen == null)
            {
                loadGen = _doc.CreateElement("loadgen");
                loadGen.Attributes.Append(GetAttribute("ipaddress", ipAddress));
                loadGen.Attributes.Append(GetAttribute("hostname", hostname));
                loadGen.Attributes.Append(GetAttribute("isdefaultzone", isDefaultZone.ToString()));
                loadGen.Attributes.Append(GetAttribute("ischecked", isChecked.ToString()));
                _doc.SelectSingleNode("//root//loadgens").AppendChild(loadGen);
            }
        }

        #endregion
    }
}
