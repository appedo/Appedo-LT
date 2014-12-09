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
    public partial class UCQueryStringData : UserControl
    {
        private static UCQueryStringData _instance;
        DataTable _paramTable = null;
        public static UCQueryStringData GetInstance(XmlNode queryData)
        {
            if (_instance == null)
                _instance = new UCQueryStringData();
            _instance.SetValue(queryData);
            return _instance;
        }
        private UCQueryStringData()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }
        private void SetValue(XmlNode queryData)
        {
            _paramTable = new DataTable();
            _paramTable.Columns.Add("name");
            _paramTable.Columns.Add("value");
            _paramTable.Columns.Add("node", typeof(XmlNode));
            if (queryData != null)
            {
                foreach (XmlNode para in queryData.ChildNodes)
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
