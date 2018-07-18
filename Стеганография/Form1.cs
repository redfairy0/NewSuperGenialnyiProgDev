using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Encoder newForm = new Encoder();
            newForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Decoder newForm = new Decoder();
            newForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
