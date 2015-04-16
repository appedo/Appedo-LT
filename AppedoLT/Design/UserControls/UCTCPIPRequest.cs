using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;
using AppedoLT.Core;
namespace AppedoLT
{
    public partial class UCTCPIPRequest : UserControl
    {
        FindDig fDig = null;
        string requestid = string.Empty;
        XmlNode request=null;
        DataTable param = new DataTable();
        DataTable assertion = new DataTable();
        RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        private RadTreeNode _node;
        private RadTreeNode _treeNode = null;
        private static UCTCPIPRequest _instance;

        public static UCTCPIPRequest GetInstance()
        {
            if (_instance == null)
                _instance = new UCTCPIPRequest();
            return _instance;
        }

        public UCTCPIPRequest GetControl(XmlNode xmlNode, RadTreeNode treeNode)
        {
            _treeNode = treeNode;
            this.SetRequest(treeNode);
            this.Tag = xmlNode;
            this.Dock = DockStyle.Fill;
            return this;
        }

        public XmlNode Request
        {
            get
            {
                return request;
            }
            set
            {
                if (value != null)
                {
                    request = value;
                    requestid = request.Attributes["id"].Value;
                    txtServerIP.Text = request.Attributes["serverip"].Value;
                    txtPort.Text = request.Attributes["port"].Value;
                    txtRequstsize.Text = request.Attributes["requestsize"].Value;
                    txtResponsesize.Text = request.Attributes["responsesize"].Value;
                    txtResponse.Text = request.Attributes["responsecontent"].Value;
                    txtRequest.Text = request.Attributes["requestcontent"].Value;
                    txtResponseTime.Text = request.Attributes["responsetime"].Value;
                    txtName.Text = request.Attributes["name"].Value;
                    chkRequestSizeConstant.Checked =Convert.ToBoolean(request.Attributes["requestsizeconstant"].Value);
                    chkResponseSizeConstant.Checked =Convert.ToBoolean(request.Attributes["responsesizeconstant"].Value);
                    if (request.Attributes["enable"] == null)
                    {
                        request.Attributes.Append(_repositoryXml.GetAttribute("enable", true.ToString()));
                        
                    }
                    
                    chkEnable.Checked = Convert.ToBoolean(request.Attributes["enable"].Value);

                    txtServerIP.Tag = request.Attributes["serverip"];
                    txtPort.Tag = request.Attributes["port"];
                    txtRequstsize.Tag = request.Attributes["requestsize"];
                    txtResponsesize.Tag = request.Attributes["responsesize"];
                    txtResponse.Tag = request.Attributes["responsecontent"];
                    txtRequest.Tag = request.Attributes["requestcontent"];
                    txtResponseTime.Tag = request.Attributes["responsetime"];
                    txtName.Tag = request.Attributes["name"];
                    chkRequestSizeConstant.Tag = request.Attributes["requestsizeconstant"];
                    chkResponseSizeConstant.Tag = request.Attributes["responsesizeconstant"];
                    chkEnable.Tag = request.Attributes["enable"];

                    LoadParams();
                    LoadAssertion();
                    LoadExtParameters();
                }
            }
        }

