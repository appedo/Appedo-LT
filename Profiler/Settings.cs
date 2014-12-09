using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Profiler
{
    public partial class Settings : Form
    {
        ProfilerXml xml = ProfilerXml.GetInstance();
        public string uid = string.Empty;

        public Settings()
        {
            InitializeComponent();
            ddlClrVer.SelectedIndex = 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string path = GetPath();
                path = path + "/getConfigurations";
                string data = string.Format("agent_type={0}&guid={1}", xml.doc.SelectSingleNode("//root/type").Attributes["value"].Value, txtGuid.Text);
                data = GetPageContent(path, data);
                uid = new Regex(".* \"uid\": \"(.*?)\"").Match(data).Groups[1].Value;
                if(uid==string.Empty)
                {
                    MessageBox.Show("Invaild GUID");
                    this.DialogResult = DialogResult.None;
                }
                else
                {
                    xml.doc.SelectSingleNode("//root/guid").Attributes["value"].Value = txtGuid.Text;
                    xml.doc.SelectSingleNode("//root/uid").Attributes["value"].Value = uid;
                    xml.doc.SelectSingleNode("//root/clrversion").Attributes["value"].Value = ddlClrVer.Text;
                    xml.Save();
                }

            }
            catch(Exception ex)
            {
                this.DialogResult = DialogResult.None;
                MessageBox.Show(ex.Message);
            }
        }
        private string GetPath()
        {
            return string.Format("{0}://{1}:{2}/{3}", ConfigurationSettings.AppSettings["protocol"], ConfigurationSettings.AppSettings["server"], ConfigurationSettings.AppSettings["port"], ConfigurationSettings.AppSettings["path"]);
        }
        public string GetPageContent(string Url, string data)
        {
            string PageContent = string.Empty;
            HttpWebRequest WebRequestObject = null;
            try
            {
                WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);
                WebRequestObject.Method = "POST";
                WebRequestObject.ContentLength = data.Length;
                using (Stream stream = WebRequestObject.GetRequestStream())
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(data), 0, data.Length);
                }
                WebResponse Response = WebRequestObject.GetResponse();
                Stream WebStream = Response.GetResponseStream();
                StreamReader objReader = new StreamReader(WebStream);
                PageContent = objReader.ReadToEnd();
                objReader.Close();
                WebStream.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            finally
            {
                WebRequestObject = null;
            }
            return PageContent;
        }
    }
}
