using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Xml;

namespace AppedoLT
{
    public partial class frmTcpAssertion : Telerik.WinControls.UI.RadForm
    {
        XmlNode _assertions = null;
        XmlNode _assertion=null;
        RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        public frmTcpAssertion(XmlNode assertions,XmlNode assertion)
        {
            InitializeComponent();
            _assertions = assertions;
            _assertion = assertion;
            if (_assertion != null)
            {
                SetAssertion(_assertion);
            }
        }
        private void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlType.SelectedIndex)
            {
                case 0:
                    grbRequest.Enabled = true;
                    grbResponse.Enabled = false;
                    txtRequest.Enabled = true;
                    break;
                case 1:
                    grbRequest.Enabled = false;
                    grbResponse.Enabled = true;
                    txtResponse.Enabled = true;
                    break;
                case 2:
                case 3:
                    txtRequest.Enabled = false;
                    txtResponse.Enabled = false;
                    grbRequest.Enabled = true;
                    grbResponse.Enabled = true;
                    break;
            }
        }
        public void SetAssertion(XmlNode assertion)
        {
            ddlType.SelectedIndex =Convert.ToInt16(assertion.Attributes["type"].Value);
            txtRequest.Text = assertion.Attributes["reqtext"].Value;
            txtRequestStartPosition.Text = assertion.Attributes["reqposition"].Value;
            txtRequestStartlength.Text = assertion.Attributes["reqlength"].Value;
            txtResponse.Text = assertion.Attributes["restext"].Value;
            txtResponseStartPosition.Text = assertion.Attributes["resposition"].Value;
            txtResponseStartlength.Text = assertion.Attributes["reslength"].Value;
            txtErrorMessage.Text = assertion.Attributes["message"].Value;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_assertion == null)
                {
                    _assertion = _repositoryXml.Doc.CreateElement("assertion");
                    _assertions.AppendChild(_assertion);
                }
                else
                {
                    _assertion.Attributes.RemoveAll();
                }
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("type", ddlType.SelectedIndex.ToString()));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("reqtext", txtRequest.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("reqposition", txtRequestStartPosition.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("reqlength", txtRequestStartlength.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("restext", txtResponse.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("resposition", txtResponseStartPosition.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("reslength", txtResponseStartlength.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("message", txtErrorMessage.Text));
            }
            catch
            {

            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
