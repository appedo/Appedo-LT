using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.Enumerations;

namespace AppedoLT
{
    public partial class frmAssertion : Telerik.WinControls.UI.RadForm
    {
        XmlNode _assertions = null;
        XmlNode _assertion = null;
        RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        public frmAssertion(XmlNode assertions, XmlNode assertion)
        {
            InitializeComponent();
            ddlCondition.SelectedIndex = 0;
            _assertions = assertions;
            _assertion = assertion;
            if (_assertion != null)
            {
                SetAssertion(_assertion);
            }
        }
        public void SetAssertion(XmlNode assertion)
        {
            txtName.Text = assertion.Attributes["name"].Value;
            if (assertion.Attributes["condition"].Value == "Response contain")
            {
                ddlCondition.SelectedIndex = 0;
            }
            else
            {
                ddlCondition.SelectedIndex = 1;
            }
            if (assertion.Attributes["type"].Value == "text")
            {
                rbtnText.ToggleState = ToggleState.On;
            }
            else
            {
                rbtnPattern.ToggleState = ToggleState.On;
            }
            txtText.Text = assertion.Attributes["text"].Value;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (Validate() == true)
            {

                if (_assertion == null)
                {
                    _assertion = _repositoryXml.doc.CreateElement("assertion");
                    _assertions.AppendChild(_assertion);
                }
                else
                {
                    _assertion.Attributes.RemoveAll();
                }
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("name", txtName.Text));
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("condition", ddlCondition.Text));
                if (rbtnText.ToggleState == ToggleState.On)
                {
                    _assertion.Attributes.Append(_repositoryXml.GetAttribute("type", "text"));
                }
                else
                {
                    _assertion.Attributes.Append(_repositoryXml.GetAttribute("type", "regex"));
                }
                _assertion.Attributes.Append(_repositoryXml.GetAttribute("text", txtText.Text));
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }

        }
        private bool Validate()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                errAssertion.SetError(txtName, "Required");
            }
            else
            {
                errAssertion.SetError(txtName, string.Empty);
            }
            if (txtText.Text.Trim() == string.Empty)
            {
                errAssertion.SetError(txtText, "Required");
            }
            else
            {
                if (rbtnPattern.ToggleState == ToggleState.On)
                {
                    try
                    {
                        Regex reg = new Regex(txtText.Text);
                        errAssertion.SetError(txtText, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        errAssertion.SetError(txtText, ex.Message);
                    }
                }
                else
                {
                    errAssertion.SetError(txtText, string.Empty);
                }
            }
            
            if (errAssertion.GetError(txtName) != string.Empty || errAssertion.GetError(txtText) != string.Empty) return false;
            else return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
