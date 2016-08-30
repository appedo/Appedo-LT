using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace AppedoLT
{
    public partial class UCHttpRequest : UserControl
    {

        private FindDig _fDig = null;
        private DataTable _paramTable = new DataTable();
        private DataTable _headerTable = new DataTable();
        private XmlNode _request;
        private RadTreeNode _treeNode = null;
        private static UCHttpRequest _instance;
        private Constants _constant = Constants.GetInstance();
        string _scriptId = string.Empty;
        private bool bRequestChanged = false;

        Regex imageTest = new Regex(@"\.(jpg|JPG|gif|GIF|jpeg|JPEG|png|PNG)$");
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
            _scriptId = xmlNode.OwnerDocument.SelectSingleNode("//vuscript").Attributes["id"].Value;
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
                ddlPostContentType.Enabled = true;
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
                txtSchema.Text = _request.Attributes["Schema"].Value;
                txtSchema.Tag = _request.Attributes["Schema"];
                txtServer.Text = _request.Attributes["Host"].Value;
                txtServer.Tag = _request.Attributes["Host"];
                txtPath.Text = _request.Attributes["Path"].Value;
                txtPath.Tag = _request.Attributes["Path"];
                radTxtPort.Text = _request.Attributes["Port"].Value;
                try
                {
                    txtRequest.Text = Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + _scriptId + "\\" + _request.Attributes["reqFilename"].Value);
                    if (_request.Attributes["ResponseHeader"]!=null)
                    {
                        txtResponse.Text = _request.Attributes["ResponseHeader"].Value + Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + _scriptId + "\\" + _request.Attributes["resFilename"].Value);
                    }
                    else
                    {
                        txtResponse.Text = Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + _scriptId + "\\" + _request.Attributes["resFilename"].Value);
                    }
                    webBrowserResponse.DocumentText = txtResponse.Text;
                    if (imageTest.IsMatch(_request.Attributes["resFilename"].Value))
                    {
                        MemoryStream ms = GetStreamFromFile((Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + _scriptId + "\\" + _request.Attributes["resFilename"].Value));
                        pictureBox1.Image = Image.FromStream(ms);
                        tabiResponseImage.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                        tabWebBrowser.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        if (radTabStrip2.SelectedTab == tabWebBrowser || radTabStrip2.SelectedTab == tabiResponseImage)
                        {
                            selectedTab = tabWebBrowser;
                        }
                    }
                    else
                    {
                        tabiResponseImage.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        tabWebBrowser.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                        if (radTabStrip2.SelectedTab == tabWebBrowser || radTabStrip2.SelectedTab == tabiResponseImage)
                        {
                            selectedTab = tabWebBrowser;
                        }
                    }

                }
                catch
                {
                    tabiResponseImage.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                    tabWebBrowser.Visibility = Telerik.WinControls.ElementVisibility.Visible;
                    if (radTabStrip2.SelectedTab == tabWebBrowser || radTabStrip2.SelectedTab == tabiResponseImage)
                    {
                        selectedTab = tabWebBrowser;
                    }
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

        private MemoryStream GetStreamFromFile(string fileName)
        {
            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);
                ms.Write(buffer, 0, (int)file.Length);
            }
            return ms;
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
                        Control obj = this.Parent;
                        while (obj != null)
                        {
                            obj = obj.Parent;
                            if (obj.GetType().Name == "ucDesign") break;
                        }
                        if (obj != null)
                        {
                            if (var.chkReplaceAll.Checked)
                            {
                                ((ucDesign)obj).btnScriptSave_Click(null, null);
                                ((ucDesign)obj).LoadTreeItem();
                            }
                        }
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
                        Control obj = this.Parent;
                        while (obj != null)
                        {
                            obj = obj.Parent;
                            if (obj.GetType().Name == "ucDesign") break;
                        }
                        if (obj != null)
                        {
                            if (var.chkReplaceAll.Checked)
                            {
                                ((ucDesign)obj).btnScriptSave_Click(null, null);
                                ((ucDesign)obj).LoadTreeItem();
                            }
                        }
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
                    Control obj = this.Parent;
                    while (obj != null)
                    {
                        obj = obj.Parent;
                        if (obj.GetType().Name == "ucDesign") break;
                    }
                    if (obj != null)
                    {
                        if (var.chkReplaceAll.Checked)
                        {
                            ((ucDesign)obj).btnScriptSave_Click(null, null);
                            ((ucDesign)obj).LoadTreeItem();
                        }
                    }
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
                frmExtractor ext = new frmExtractor(txtResponse.Text, _request, null);
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

                    frmExtractor ext = new frmExtractor(txtResponse.Text, _request, (XmlNode)dgvExtractor.SelectedRows[0].Cells[2].Value);
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
                XmlNode assertions = ((XmlNode)this.Tag).OwnerDocument.CreateElement("assertions");
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
                        bRequestChanged = true;
                        ddlPostContentType.SelectedIndex = 1;
                        bRequestChanged = false;
                    }
                    else if (param.Attributes["type"].Value == "text")
                    {
                        bRequestChanged = true;
                        ddlPostContentType.SelectedIndex = 2;
                        bRequestChanged = false;
                        
                    }
                    else
                    {
                        bRequestChanged = true;
                        ddlPostContentType.SelectedIndex = 0;
                        bRequestChanged = false;
                    }
                }
                else
                {
                    bRequestChanged = true;
                    ddlPostContentType.SelectedIndex = 0;
                    bRequestChanged = false;
                }
            }
            else
            {
                lblContentType.Visible = false;
                ddlPostContentType.Visible = false;
            }
        }
        /// <summary>
        /// This event function will trigger when the content type selection changed
        /// Based on selection of content type need to convert into the seleced formate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlPostContentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get user selected option & check it should not be the multiprt
            // if it is multipart then it should be message to end user like this selection is 
            // not available
            if (ddlPostContentType.SelectedValue.ToString() == "Multipart/form-data")
            {
                MessageBox.Show("This selection is not available");
            }
            else
            {
                if (_request != null && !bRequestChanged)
                {
                    // if content type text then convert the text into form formate
                    if (ddlPostContentType.SelectedValue.ToString() == "Text") 
                        convertFormToText();
                    else if (ddlPostContentType.SelectedValue.ToString() == "Form")
                        convertTextToForm();
                }
            }
            
        }
        /// <summary>
        /// This function to convert form parameters to "Text" type
        /// </summary>
        public void convertFormToText()
        {
            XmlDocument doc = new XmlDocument();
            StringBuilder sbParam = new StringBuilder("");
            try
            {
                XmlNode param = _request.SelectSingleNode("./params");

                if (param.ChildNodes.Count>0)
                {
                    for (int i = 0; i < param.ChildNodes.Count; i++)
                    {
                        if (i == 0)
                        {
                            sbParam.Append(param.ChildNodes[i].Attributes["name"].Value + "=" + param.ChildNodes[i].Attributes["value"].Value);
                        }
                        else
                        {
                            sbParam.Append("&" + param.ChildNodes[i].Attributes["name"].Value + "=" + param.ChildNodes[i].Attributes["value"].Value);
                        }
                    }

                    // create xml element with type "Text" 
                    XmlElement xeParams = doc.CreateElement("params");
                    xeParams.SetAttribute("type", "text");

                    XmlElement xeParam = doc.CreateElement("Param");
                    xeParam.SetAttribute("name", "Text");
                    xeParam.SetAttribute("rawname", "Text");
                    xeParam.SetAttribute("value", sbParam.ToString());
                    xeParams.AppendChild(xeParam);
                    doc.AppendChild(xeParams);
                    replaceParamElement(doc);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
       }
        /// <summary>
        /// To replace the parameter element with text/form
        /// </summary>
        /// <param name="doc"></param>
        public void replaceParamElement(XmlDocument doc)
        {
            try
            {
                // remove the form type parameter from this request
            string FILE_NAME = @"Scripts/"+_scriptId+"/vuscript.xml";
            XmlTextReader reader = new XmlTextReader(FILE_NAME);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(reader);
            reader.Close();
            
            //Select the cd node with the matching title
            XmlNode oldCd;
            XmlNode oldRequest;
            XmlElement root = xmldoc.DocumentElement;
            oldRequest = root.SelectSingleNode("//request[@id='" + _request.Attributes["id"].Value + "']");

            oldCd = oldRequest.SelectSingleNode("./params");

            oldRequest.RemoveChild(oldCd);
            XmlNode newBook = xmldoc.ImportNode(doc.DocumentElement, true);

            oldRequest.AppendChild(newBook);
            //save the output to a file
            xmldoc.Save(FILE_NAME);

            ucDesign.GetInstance().LoadTreeItem();

            this.SetRequest(oldRequest);
            }catch (Exception ex) {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            
        }
        /// <summary>
        /// This function to convert form parameters to "Form" type
        /// </summary>
        public void convertTextToForm()
        {
            try
            {
                StringBuilder sbParam = new StringBuilder("");
                XmlNode param = _request.SelectSingleNode("./params");

                XmlDocument doc = new XmlDocument();
                // create xml element with type "Text" 
                XmlElement xeParams = doc.CreateElement("params");
                xeParams.SetAttribute("type", "form");
                string[] strArry = null;
                for (int i = 0; i < param.ChildNodes.Count; i++)
                {
                    if (param.ChildNodes[i].Attributes["value"].Value != null)
                    {
                        
                        string[] strArrayParams = Regex.Split(param.ChildNodes[i].Attributes["value"].Value.Replace("Text=", ""), "&");


                        foreach (string str in strArrayParams)
                        {
                            string[] arrParamKeyValue = Regex.Split(str, "=");

                            if (arrParamKeyValue.Length > 1)
                            {
                                XmlElement xeParam = doc.CreateElement("Param");

                                // This is added to check if the Name valuue is null some time & is coming at the end and the arrayindexoutof bound error was throwing.
                                if (!string.IsNullOrEmpty(arrParamKeyValue[0]))
                                {
                                    xeParam.SetAttribute("name", arrParamKeyValue[0]);

                                    // Check for if the "=" split values size contain more than 2 
                                    //&smCourse_TSM=;;AjaxControlToolkit, Version=4.1.51116.0, Culture=neutral&

                                    if (arrParamKeyValue.Length > 2)
                                    {
                                        StringBuilder sbParam1 = new StringBuilder("");
                                        for (int j = 1; j < arrParamKeyValue.Length; j++)
                                        {
                                            if (j == 1)
                                            {
                                                sbParam1.Append(arrParamKeyValue[j]);
                                            }
                                            else
                                            {
                                                sbParam1.Append("=" + arrParamKeyValue[j]);
                                            }

                                        }

                                        xeParam.SetAttribute("value", sbParam1.ToString());
                                        xeParam.SetAttribute("rawname", sbParam1.ToString());
                                        xeParam.SetAttribute("rawvalue", "");
                                        xeParams.AppendChild(xeParam);
                                    }


                                    else
                                    {

                                        xeParam.SetAttribute("value", arrParamKeyValue[1]);
                                        xeParam.SetAttribute("rawname", arrParamKeyValue[1]);
                                        xeParam.SetAttribute("rawvalue", "");
                                        xeParams.AppendChild(xeParam);


                                    }


                                }
                            }

                        }
                    }

                }
                doc.AppendChild(xeParams);                
                replaceParamElement(doc);
                

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}