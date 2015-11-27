using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class UCFormPost : UserControl
    {
        private static UCFormPost _instance;
        DataTable _paramTable = null;
        public static UCFormPost GetInstance(XmlNode formData)
        {
            if (_instance == null)
                _instance = new UCFormPost();
            _instance.SetValue(formData);
            return _instance;
        }
        private UCFormPost()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }
        private void SetValue(XmlNode formData)
        {
            _paramTable = new DataTable();
            _paramTable.Columns.Add("name");
            _paramTable.Columns.Add("value");
            _paramTable.Columns.Add("node", typeof(XmlNode));
            if (formData != null)
            {
                foreach (XmlNode para in formData.ChildNodes)
                {
                    _paramTable.Rows.Add(para.Attributes["name"].Value, para.Attributes["value"].Value, para);
                }
            }
            dgvParam.DataSource = _paramTable;
        }

        private void dgvParam_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                DataGridView.HitTestInfo hit = dgvParam.HitTest(e.X, e.Y);
                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0)
                {
                    XmlNode paramNode = (XmlNode)dgvParam.Rows[hit.RowIndex].Cells[2].Value;
                    RequestParameter var = new RequestParameter(paramNode);
                    if (var.ShowDialog() == DialogResult.OK)
                    {
                        _paramTable.Rows[hit.RowIndex]["name"] = paramNode.Attributes["name"].Value;
                        _paramTable.Rows[hit.RowIndex]["value"] = paramNode.Attributes["value"].Value;

                        Control obj=this.Parent;
                        while (obj != null)
                        {
                            obj = obj.Parent;
                            if (obj.GetType().Name == "ucDesign") break; 
                        }
                        if (obj != null)
                        {
                            if (var.chkReplaceAll.Checked) { 
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
   }
}
