using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AppedoLT.Forms
{

    public partial class dropdownrnd : Form
    {

        public dropdownrnd()
        {
            InitializeComponent();
        }
        private void dropdownrnd_Load(object sender, EventArgs e)
        {
           
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string key = ((KeyValuePair<string,string>)comboBox1.SelectedItem).Key;
            string value = ((KeyValuePair<string,string>)comboBox1.SelectedItem).Value;
            MessageBox.Show(key + "   " + value);
        }
    }
}
