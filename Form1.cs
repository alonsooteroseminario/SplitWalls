using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplitWalls
{
    public partial class Form1 : Form
    {
        public string textString { set; get; }

        public bool checkBox_1 { get; set; }

        public bool checkBox_2 { get; set; }

        public bool checkBox_3 { get; set; }

        public bool checkBox_4 { get; set; }

        public bool checkBox_5 { get; set; }

        public bool checkBox_6 { get; set; }

        public bool checkBox_7 { get; set; }

        //public bool checkBox_8 { get; set; }


        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textString = "1220";
  
            if (!(textString == "1220"))
            {
                checkBox1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;
            textString = textBox1.Text;
            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) // Muro sin Ventanas
        {
            checkBox_1 = checkBox1.Checked;

            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }


            checkBox_2 = checkBox2.Checked;
            textString = textBox1.Text;
            
            if (checkBox_2)
            {
                checkBox3.Checked = false;
                checkBox6.Checked = false;
                checkBox7.Checked = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;

            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }


            checkBox_3 = checkBox3.Checked;
            textString = textBox1.Text;
            if (checkBox_3)
            {
                checkBox2.Checked = false;
                checkBox6.Checked = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;

            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }

            checkBox_4 = checkBox4.Checked;
            textString = textBox1.Text;
            if (checkBox_4)
            {
                checkBox5.Checked = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;

            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }

            checkBox_5 = checkBox5.Checked;
            textString = textBox1.Text;
            if (checkBox_5)
            {
                checkBox4.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox_2 && !checkBox_3 && !checkBox_6)
            {
                MessageBox.Show("Por Favor elige las opciones correctas", "No has ingresado los Valores correctos!");
            }
            else if (!checkBox_4 && !checkBox_5)
            {
                MessageBox.Show("Por Favor elige las opciones correctas", "No has ingresado los Valores correctos!");
            }
            else if (textString =="" && !checkBox_1)
            {
                MessageBox.Show("Por Favor elige las opciones correctas", "No has ingresado los Valores correctos!");
            }
            //else if (!checkBox_7 && !checkBox_8)
            //{
            //    MessageBox.Show("Por Favor elige las opciones correctas", "No has ingresado los Valores correctos!");
            //}
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;



            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }


            checkBox_6 = checkBox6.Checked;
            textString = textBox1.Text;
            if (checkBox_6)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_1 = checkBox1.Checked;

            if (checkBox_1)
            {
                textBox1.Text = "1220";
            }


            checkBox_7 = checkBox7.Checked;
            textString = textBox1.Text;
            //if (checkBox_7)
            //{
            //    checkBox8.Checked = false;
            //}
        }

        //private void checkBox8_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkBox_1 = checkBox1.Checked;

        //    if (checkBox_1)
        //    {
        //        textBox1.Text = "1220";
        //    }


        //    checkBox_8 = checkBox8.Checked;
        //    textString = textBox1.Text;
        //    if (checkBox_8)
        //    {
        //        checkBox7.Checked = false;
        //    }
        //}
    }
}
