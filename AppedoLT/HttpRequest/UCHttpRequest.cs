﻿using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using Telerik.WinControls.UI;

namespace AppedoLT
{
    public partial class UCHttpRequest : UserControl
    {

        private FindDig _fDig = null;
        private DataTable _paramTable = new DataTable();
        private DataTable _headerTable = new DataTable();
        private RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        private XmlNode _request;
        private RadTreeNode _treeNode = null;
        private static UCHttpRequest _instance;
        private Constants _constant = Constants.GetInstance();
        public static UCHttpRequest GetInstance()
        {
            if (_instance == null)
                _instance = new UCHttpRequest();
            return _instance;
        }

        private UCHttpRequest()
        {
            InitializeComponent();
            tabiRequestParameters.Select();
            ddlMethod.DataSource = _constant.HttpMethods;
            ddlPostContentType.DataSource = _constant.HttpPostContentType;
        }

        public UCHttpRequest GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.SetRequest(xmlNode);
            this.Tag = xmlNode;
            this.Dock = DockStyle.Fill;
            return this;
        }

        private void SetRequest(XmlNode request)
        {
            try
            {
                XmlNode param;
                _request = request;
                ddlMethod.Enabled = false;
                ddlPostContentType.Enabled = false;
                ddlMethod.SelectedItem = _request.Attributes["Method"].Value;
                updatePostType();
                param = request.SelectSingleNode("headers");
                _headerTable = new DataTable();
                _headerTable.Columns.Add("name");
                _headerTable.Columns.Add("value");
                _headerTable.Columns.Add("node", typeof(XmlNode));
                if (param != null)
                {
                    foreach (XmlNode para in param.ChildNodes)
                    {
                        _headerTable.Rows.Add(para.Attributes["name"].Value, para.Attributes["value"].Value, para);
                    }
                }
                dgvHeader.DataSource = _headerTable;
                object selectedTab = radTabStrip2.SelectedTab;
                txtUrl.Text = _request.Attributes["Url"].Value;
                txtUrl.Tag = _request.Attributes["Url"];
                ddlMethod.Tag = _request.Attributes["Method"];
                txtServer.Text = _request.Attributes["Host"].Value;
                txtServer.Tag = _request.Attributes["Host"];
                txtPath.Text = _request.Attributes["Path"].Value;
                txtPath.Tag = _request.Attributes["Path"];
                try
                {
                    txtRequest.Text = Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Request\\" + _request.Attributes["reqFilename"].Value);
                    txtResponse.Text = _request.Attributes["ResponseHeader"].Value + Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Response\\" + _request.Attributes["resFilename"].Value);
                    webBrowserResponse.DocumentText = txtResponse.Text;
                    pictureBox1.Image = Image.FromFile(Constants.GetInstance().ExecutingAssemblyLocation + "\\Response\\" + _request.Attributes["resFilename"].Value);
                    tabiResponseImage.Visibility =Telerik.WinControls.ElementVisibility.Visible;
                    tabWebBrowser.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                    if (radTabStrip2.SelectedTab == tabWebBrowser || radTabStrip2.SelectedTab == tabiResponseImage)
                    {
                        selectedTab = tabWebBrowser;
                    }

                }
                catch (OutOfMemoryException ex)
                {
                    tabiResponseImage.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                    tabWebBrowser.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                    if (radTabStrip2.SelectedTab == tabWebBrowser || radTabStrip2.SelectedTab == tabiResponseImage)
                    {
                        selectedTab = tabWebBrowser;
                    }

                }
                catch
                {

                }
                chkEnable.Checked = Convert.ToBoolean(_request.Attributes["IsEnable"].Value);
                chkEnable.Tag = _request.Attributes["IsEnable"];
                ((Telerik.WinControls.RadItem)selectedTab).Select();
                tabiRequestParameters.ContentPanel.Controls.Clear();
                tabiRequestParameters.ContentPanel.Controls.Add(UCRequestData.GetInstance(request));
                LoadExtParameters();
                LoadAssertion();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void LoadExtParameters()
        {
            try
            {
                DataTable ExtParamTable = new DataTable();
                ExtParamTable.Columns.Add("Name");
                ExtParamTable.Columns.Add("Value");
                ExtParamTable.Columns.Add("node", typeof(XmlNode));

                foreach (XmlNode variable in _request.SelectNodes("extractor"))
                {
                    ExtParamTable.Rows.Add(variable.Attributes["name"].Value, variable.Attributes["regex"].Value, variable);
                }
                dgvExtractor.DataSource = ExtParamTable;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void gvRequestParameters_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                DataGridView.HitTestInfo hit = btnEdit.HitTest(e.X, e.Y);
                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0)
                {
                    XmlNode paramNode = (XmlNode)btnEdit.Rows[hit.RowIndex].Cells[2].Value;
                    RequestParameter var = new RequestParameter(paramNode);
                    if (var.ShowDialog() == DialogResult.OK)
                    {
                        _paramTable.Rows[hit.RowIndex]["name"] = paramNode.Attributes["name"].Value;
                        _paramTable.Rows[hit.RowIndex]["value"] = paramNode.Attributes["value"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void txtResponse_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
                {
                    _fDig = new FindDig();
                    _fDig.SetTextBox(txtResponse);
                    _fDig.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void dgvHeader_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                DataGridView.HitTestInfo hit = dgvHeader.HitTest(e.X, e.Y);
                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0)
                {
                    XmlNode headerNode = (XmlNode)dgvHeader.Rows[hit.RowIndex].Cells[2].Value;
                    RequestParameter var = new RequestParameter(headerNode);
                    if (var.ShowDialog() == DialogResult.OK)
                    {
                        _headerTable.Rows[hit.RowIndex]["name"] = headerNode.Attributes["name"].Value;
                        _headerTable.Rows[hit.RowIndex]["value"] = headerNode.Attributes["value"].Value;

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            try
            {
                RequestParameter var = new RequestParameter((XmlAttribute)txtPath.Tag);
                if (var.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = ((XmlAttribute)txtPath.Tag).Value;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmDynamicRequestParameter var = new frmDynamicRequestParameter(_request1, loadScenario, "Parameter");
            //if (var.ShowDialog() == DialogResult.OK)
            //{
            //    paramTable.Rows.Clear();
            //    foreach (Parameter para in _request1.Parameters)
            //    {
            //        paramTable.Rows.Add(para.parameterid, para.Name, para.Value);
            //    }

            //    btnEdit.DataSource = paramTable;
            //    btnEdit.Columns[0].Visible = false;
            //    ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(loadScenario));
            //}
        }

        private void deleteParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //if (btnEdit.RowCount > 0)
                //{
                //    if (MessageBox.Show("Are You sure You want to delete selected Request Parameter?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //    {
                //        int index = 0;
                //        if (btnEdit.CurrentCell != null)
                //            index = btnEdit.CurrentCell.RowIndex;
                //        _request1.Parameters.Remove(_request1.Parameters.Find(f => f.parameterid == btnEdit.Rows[index].Cells[0].Value.ToString()));
                //        paramTable.Rows.RemoveAt(index);
                //        btnEdit.DataSource = paramTable;
                //        btnEdit.Columns[0].Visible = false;
                //        ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(loadScenario));
                //    }
                //}
                //else
                //    MessageBox.Show("No Request Parameters Found");
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addHeadertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmDynamicRequestParameter var = new frmDynamicRequestParameter(_request1, loadScenario, "Header");
            //if (var.ShowDialog() == DialogResult.OK)
            //{
            //    headerTable.Rows.Clear();
            //    foreach (Header para in _request1.Headers)
            //    {
            //        headerTable.Rows.Add(para.headerid, para.Name, para.Value);
            //    }

            //    dgvHeader.DataSource = headerTable;
            //    dgvHeader.Columns[0].Visible = false;
            //    ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(loadScenario));
            //}
        }

        private void deleteHeadertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvHeader.RowCount > 0)
                {
                    if (MessageBox.Show("Are You sure You want to delete selected Header Parameter?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int index = 0;
                        if (dgvHeader.CurrentCell != null)
                            index = dgvHeader.CurrentCell.RowIndex;
                        //_request1.Headers.Remove(_request1.Headers.Find(f => f.headerid == dgvHeader.Rows[index].Cells[0].Value.ToString()));
                        //headerTable.Rows.RemoveAt(index);
                        //dgvHeader.DataSource = headerTable;
                        //dgvHeader.Columns[0].Visible = false;
                        //ExceptionHandler.WriteRepository(Utility.SerializeObjectToXML(loadScenario));
                    }
                }
                else
                    MessageBox.Show("No Header Parameters Found");
            }
            catch (Exception Ex)
            {
            }
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                frmExtrator ext = new frmExtrator(txtResponse.Text, _request, null);
                if (ext.ShowDialog() == DialogResult.OK)
                {
                    LoadExtParameters();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvExtractor.SelectedRows.Count > 0)
                {

                    frmExtrator ext = new frmExtrator(txtResponse.Text, _request, (XmlNode)dgvExtractor.SelectedRows[0].Cells[2].Value);
                    if (ext.ShowDialog() == DialogResult.OK)
                    {
                        LoadExtParameters();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvExtractor.SelectedRows.Count > 0 && MessageBox.Show("Are you sure you want to delete selected extrator?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _request.RemoveChild((XmlNode)dgvExtractor.SelectedRows[0].Cells[2].Value);
                    LoadExtParameters();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void txt_Validated(object sender, EventArgs e)
        {
            RadTextBox txt = (RadTextBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value)
            {
                attr.Value = txt.Text;
            }

        }

        private void chkEnable_Leave(object sender, EventArgs e)
        {
            XmlAttribute attr = (XmlAttribute)chkEnable.Tag;
            attr.Value = chkEnable.Checked.ToString();
        }

        private void btnAssertionAdd_Click(object sender, EventArgs e)
        {
            if (_request.SelectSingleNode("assertions") == null)
            {
                XmlNode assertions = _repositoryXml.doc.CreateElement("assertions");
                frmAssertion parm = new frmAssertion(assertions, null);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                    _request.AppendChild(assertions);
                    LoadAssertion();
                }
            }
            else
            {
                XmlNode assertions = _request.SelectSingleNode("assertions");
                frmAssertion parm = new frmAssertion(assertions, null);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                    LoadAssertion();
                }
            }
        }
        private void LoadAssertion()
        {
            lsvAssertion.Items.Clear();
            if (_request.SelectSingleNode("assertions") != null)
            {
                foreach (XmlNode node in _request.SelectNodes("assertions/assertion"))
                {
                    ListViewItem item = new ListViewItem();
                    item.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    item.Tag = node;
                    item.Text = node.Attributes["name"].Value;
                    item.SubItems.AddRange(new string[] { node.Attributes["condition"].Value, node.Attributes["type"].Value, node.Attributes["text"].Value });
                    lsvAssertion.Items.Add(item);
                }
            }
        }

        private void btnAssertionEdit_Click(object sender, EventArgs e)
        {
            if (lsvAssertion.SelectedItems.Count > 0)
            {
                frmAssertion parm = new frmAssertion(_request.SelectSingleNode("assertions"), (XmlNode)lsvAssertion.SelectedItems[0].Tag);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                    LoadAssertion();
                }
            }
        }

        private void btnAssertionDelete_Click(object sender, EventArgs e)
        {
            if (lsvAssertion.CheckedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete selected assertion", "delete", MessageBoxButtons.YesNo) == DialogResult.Yes && lsvAssertion.SelectedItems != null)
                {
                    try
                    {
                        foreach (ListViewItem item in lsvAssertion.CheckedItems)
                        {
                            _request.SelectSingleNode("assertions").RemoveChild((XmlNode)item.Tag);
                            lsvAssertion.Items.Remove(item);
                        }
                        LoadAssertion();

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void updatePostType()
        {
            if (ddlMethod.Text == "POST")
            {
                lblContentType.Visible = true;
                ddlPostContentType.Visible = true;
                XmlNode param = _request.SelectSingleNode("./params");
                if (param != null)
                {
                    if (param.Attributes["type"].Value == "multipart/form-data")
                    {
                        ddlPostContentType.SelectedIndex = 1;
                    }
                    else if (param.Attributes["type"].Value == "text")
                    {
                        ddlPostContentType.SelectedIndex = 2;
                    }
                    else
                    {
                        ddlPostContentType.SelectedIndex = 0;
                    }
                }
                else
                {
                    ddlPostContentType.SelectedIndex = 0;
                }
            }
            else
            {
                lblContentType.Visible = false;
                ddlPostContentType.Visible = false;
            }
        }
    }
}