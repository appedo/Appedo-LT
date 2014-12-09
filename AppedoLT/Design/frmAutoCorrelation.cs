using System;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using Telerik.WinControls.UI;
using System.Threading;
using System.ComponentModel;
using Telerik.WinControls;
namespace AppedoLT
{
    public partial class frmAutoCorrelation : Telerik.WinControls.UI.RadForm
    {
        //VUScript objVUScript = new VUScript();
        //public frmAutoCorrelation(VUScript _objVUScript)
        //{
        //    objVUScript = _objVUScript;
        //    InitializeComponent();
        //    GridViewCheckBoxColumn checkBoxColumn = new GridViewCheckBoxColumn();
        //    checkBoxColumn.Width = 1;
        //    checkBoxColumn.DataType = typeof(Boolean);
        //    checkBoxColumn.FieldName = "Select";
        //    checkBoxColumn.HeaderText = "";
        //    GridViewTextBoxColumn txtBoxColumnName = new GridViewTextBoxColumn();
        //    txtBoxColumnName.DataType = typeof(String);
        //    txtBoxColumnName.FieldName = "ParamName";
        //    txtBoxColumnName.HeaderText = "Parameter Name";
        //    GridViewTextBoxColumn txtBoxColumnValue = new GridViewTextBoxColumn();
        //    txtBoxColumnValue.DataType = typeof(String);
        //    txtBoxColumnValue.FieldName = "ParamValue";
        //    txtBoxColumnValue.HeaderText = "Parameter Value";
        //    grdAutoCorrelation.MasterGridViewTemplate.Columns.Add(checkBoxColumn);
        //    grdAutoCorrelation.MasterGridViewTemplate.Columns.Add(txtBoxColumnName);
        //    grdAutoCorrelation.MasterGridViewTemplate.Columns.Add(txtBoxColumnValue);
        //    new Thread(GridDataBind).Start();
        //}
        //private void btnOk_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        objVUScript.ScriptAutoParameters.Clear();
        //        for (int i = 0; i < grdAutoCorrelation.Rows.Count; i++)
        //        {
        //            if (grdAutoCorrelation.Rows[i].Cells[0].Value.ToString() == "True")
        //                FindFiles(objVUScript.Containers, grdAutoCorrelation.Rows[i].Cells[1].Value.ToString(), grdAutoCorrelation.Rows[i].Cells[2].Value.ToString());
        //            ScriptAutoParameter objScriptAutoParameter = new ScriptAutoParameter();
        //            objScriptAutoParameter.ParamName = grdAutoCorrelation.Rows[i].Cells[1].Value.ToString();
        //            objScriptAutoParameter.ParamValue = grdAutoCorrelation.Rows[i].Cells[2].Value.ToString();
        //            objScriptAutoParameter.ParamChecked = grdAutoCorrelation.Rows[i].Cells[0].Value != DBNull.Value ? Convert.ToBoolean(grdAutoCorrelation.Rows[i].Cells[0].Value) : false;
        //            objVUScript.ScriptAutoParameters.Add(objScriptAutoParameter);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
        //    }
        //    this.DialogResult = DialogResult.OK;
        //}

        //private void btnCancel_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}
        //public void FindFiles(List<Container> lstContainers, String ParamName, String ParamValue)
        //{
        //    try
        //    {
        //        string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        foreach (Container objContainer in lstContainers)
        //        {
        //            if (objContainer.Type == "1")
        //            {
        //                FindFiles(objContainer.Containers, ParamName, ParamValue);
        //            }
        //            else if (objContainer.Type == "2")
        //            {
        //                foreach (Request objRequest in objContainer.Requests)
        //                {
        //                    foreach (Parameter para in objRequest.Parameters)
        //                    {
        //                        if (para.Name == ParamName)
        //                            para.Value = ParamValue;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
        //    }
        //}
        //public void GridDataBind()
        //{
        //    try
        //    {
        //        foreach (ScriptAutoParameter obj in objVUScript.ScriptAutoParameters)
        //        {
        //            grdAutoCorrelation.Rows.Add(obj.ParamChecked, obj.ParamName, obj.ParamValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
        //    }
        //}

        //private void grdAutoCorrelation_CellEndEdit(object sender, GridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ColumnIndex == 1)
        //        {
        //            int intRownIndex = 0;
        //            if (e.RowIndex.ToString() == "-1")
        //                intRownIndex = grdAutoCorrelation.RowCount - 1;
        //            else
        //                intRownIndex = e.RowIndex;
        //            for (int i = 0; i < grdAutoCorrelation.RowCount; i++)
        //            {
        //                if (i.ToString() != intRownIndex.ToString() && e.Value.ToString() == grdAutoCorrelation.Rows[i].Cells[1].Value.ToString())
        //                {
        //                    MessageBox.Show("Parameter Name Already Exists","",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //                    grdAutoCorrelation.CurrentCell.Value = String.Empty;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
        //    }
        //}

    }
}