        public UCTCPIPRequest()
        {
            InitializeComponent();
            param.Columns.Add("name");
            param.Columns.Add("startposition");
            param.Columns.Add("length");
            param.Columns.Add("paddingtype");
            param.Columns.Add("paddingchar");
            param.Columns.Add("value");
            param.Columns.Add("node",typeof(object));
            gvRequestParam.DataSource = param;
            gvRequestParam.AutoGenerateColumns = false;

            assertion.Columns.Add("reqtext");
            assertion.Columns.Add("reqposition");
            assertion.Columns.Add("reqlength");
            assertion.Columns.Add("type");
            assertion.Columns.Add("restext");
            assertion.Columns.Add("resposition");
            assertion.Columns.Add("reslength");
            assertion.Columns.Add("message");
            assertion.Columns.Add("node", typeof(object));

            gvAssertion.DataSource = assertion;
            gvAssertion.AutoGenerateColumns = false;
        }
        public void SetRequest(RadTreeNode node)
        {
            this.Request =(XmlNode) node.Tag;
            _node = node;
        }
        private void LoadParams()
        {
            param.Rows.Clear();
            foreach (XmlNode node in request.SelectNodes("params/param"))
            {
                param.Rows.Add(node.Attributes["name"].Value, node.Attributes["startposition"].Value, node.Attributes["length"].Value, node.Attributes["paddingtype"].Value, node.Attributes["paddingchar"].Value, node.Attributes["value"].Value, node);
               
            }
        }
        private void LoadAssertion()
        {
            assertion.Rows.Clear();
            if (request.SelectSingleNode("assertions") != null)
            {
                foreach (XmlNode node in request.SelectNodes("assertions/assertion"))
                {
                    assertion.Rows.Add(node.Attributes["reqtext"].Value, node.Attributes["reqposition"].Value, node.Attributes["reqlength"].Value, node.Attributes["type"].Value, node.Attributes["restext"].Value, node.Attributes["resposition"].Value, node.Attributes["reslength"].Value, node.Attributes["message"].Value, node);

                }
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

                foreach (XmlNode variable in request.SelectNodes("extractor"))
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
        private void btnAddParam_Click(object sender, EventArgs e)
        {
            frmTCPIPParam parm = new frmTCPIPParam(request.SelectNodes("params")[0],null);
            parm.ShowDialog();
            LoadParams();
        }
        private void btnRemoveParam_Click(object sender, EventArgs e)
        {
           
            if (MessageBox.Show("Are you sure you want to delete selected parameter","delete",MessageBoxButtons.YesNo)==DialogResult.Yes && gvRequestParam.SelectedRows != null)
            {
                try
                {
                    foreach (XmlNode node in request.SelectNodes("params/param"))
                    {
                        if (node.Attributes["name"].Value == gvRequestParam.SelectedRows[0].Cells["name"].Value.ToString())
                        {
                            request.FirstChild.RemoveChild(node);
                            gvRequestParam.Rows.Remove(gvRequestParam.SelectedRows[0]);
                            LoadParams();
                           
                            break;
                        }
                    }
                }
                catch(Exception ex)
                {
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                frmExtrator ext = new frmExtrator(txtResponse.Text, request,null);
                if ( ext.ShowDialog()==DialogResult.OK)
                {
                     LoadExtParameters();
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
                    request.RemoveChild((XmlNode)dgvExtractor.SelectedRows[0].Cells[2].Value);
                    LoadExtParameters();
                   
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void gvRequestParam_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
           if(gvRequestParam.SelectedRows.Count>0)
           {
               frmTCPIPParam parm = new frmTCPIPParam(request.SelectNodes("params")[0],(XmlNode) gvRequestParam.SelectedRows[0].Cells["node"].Value);
               parm.ShowDialog();
               LoadParams();
           }
        }
        private void txt_Leave(object sender, EventArgs e)
        {
            RadTextBox txt = (RadTextBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value)
            {
                attr.Value = txt.Text;
               
            }

        }
        private void btnReqParam_Click(object sender, EventArgs e)
        {
            RequestParameter frm = new RequestParameter("Request", txtRequest.Text,(XmlNode)this.Tag );
            if (frm.ShowDialog() == DialogResult.OK)
            {
                txtRequest.Text = frm._resultValue;
                txtRequest.Focus();
            }
        }

        private void btnAssertionAdd_Click(object sender, EventArgs e)
        {
            if (request.SelectSingleNode("assertions") == null)
            {
                XmlNode assertions = _repositoryXml.Doc.CreateElement("assertions");
                frmTcpAssertion parm = new frmTcpAssertion(assertions, null);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                    request.AppendChild(assertions);
                   
                    LoadAssertion();
                }
            }
            else
            {
                XmlNode assertions = request.SelectSingleNode("assertions");
                frmTcpAssertion parm = new frmTcpAssertion(assertions, null);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                    
                    LoadAssertion();
                }
            }
           
        }

        private void btnAssertionEdit_Click(object sender, EventArgs e)
        {
            if (gvAssertion.SelectedRows.Count > 0)
            {
                frmTcpAssertion parm = new frmTcpAssertion(request.SelectSingleNode("assertions"), (XmlNode)gvAssertion.SelectedRows[0].Cells[8].Value);
                if (parm.ShowDialog() == DialogResult.OK)
                {
                   
                    LoadAssertion();
                }
            }
        }

        private void btnAssertionDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete selected assertion", "delete", MessageBoxButtons.YesNo) == DialogResult.Yes && gvRequestParam.SelectedRows != null)
            {
                try
                {
                    request.SelectSingleNode("assertions").RemoveChild((XmlNode)gvAssertion.SelectedRows[0].Cells[8].Value);
                    gvAssertion.Rows.Remove(gvAssertion.SelectedRows[0]);
                    LoadAssertion();
                   

                }
                catch (Exception ex)
                {
                }
            }
        }


        private void chkRequestSizeConstant_Leave(object sender, EventArgs e)
        {
            RadCheckBox txt = (RadCheckBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value)
            {
                attr.Value = txt.Checked.ToString();
                
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvExtractor.SelectedRows.Count > 0)
                {

                    frmExtrator ext = new frmExtrator(txtResponse.Text, request,(XmlNode) dgvExtractor.SelectedRows[0].Cells[2].Value);
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

        private void txtName_Validated(object sender, EventArgs e)
        {
            RadTextBox txt = (RadTextBox)sender;
            XmlAttribute attr = (XmlAttribute)txt.Tag;
            if (txt.Text != attr.Value)
            {
                attr.Value = txt.Text;
                _node.Text = txt.Text;
              
            }
        }

        private void gvAssertion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
