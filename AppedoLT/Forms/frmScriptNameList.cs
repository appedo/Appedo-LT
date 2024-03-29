﻿using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class frmScriptNameList : Form
    {
        List<string> _availableScript = null;
        ucDesign _parent = null;
        int selectCount = 0;
        string userid =Session.UserID;
        string scriptResourcePath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\";
        public frmScriptNameList(List<string> availableScript,string scripts,ucDesign parent)
        {
            InitializeComponent();
            _parent = parent;
            SetTreeItem(scripts);
            _availableScript = availableScript;
        }
        void SetTreeItem(string scripts)
        {
            string[] list = scripts.Split(',');

            foreach (string name in list)
            {
                RadTreeNode node = new RadTreeNode();
                node.Text = name;
                node.Checked = false;
                tvScriptName.Nodes.Add(node);
            }

        }
        bool Download(string scriptName)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();

            XmlNode script = RepositoryXml.GetInstance().Doc.SelectSingleNode("//vuscript[@name='" + scriptName + "']");
            string scriptid;
            string extractFolderPath=string.Empty;
            string extractFilePath;

            try
            {
                Dictionary<string,string> scripts=VuscriptXml.GetScriptNameAndId();
                if (scripts.ContainsKey(scriptName))
                {
                    scriptid = scripts[scriptName];
                }
                else
                {
                    scriptid = Constants.GetInstance().UniqueID;
                }

                extractFolderPath = scriptResourcePath + scriptid + "_" + DateTime.Now.Ticks;
                extractFilePath = extractFolderPath + "\\" + scriptName + ".zip";
                Directory.CreateDirectory(extractFolderPath);

                Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                header.Add("userid", userid);
                header.Add("scriptname", scriptName);
                server.Send(new TrasportData("GETSCRIPT", string.Empty, header));

                if (File.Exists(extractFilePath) == true) File.Delete(extractFilePath);

                TrasportData respose = DownloadFile(server, extractFilePath, scriptName);
                server.Close();
                Constants.GetInstance().UnZip(extractFilePath, extractFolderPath);
                File.Delete(extractFilePath);

                XmlDocument doc = new XmlDocument();
                doc.Load(extractFolderPath + "\\vuscript.xml");
                doc.SelectSingleNode("//vuscript").Attributes["id"].Value = scriptid;
                doc.Save(extractFolderPath + "\\vuscript.xml");            
                if (Directory.Exists(scriptResourcePath + "\\" + scriptid)) Directory.Delete(scriptResourcePath + "\\" + scriptid, true);
                Directory.Move(extractFolderPath, scriptResourcePath + "\\" + scriptid);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                if(Directory.Exists(extractFolderPath))
                {
                    Directory.Delete(extractFolderPath,true);
                }
            }
        }
        private void btnDownload_Click(object sender, EventArgs e)
        {

            List<string> names = new List<string>();
            List<string> faildDownload = new List<string>();
            List<string> conflictScript = new List<string>();

            foreach (RadTreeNode node in tvScriptName.Nodes)
            {
                if (node.Checked == true)
                {
                    names.Add(node.Text);
                    if (_availableScript.Contains(node.Text) == true) conflictScript.Add(node.Text);
                    
                }
            }
            if (names.Count == 0)
            {
                MessageBox.Show("Please select script(s) to download");
                return;
            }

            if (conflictScript.Count > 0)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Following script(s) are available on your local machine.");
                foreach (string name in conflictScript)
                {
                    msg.AppendLine(name);
                }
                msg.AppendLine().AppendLine("Are you sure you want to replace?");
                if (MessageBox.Show(msg.ToString(),"Warning",MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }
            foreach (string script in names)
            {
                if (Download(script) == false) faildDownload.Add(script);

            }
            if(faildDownload.Count>0)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Unable to download following scipt(s): ");
                foreach (string script in faildDownload)
                {
                    msg.AppendLine(script);
                }
                msg.AppendLine().AppendLine("Please try again");
                MessageBox.Show(msg.ToString());
            }
            else
            {
                _parent.LoadTreeItem();
                MessageBox.Show("Downloaded successfully!");
                this.Close();
               

            }

        }
        private void Copynode(string scriptid, string scriptName, XmlNode scr, XmlNode des)
        {

            if (des == null)
            {
                des = RepositoryXml.GetInstance().Doc.CreateElement("vuscript");
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("name", scriptName));
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("id", scriptid));
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("autoid", "0"));
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("type", "http"));
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("exclutionfiletypes", string.Empty));
                des.Attributes.Append(RepositoryXml.GetInstance().GetAttribute("dynamicreqenable", false.ToString()));
                RepositoryXml.GetInstance().Doc.SelectNodes("root/vuscripts")[0].AppendChild(des);
            }
            des.Attributes["name"].Value = scriptName;
            des.Attributes["autoid"].Value = scr.Attributes["autoid"].Value;
            des.Attributes["type"].Value = scr.Attributes["type"].Value;
            des.Attributes["exclutionfiletypes"].Value = scr.Attributes["exclutionfiletypes"].Value;
            des.Attributes["dynamicreqenable"].Value = scr.Attributes["dynamicreqenable"].Value;
            des.InnerXml = scr.InnerXml;
            RepositoryXml.GetInstance().Save();

        }
        private TrasportData DownloadFile(Trasport server, string filePath,string name)
        {
            TrasportData respose = null;
            long totalByte = 0;
            long recivedByte = 0;
            bool Success = true;

            new Thread(() =>
            {
                try
                {
                    respose = server.Receive(filePath, ref totalByte, ref recivedByte, ref Success);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    Success = false;
                }
            }).Start();

            while (((totalByte == 0 && recivedByte == 0) || recivedByte < totalByte))
            {
                if (totalByte > 0)
                {

                    frmDownloadProgress frm = new frmDownloadProgress(true);
                    frm.Text = "Downloading[" + name + "]...";
                    new Thread(() =>
                    {
                        frm.UpdateStatus(ref totalByte, ref recivedByte, ref Success);
                    }).Start();
                    frm.ShowDialog();
                }
                if (Success == false) break;
                Thread.Sleep(1000);
            }
            if (Success == false)
            {
                throw new Exception("Download Faild");
            }
            return respose;
        }
        
    }
}
