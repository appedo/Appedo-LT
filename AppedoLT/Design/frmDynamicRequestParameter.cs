using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using AppedoLT.Core;
using AppedoLT.BusinessLogic;

namespace AppedoLT
{
    public partial class frmDynamicRequestParameter : Telerik.WinControls.UI.RadForm
    {
      
        String strHeaderORParam = String.Empty;
        ErrorProvider errName = new ErrorProvider();
        ErrorProvider errValue = new ErrorProvider();
        public frmDynamicRequestParameter()
        {
            InitializeComponent();
        }
        //public frmDynamicRequestParameter(Request _request, LoadScenario _loadScenario,String _strHeaderORParam)
        //{
        //    InitializeComponent();
        //    loadScenario = _loadScenario;
        //    request = _request;
        //    strHeaderORParam = _strHeaderORParam;
        //    if (strHeaderORParam.Equals("Parameter"))
        //    {
        //        this.Text = "Dynamic Request Parameters";
        //        lblName.Text = "Parameter Name : ";
        //        lblValue.Text = "Parameter Value : ";
        //        lblMethod.Visible = radioGET.Visible = radioPOST.Visible = radioMultipart.Visible =  true;
        //    }
        //    else if (strHeaderORParam.Equals("Header"))
        //    {
        //        this.Text = "Dynamic Headers";
        //        lblName.Text = "Header Name : ";
        //        lblValue.Text = "Header Value : ";
        //        lblMethod.Visible = radioGET.Visible = radioPOST.Visible = radioMultipart.Visible = false;
        //    }
        //}
        //private void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtParameterName.Text != String.Empty && txtParameterValue.Text != String.Empty)
        //        {
        //            errName.Clear();
        //            errValue.Clear();
        //            if (strHeaderORParam.Equals("Parameter"))
        //            {
        //                Parameter parameter = new Parameter();
        //                parameter.Name = txtParameterName.Text;
        //                parameter.Value = txtParameterValue.Text;
        //                parameter.Type = radioGET.IsChecked ? "GET" : radioPOST.IsChecked ? "POST" : "Multipart";
        //                request.Parameters.Add(parameter);
        //            }
        //            else if (strHeaderORParam.Equals("Header"))
        //            {
        //                Header header = new Header();
        //                header.Name = txtParameterName.Text;
        //                header.Value = txtParameterValue.Text;
        //                request.Headers.Add(header);
        //            }
        //            this.DialogResult = DialogResult.OK;
        //        }
        //        else if (txtParameterName.Text == String.Empty && txtParameterValue.Text != String.Empty)
        //        {
        //            errName.SetError(txtParameterName, "Please fill the required field");
        //            errValue.Clear();
        //        }
        //        else if (txtParameterName.Text != String.Empty && txtParameterValue.Text == String.Empty)
        //        {
        //            errValue.SetError(txtParameterValue, "Please fill the required field");
        //            errName.Clear();
        //        }
        //        else
        //        {
        //            errName.SetError(txtParameterName, "Please fill the required field");
        //            errValue.SetError(txtParameterValue, "Please fill the required field");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
        //    }
        //}
    }
}
