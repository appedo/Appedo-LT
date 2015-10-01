using AppedoLT.Core;
using System;
using System.Windows.Forms;

namespace AppedoLT
{
    /// <summary>
    /// Used to show response content for extraction.
    /// 
    ///  Author: Rasith
    /// </summary>
    public partial class frmShowText : Telerik.WinControls.UI.RadForm
    {
        public frmShowText(string response)
        {
            InitializeComponent();
            txtResponse.Text = response;
            txtResponse.SelectedText = string.Empty;
        }

        private void txtResponse_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //If user pressed ctrl+F to find text.
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
                {
                    FindDig fDig = new FindDig();
                    fDig.SetTextBox(txtResponse);
                    fDig.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
