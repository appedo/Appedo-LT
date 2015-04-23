using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AppedoLT
{
    public class VuscriptXml
    {
        #region The private fields

        private string _id = string.Empty;
        private string _filePath = string.Empty;
        private XmlDocument _doc = new XmlDocument();

        #endregion

        #region The public property

        public XmlDocument Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }

        #endregion

        #region The public methods

        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = _doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        public VuscriptXml(string id)
        {
            try
            {
                _id = id;
                string scriptDic = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + id;
                if (!Directory.Exists(scriptDic)) Directory.CreateDirectory(scriptDic);
                _filePath = scriptDic + "\\vuscript.xml";

                if (File.Exists(_filePath) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        xml.Append("<vuscript></vuscript>");
                        streamWriter.Write(xml.ToString());
                    }
                }
                _doc.Load(_filePath);

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public VuscriptXml(string id, string content)
        {
            try
            {
                _id = id;
                string scriptDic = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + id;
                if (!Directory.Exists(scriptDic)) Directory.CreateDirectory(scriptDic);
                _filePath = scriptDic + "\\vuscript.xml";

                if (File.Exists(_filePath) == false)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.Append(content);
                        streamWriter.Write(xml.ToString());
                    }
                }
                _doc.Load(_filePath);

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Save(bool deleteUnwantedResource = true)
        {
            if (deleteUnwantedResource == true)
            {
                try
                {
                    DirectoryInfo info;
                    foreach (XmlNode script in _doc.SelectNodes("//vuscript"))
                    {
                        info = new DirectoryInfo(Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + script.Attributes["id"].Value);
                        foreach (FileInfo fileName in info.GetFiles())
                        {
                            if (script.SelectSingleNode("//request[@reqFilename='" + fileName.Name + "' or @resFilename='" + fileName.Name + "']") == null)
                            {
                                File.Delete(fileName.FullName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
            _doc.Save(_filePath);
        }

        public static List<string> GetScriptName()
        {
            List<string> scritpNames = new List<string>();
            foreach (string info in Directory.GetDirectories(".\\Scripts"))
            {
                try
                {
                    DirectoryInfo dicinfo = new DirectoryInfo(info);

                    if (File.Exists(info + "\\vuscript.xml"))
                    {
                        VuscriptXml vuscriptXml = new VuscriptXml(dicinfo.Name);
                        scritpNames.Add(vuscriptXml._doc.SelectSingleNode("//vuscript").Attributes["name"].Value);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
            return scritpNames;

        }

        public static Dictionary<string, string> GetScriptidAndName()
        {
            Dictionary<string, string> scritpNames = new Dictionary<string, string>();
            foreach (string info in Directory.GetDirectories(".\\Scripts"))
            {
                try
                {
                    DirectoryInfo dicinfo = new DirectoryInfo(info);

                    if (File.Exists(info + "\\vuscript.xml"))
                    {
                        VuscriptXml vuscriptXml = new VuscriptXml(dicinfo.Name);
                        scritpNames.Add(vuscriptXml._doc.SelectSingleNode("//vuscript").Attributes["id"].Value, vuscriptXml._doc.SelectSingleNode("//vuscript").Attributes["name"].Value);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
            return scritpNames;
        }

        public static Dictionary<string, string> GetScriptNameAndId()
        {
            Dictionary<string, string> scritpNames = new Dictionary<string, string>();
            foreach (string info in Directory.GetDirectories(".\\Scripts"))
            {
                try
                {
                    DirectoryInfo dicinfo = new DirectoryInfo(info);

                    if (File.Exists(info + "\\vuscript.xml"))
                    {
                        VuscriptXml vuscriptXml = new VuscriptXml(dicinfo.Name);
                        scritpNames.Add(vuscriptXml._doc.SelectSingleNode("//vuscript").Attributes["name"].Value, vuscriptXml._doc.SelectSingleNode("//vuscript").Attributes["id"].Value);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
            return scritpNames;
        }

        #endregion
    }
}
