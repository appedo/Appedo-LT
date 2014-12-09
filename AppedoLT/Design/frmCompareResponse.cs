using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class frmCompareResponse : Telerik.WinControls.UI.RadForm
    {
        public frmCompareResponse(String strRecordedResponse, String strValidatedResponse)
        {
            InitializeComponent();
            txtRecordedResponse.Text = strRecordedResponse;
            txtValidatedResponse.Text = strValidatedResponse;
            compareResponses();
            txtRecordedResponse.SelectedText = String.Empty;
        }

        public void compareResponses()
        {
            try
            {
                List<string> myListRecordedResponse = new List<string>();
                List<string> myListValidatedResponse = new List<string>();
                for (int i = 0; i < txtRecordedResponse.Lines.Length; i++)
                {
                    myListRecordedResponse.Add(txtRecordedResponse.Lines[i]);
                }
                for (int i = 0; i < txtValidatedResponse.Lines.Length; i++)
                {
                    myListValidatedResponse.Add(txtValidatedResponse.Lines[i]);
                }
                for (int i = 0; i < myListRecordedResponse.Count; i++)
                {
                    if (myListRecordedResponse[i] != myListValidatedResponse[i])
                    {
                        myListValidatedResponse.Insert(i, Environment.NewLine);
                    }
                }
                String strTempValidatedResponse = String.Empty;
                for (int i = 0; i < myListValidatedResponse.Count; i++)
                {
                    if(myListValidatedResponse[i] == Environment.NewLine)
                        strTempValidatedResponse += myListValidatedResponse[i];
                    else
                        strTempValidatedResponse += myListValidatedResponse[i] + Environment.NewLine;
                }
                txtValidatedResponse.Text = strTempValidatedResponse;
                for (int i = 0; i < txtRecordedResponse.Lines.Length; i++)
                {
                    int countLineChar = txtRecordedResponse.Lines[i].Length;
                    int start_index = txtRecordedResponse.GetFirstCharIndexFromLine(i);
                    countLineChar += txtRecordedResponse.GetFirstCharIndexFromLine(i + 1) -
                    ((start_index + countLineChar - 1) + 1);
                    if (txtValidatedResponse.Lines[i] != txtRecordedResponse.Lines[i])
                    {
                        //int m = txtRecordedResponse.GetLineFromCharIndex(start_index);
                        //string currentlinetext = txtRecordedResponse.Lines[m];
                        txtRecordedResponse.Select(start_index, countLineChar);
                        txtRecordedResponse.SelectionBackColor = Color.YellowGreen;
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
