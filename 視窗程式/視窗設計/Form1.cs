using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 視窗設計
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "123" && textBox2.Text == "123")
            {
                Form2 f2 = new Form2();
                f2.Owner = this;
                this.Hide(); 		/* 或 this.close(); */
                f2.Show();
            }
            else
                MessageBox.Show("Wrong Password");
        }
    }
}
