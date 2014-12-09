using System;
using System.Windows.Forms;
using AppedoLT.Core;

namespace AppedoLT
{
    public partial class FindDig : Telerik.WinControls.UI.RadForm
    {
        RichTextBox receivedRichTextBox = new RichTextBox();
        int foundIndex = 0;
        public FindDig()
        {
            InitializeComponent();
           

        }
        public void SetTextBox(RichTextBox txtText)
        {
            try
            {
                foundIndex = 0;
                receivedRichTextBox = txtText;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (foundIndex < 0)
                {
                    foundIndex = receivedRichTextBox.Find(txtFindText.Text, 0, RichTextBoxFinds.None);
                }
                else
                {
                    if (foundIndex + 1 <= receivedRichTextBox.Text.Length)
                        foundIndex = receivedRichTextBox.Find(txtFindText.Text, foundIndex + 1, RichTextBoxFinds.None);
                    else
                        foundIndex = -1;
                }
                if (foundIndex > -1)
                {
                    receivedRichTextBox.Select(foundIndex, txtFindText.Text.Length);
                    receivedRichTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
