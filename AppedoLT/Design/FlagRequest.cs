using System;
using System.Collections.Generic;
using System.Xml;
using AppedoLT.Core;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class FlagRequest : Telerik.WinControls.UI.RadForm
    {
        RadTreeNode receivedNode = new RadTreeNode();
        List<RequestResponse> requestResponse = new List<RequestResponse>();
        RepositoryXml reposioryXml = RepositoryXml.GetInstance();
        XmlNode _flagRequest = null;
        public XmlNode FlagRequestObj
        {
            get
            {
                return _flagRequest;
            }
            set
            {
                _flagRequest = value;
                if (_flagRequest != null)
                {
                    SetType(_flagRequest.Attributes["type"].Value);
                    txtHasVariable.Text = _flagRequest.Attributes["hasvariablename"].Value;
                    txtText.Text = _flagRequest.Attributes["text"].Value;
                    if (_flagRequest.Attributes["condition"].Value == "contain")
                    {
                        ddlCondition.SelectedIndex = 0;
                    }
                    else
                    {
                        ddlCondition.SelectedIndex = 1;
                    }
                }
            }
        }

        public FlagRequest(XmlNode flagRequset)
        {
            try
            {
                InitializeComponent();
                ddlCondition.SelectedIndex = 0;
                FlagRequestObj = flagRequset;
                this.receivedNode = receivedNode;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void SelectionChange(object sender, StateChangedEventArgs args)
        {
            ddlCondition.Enabled = txtHasVariable.Enabled = txtText.Enabled = btnVariableSelector.Enabled = false;
            if (rbtnRequestBody.ToggleState == ToggleState.On || rbtnRequestHeader.ToggleState == ToggleState.On || rbtnResponseBody.ToggleState == ToggleState.On || rbtnResponseHeader.ToggleState == ToggleState.On)
            {
                ddlCondition.Enabled = txtText.Enabled = true;
            }
            else if (rbtnHasVariable.ToggleState == ToggleState.On)
            {
                txtHasVariable.Enabled = btnVariableSelector.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_flagRequest == null)
            {
                _flagRequest = RepositoryXml.GetInstance().doc.CreateElement("flagrequest");
            }
            else
            {
                _flagRequest.Attributes.RemoveAll();
            }
            _flagRequest.Attributes.Append(reposioryXml.GetAttribute("type", GetType()));
            _flagRequest.Attributes.Append(reposioryXml.GetAttribute("condition", ddlCondition.Text.Replace(" ",string.Empty).ToLower()));
            _flagRequest.Attributes.Append(reposioryXml.GetAttribute("text", txtText.Text));
            _flagRequest.Attributes.Append(reposioryXml.GetAttribute("hasvariablename", txtHasVariable.Text));
        }
      
        private string GetType()
        {
            if (rbtnNone.ToggleState == ToggleState.On)
            {
                return "none";
            }
            else if (rbtnRequestHeader.ToggleState == ToggleState.On)
            {
                return "requestheader";
            }
            else if (rbtnRequestBody.ToggleState == ToggleState.On)
            {
                return "requestbody";
            }
            else if (rbtnResponseHeader.ToggleState == ToggleState.On)
            {
                return "responseheader";
            }
            else if (rbtnResponseBody.ToggleState == ToggleState.On)
            {
                return "responsebody";
            }
            else if (rbtnHasVariable.ToggleState == ToggleState.On)
            {
                return "hasvariable";
            }
            else if (rbtnHasError.ToggleState == ToggleState.On)
            {
                return "haserror";
            }
            else if (rbtnDisabled.ToggleState == ToggleState.On)
            {
                return "disabled";
            }
            else
            {
                return string.Empty;
            }
        }

        private void SetType(string type)
        {
            switch (type)
            {
                case "none":
                    {
                        rbtnNone.ToggleState = ToggleState.On;
                        break;
                    }
                case "requestheader":
                    {
                        rbtnRequestHeader.ToggleState = ToggleState.On;
                        break;
                    }
                case "requestbody":
                    { 
                        rbtnRequestBody.ToggleState = ToggleState.On;
                        break;
                    }
                case "responseheader":
                    {
                        rbtnResponseHeader.ToggleState = ToggleState.On;
                        break;
                    }
                case "responsebody":
                    { 
                        rbtnResponseBody.ToggleState = ToggleState.On;
                        break;
                    }
                case "hasvariable":
                    {
                       rbtnHasVariable.ToggleState = ToggleState.On;
                         break;
                    }
                case "haserror":
                    {
                        rbtnHasError.ToggleState = ToggleState.On;
                        break;
                    }
                case "disabled":
                    {
                        rbtnDisabled.ToggleState = ToggleState.On;
                        break;
                    }
                default:
                    rbtnNone.ToggleState=ToggleState.On;
                    break;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
