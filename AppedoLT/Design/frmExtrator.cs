using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace AppedoLT
{

    /// <summary>
    /// Form used to give UI for extractor. Extract value from response.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class frmExtractor : Telerik.WinControls.UI.RadForm
    {
        string _response = string.Empty;
        XmlNode _editVariable=null;
        XmlNode _request = null;
        Common _common = Common.GetInstance();

        /// <summary>
        /// Create extractor form object.
        /// </summary>
        /// <param name="response">Response string</param>
        /// <param name="request">Request xml node</param>
        /// <param name="extractVariable"><Extractor xml node/param>
        public frmExtractor(string response,XmlNode request, XmlNode extractVariable)
        {
            InitializeComponent();
            _editVariable = extractVariable;
            _request = request;
            _response = txtResponse.Text = response;
            if (extractVariable != null) setExtractorVar(extractVariable);
        }

        /// <summary>
        /// Populate data into corresponding field from xml node.
        /// </summary>
        /// <param name="extractVar">Extract xml node</param>
        public void setExtractorVar(XmlNode extractVar)
        {
            txtName.Text = extractVar.Attributes["name"].Value;
            txtStartwith.Text =extractVar.Attributes["start"].Value;
            txtEndwith.Text = extractVar.Attributes["end"].Value;
            //Mode =1 it is simple mode, else it is advanced mode that is regular exp
            if (extractVar.Attributes["mode"].Value == "1")
            {
                rbtnSimple.Checked = true;
            }
            else
            {
                rbtnAdvanced.Checked = true;
            }
            if (extractVar.Attributes["selctiontype"].Value == "all")
                rbtnAllOccurrence.Checked = true;
            else if (extractVar.Attributes["selctiontype"].Value == "random")
                rbtnRandomOccurrence.Checked = true;
            else if (extractVar.Attributes["selctiontype"].Value == "single")
            {
                rbtnSingleOccurrence.Checked = true;
                txtOrdinal.Text = extractVar.Attributes["ordinal"].Value;
                txtOrdinal.Visible = true;
                lblOrdinal.Visible = true;
            }
            txtRegEx.Text = extractVar.Attributes["regex"].Value;
            txtGroupIndex.Text=extractVar.Attributes["groupindex"].Value;
            ShowResult();
        }
        private void btnStartWith_Click(object sender, EventArgs e)
        {
            frmShowText st = new frmShowText(_response);
            if (st.ShowDialog() == DialogResult.OK && st.txtResponse.SelectedText!=null)
            {
                txtStartwith.Text =st.txtResponse.SelectedText;
            }
        }
        private void btnEndWith_Click(object sender, EventArgs e)
        {
            frmShowText st = new frmShowText(_response);
            if (st.ShowDialog() == DialogResult.OK && st.txtResponse.SelectedText!=null)
            {
                txtEndwith.Text =st.txtResponse.SelectedText;
            }
        }

        /// <summary>
        /// Get extractor xml node to store
        /// </summary>
        /// <param name="ExtractVar">Contain extractor variable info</param>
        /// <returns></returns>
        private XmlNode GetExtratorVar(XmlNode ExtractVar)
        {
            if (ExtractVar == null)
            {
                ExtractVar =_request.OwnerDocument.CreateElement("extractor");
            }
            else
            {
                ExtractVar.Attributes.RemoveAll();
            }
            ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"name",txtName.Text));
            ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"start",txtStartwith.Text));
            ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"end", txtEndwith.Text));
            if (rbtnSimple.Checked == true) ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"mode", "1"));
            else
            {
                ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"mode", "2"));
            }
            if (rbtnAllOccurrence.Checked == true)
            {
                ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"selctiontype", "all"));
            }
            else if (rbtnRandomOccurrence.Checked == true)
            {
                ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"selctiontype", "random"));
            }
            else
            {
                ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"selctiontype", "single"));
            }
            ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"regex", txtRegEx.Text));
            ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"groupindex", txtGroupIndex.Text));
            if (txtOrdinal.Text == string.Empty) ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"ordinal","1"));
            else
            {
                ExtractVar.Attributes.Append(_common.GetAttribute(_request.OwnerDocument,"ordinal", txtOrdinal.Text));
            }
            return ExtractVar;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Name required");
                this.DialogResult = DialogResult.None;
            }
            else if (rbtnSingleOccurrence.Checked == true && txtOrdinal.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter ordianl");
                txtOrdinal.Focus();
            }
            else
            {
                if (_editVariable == null)
                {
                    _request.AppendChild(GetExtratorVar(null));
                }
                else
                {
                    GetExtratorVar(_editVariable);
                }
                
                this.Close();
            }
        }
        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) == true || char.IsLetterOrDigit(e.KeyChar) == true || e.KeyChar == '_'))
            {
                e.Handled = true;
            }
        }
        private void btnShowResult_Click(object sender, EventArgs e)
        {
            ShowResult();
        }
        private void txtStartwith_Enter(object sender, EventArgs e)
        {
            txtStartwith.Tag = txtStartwith.Text;
        }
        private void txtEndwith_Enter(object sender, EventArgs e)
        {
            txtEndwith.Tag = txtEndwith.Text;
        }
        private void txtOrdinal_Enter(object sender, EventArgs e)
        {
            txtOrdinal.Tag = txtOrdinal.Text;
        }
        private void txtOrdinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void rbtnSimple_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnSimple.Checked == true)
            {
                txtRegEx.Text = string.Empty;
                txtGroupIndex.Text = "1";
                SetReg();
                txtRegEx.Enabled = false;
                txtGroupIndex.Enabled = false;
                btnStartWith.Enabled = txtStartwith.Enabled = true;
                btnEndWith.Enabled = txtEndwith.Enabled = true;
            }
            else
            {
                txtRegEx.Enabled = true;
                txtGroupIndex.Enabled = true;
                btnStartWith.Text = btnEndWith.Text = string.Empty;
                btnStartWith.Enabled = txtStartwith.Enabled = false;
                btnEndWith.Enabled = txtEndwith.Enabled = false;
            }
        }

        /// <summary>
        /// Form regular exp from startwith and endwith.
        /// 
        /// </summary>
        private void SetReg()
        {
            if (rbtnSimple.Checked == true)
            {
                txtRegEx.Text = Regex.Escape(txtStartwith.Text) + "(.*?)" + Regex.Escape(txtEndwith.Text);
            }
        }
        private void txtLeave(object sender, EventArgs e)
        {
            if (rbtnSingleOccurrence.Checked == true)
            {
                txtOrdinal.Visible = true;
                lblOrdinal.Visible = true;
            }
            else
            {
                txtOrdinal.Visible = false;
                lblOrdinal.Visible = false;
            }
            if (txtGroupIndex.Text == string.Empty)
            {
                txtGroupIndex.Text = "1";
            }
            ShowResult();
        }
        private void txtStartwith_TextChanged(object sender, EventArgs e)
        {
            SetReg();
        }
        private void txtEndwith_TextChanged(object sender, EventArgs e)
        {
            SetReg();
        }

        /// <summary>
        /// Show result from given input(response text and reqular exp)
        /// </summary>
        private void ShowResult()
        {
            string startWith = Regex.Escape(txtStartwith.Text);
            string endWith = Regex.Escape(txtEndwith.Text);
            int ordinal = 0;
            int groupIndex = int.Parse(txtGroupIndex.Text);

            MatchCollection match = Regex.Matches(_response, txtRegEx.Text, RegexOptions.Singleline | RegexOptions.Multiline);
            txtResult.Text = string.Empty;
            if (rbtnSingleOccurrence.Checked == true && int.TryParse(txtOrdinal.Text, out ordinal) == true)
            {
                for (int index = 0; index < match.Count; index++)
                {
                    if (index + 1 == ordinal)
                    {
                        txtResult.Text += match[index].Groups[groupIndex].Value;
                        break;
                    }
                }
            }
            else
            {
                for (int index = 0; index < match.Count; index++)
                {
                    txtResult.Text += match[index].Groups[groupIndex].Value + ",";
                }
                if (match.Count > 0 && txtResult.Text.Length > 0) txtResult.Text = txtResult.Text.Remove(txtResult.Text.Length - 1);
            }

        }
        private void txtGroupIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

    }
}
