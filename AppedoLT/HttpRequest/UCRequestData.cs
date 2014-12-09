using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{
    public partial class UCRequestData : UserControl
    {
        public string _method = string.Empty;
        private static UCRequestData _instance;
        public static UCRequestData GetInstance(XmlNode request)
        {
            if (_instance == null)
                _instance = new UCRequestData();
            _instance.SetValue(request);
            return _instance;
        }
        private UCRequestData()
        {
            InitializeComponent();
        }
        private void SetValue(XmlNode request)
        {
            XmlNode querySring = request.SelectSingleNode("querystringparams");
            XmlNode postData = request.SelectSingleNode("params");
            
            tabItem1.ContentPanel.Controls.Clear();
            tabItem1.ContentPanel.Controls.Add(UCQueryStringData.GetInstance(querySring));

            if (request.Attributes["Method"].Value == "GET")
            {
                tapPostParam.Visibility = Telerik.WinControls.ElementVisibility.Hidden;
                tabItem1.Select();
            }
            else if (request.Attributes["Method"].Value == "POST" && postData != null)
            {
                tapPostParam.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                tapPostParam.Select();
                switch (postData.Attributes["type"].Value.ToString())
                {
                    case "multipart/form-data":
                        tapPostParam.ContentPanel.Controls.Clear();
                        tapPostParam.ContentPanel.Controls.Add(UCMultipartPost.GetInstance(postData));
                        tapPostParam.Text = "Multipart Parameters";
                        break;
                    case "text":
                        tapPostParam.ContentPanel.Controls.Clear();
                        tapPostParam.ContentPanel.Controls.Add(UCTextPost.GetInstance(postData));
                        tapPostParam.Text = "Text Content";
                        break;
                    default:
                        tapPostParam.ContentPanel.Controls.Clear();
                        tapPostParam.ContentPanel.Controls.Add(UCFormPost.GetInstance(postData));
                        tapPostParam.Text = "Post Parameters";
                        break;
                }
            }
           
        }
    }
}
