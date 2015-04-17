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
    public partial class IntTextBox : TextBox
    {
        public IntTextBox()
        {
            InitializeComponent();
            this.Size = new Size(35, 20);
            this.TextAlign = HorizontalAlignment.Right;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.Paint += new PaintEventHandler(UserControl1_Paint);
            this.Text = "0";
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void IntTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void IntTextBox_Leave(object sender, EventArgs e)
        {
            if (this.Text == string.Empty) this.Text = "0";
        }

        private void IntTextBox_Enter(object sender, EventArgs e)
        {
            if (this.Text == "0") this.Text = string.Empty;
        }
        private void UserControl1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }
    }
}
