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
    public partial class frmTCPIPParam : Telerik.WinControls.UI.RadForm
    {
       
        XmlNode parms;
        XmlNode param;
        RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        public frmTCPIPParam(XmlNode _parms,XmlNode _parm)
        {
            InitializeComponent();
            param = _parm;
            parms = _parms;
            if (param != null) SetParm(param);
            ddlPaddingType.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (param == null)
            {
                param = _repositoryXml.doc.CreateElement("param");
            }
            else
            {
                param.Attributes.RemoveAll();
            }
            param.Attributes.Append(_repositoryXml.GetAttribute("name", txtParamName.Text));
            param.Attributes.Append(_repositoryXml.GetAttribute("startposition", txtStartPosition.Text));
            param.Attributes.Append(_repositoryXml.GetAttribute("length", txtLength.Text));
            param.Attributes.Append(_repositoryXml.GetAttribute("paddingtype", ddlPaddingType.Text));
            param.Attributes.Append(_repositoryXml.GetAttribute("paddingchar", txtPaddingChar.Text));
            param.Attributes.Append(_repositoryXml.GetAttribute("value", txtValue.Text));
            parms.AppendChild(param);
            
            MessageBox.Show("Saved");
            Clear();
            txtParamName.Focus();

        }
        private void SetParm(XmlNode parm)
        {
            txtParamName.Text = param.Attributes["name"].Value;
            txtStartPosition.Text = param.Attributes["startposition"].Value;
            txtLength.Text = param.Attributes["length"].Value;
            ddlPaddingType.SelectedText = param.Attributes["paddingtype"].Value;
            txtPaddingChar.Text = param.Attributes["paddingchar"].Value;
            txtValue.Text = param.Attributes["value"].Value;
        }
      
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Clear()
        {
            txtParamName.Text = txtLength.Text = txtPaddingChar.Text = txtStartPosition.Text = txtValue.Text = string.Empty;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnVariable_Click(object sender, EventArgs e)
        {
            RequestParameter frm = new RequestParameter("Value", txtParamName.Text, parms);
            frm.ShowDialog();
            txtValue.Text = frm._resultValue;
        }

        private void txtStartPosition_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtStartPosition_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)==false && char.IsDigit(e.KeyChar) == false) e.Handled = true;
        }

        private void txtLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) == false && char.IsDigit(e.KeyChar) == false) e.Handled = true;
        } 

    }
}
