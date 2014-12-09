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
    public partial class UCTextPost : UserControl
    {
         private static UCTextPost _instance;
         public static UCTextPost GetInstance(XmlNode textPostData)
        {
            if (_instance == null)
                _instance = new UCTextPost();
            _instance.SetValue(textPostData);
            return _instance;
        }
        private UCTextPost()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }
        private void SetValue(XmlNode textPostData)
        {
            
            txtValue.Text = textPostData.ChildNodes[0].Attributes["value"].Value;
            txtValue.Tag = textPostData.ChildNodes[0].Attributes["value"];
        }

        private void txtValue_Validated(object sender, EventArgs e)
        {
            XmlAttribute att=(XmlAttribute)txtValue.Tag;
            if (att.Value != txtValue.Text)
            {
                att.Value = txtValue.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (new RequestParameter((XmlAttribute)txtValue.Tag).ShowDialog() == DialogResult.OK)
            {
                txtValue.Text = ((XmlAttribute)txtValue.Tag).Value;
            }
        }
    }
}
