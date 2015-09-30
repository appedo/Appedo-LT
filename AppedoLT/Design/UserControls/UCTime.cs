using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppedoLT
{

    /// <summary>
    /// User control used to show time information.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class UCTime : UserControl
    {
        public TimeSpan Time
        {
            get
            {
                return new TimeSpan(int.Parse(txtHours.Text), int.Parse(txtMinutes.Text), int.Parse(txtSeconds.Text));
            }
            set
            {

                txtHours.Text = value.Hours.ToString();
                txtMinutes.Text = value.Minutes.ToString();
                txtSeconds.Text = value.Seconds.ToString();
            }
        }
        public UCTime()
        {
            InitializeComponent();
            
        }

        private void UCTime_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == false)
            {
                txtHours.Enabled = txtMinutes.Enabled = txtSeconds.Enabled = false;
            }
            else
            {
                txtHours.Enabled = txtMinutes.Enabled = txtSeconds.Enabled = true;
            }
        }
    }
}
