using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.IO;
using AppedoLT.Core;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;

namespace AppedoLT
{
    public partial class RadForm1 : Telerik.WinControls.UI.RadForm
    {
        Constants constants = Constants.GetInstance();
        public RadForm1()
        {
            InitializeComponent();
          
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            MessageBox.Show("completed");

        }
    }
}
