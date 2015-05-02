using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{
    public partial class frmScriptUpgrade : Form
    {
        RepositoryXml repositoryXml = RepositoryXml.GetInstance();
        public frmScriptUpgrade()
        {
            InitializeComponent();
            Upgrade();
        }
        void Upgrade()
        {
            XmlNode vuscripts=repositoryXml.Doc.SelectSingleNode("//vuscripts");
            progressBar1.Minimum = 0;
            
            if(vuscripts!=null &&  vuscripts.ChildNodes.Count>0)
            {
                progressBar1.Maximum = vuscripts.ChildNodes.Count;
                foreach (XmlNode script in vuscripts.ChildNodes)
                {
                    try
                    {
                        string scriptid = script.Attributes["id"].Value;
                        string sorceRequestFolderPath = ".\\Request\\";
                        string sorceResponseFolderPath = ".\\Response\\";
                        string desFolderPath = ".\\Scripts\\" + scriptid + "\\";
                        if (Directory.Exists(desFolderPath)) Directory.Delete(desFolderPath, true);
                        VuscriptXml vuscriptxml = new VuscriptXml(scriptid, script.OuterXml);
                        vuscriptxml.Save();

                       // foreach (XmlNode request in vuscriptxml.doc.SelectNodes("//request"))
                        {
                           // if (File.Exists(sorceRequestFolderPath + request.Attributes["reqFilename"].Value)) File.Copy(sorceRequestFolderPath + request.Attributes["reqFilename"].Value, desFolderPath + request.Attributes["reqFilename"].Value);
                            //if (File.Exists(sorceResponseFolderPath + request.Attributes["resFilename"].Value)) File.Copy(sorceResponseFolderPath + request.Attributes["resFilename"].Value, desFolderPath + request.Attributes["resFilename"].Value);
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                    progressBar1.Value += 1;
                }
            }
            this.Close();
        }
    }
}
