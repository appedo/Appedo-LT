using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppedoLT
{
    public partial class frmShowData : Form
    {
        public frmShowData(string msg,object data)
        {
            InitializeComponent();
            this.Text = msg;
            this.dataGridView1.DataSource = data;
        }
        
    }
}
