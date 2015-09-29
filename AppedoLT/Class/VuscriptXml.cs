using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AppedoLT
{
    /// <summary>
    /// Used to create vuscript.xml for each script.
    /// 
    /// Author: Rasith
    /// </summary>
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

        /// <summary>
        /// Create attribute in vuscript.xml for give name and value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public XmlAttribute GetAttribute(string name, string value)
        {
            XmlAttribute att = _doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }

        /// <summary>
        /// Create or open script file(vuscript.xml)
        /// </summary>
        /// <param name="id">Script id</param>
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

       /// <summary>
        /// Create or open script file(vuscript.xml)
       /// </summary>
       /// <param name="id">Script id</param>
       /// <param name="content"> script content</param>
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

        /// <summary>
        /// Save script file
        /// </summary>
        /// <param name="deleteUnwantedResource">If it true, it will delete unwanted resource</param>
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

        /// <summary>
        /// Get list  of available scripts
        /// </summary>
        /// <returns>Available script list</returns>
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

        /// <summary>
        /// Get list  of available script id with name
        /// </summary>
        /// <returns>Available script list</returns>
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

        /// <summary>
        /// Get list  of available script name with id
        /// </summary>
        /// <returns></returns>
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
