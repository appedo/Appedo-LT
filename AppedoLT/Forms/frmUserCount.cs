using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmUserCount : Form
    {

        public frmUserCount()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            txtResult.Text = Constants.GetInstance().Encrypt(txtInput.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txtResult.Text = Constants.GetInstance().Decrypt(txtInput.Text);
        }
    }
}
