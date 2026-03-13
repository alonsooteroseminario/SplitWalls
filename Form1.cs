using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SplitWalls.Models;
using SplitWalls.Services;

namespace SplitWalls
{
    public partial class Form1 : Form
    {
        public PanelOptions Options { get; private set; }
        public WallProfileConfig LoadedProfile { get; private set; }

        private System.Windows.Forms.Label _lblProfileName;
        private string textString;
        private bool checkBox_1;
        private bool checkBox_2;
        private bool checkBox_3;
        private bool checkBox_4;
        private bool checkBox_5;
        private bool checkBox_6;
        private bool checkBox_7;


        public Form1()
        {
            InitializeComponent();
            AddProfileControls();
        }

        private void AddProfileControls()
        {
            var btn = new System.Windows.Forms.Button
            {
                Name = "btnLoadProfile",
                Text = "Load Profile",
                Width = 100,
                Height = 25,
                Location = new System.Drawing.Point(10, this.ClientSize.Height - 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btn.Click += btnLoadProfile_Click;

            _lblProfileName = new System.Windows.Forms.Label
            {
                Name = "lblProfileName",
                Text = "(no profile loaded)",
                Width = 220,
                Height = 20,
                Location = new System.Drawing.Point(115, this.ClientSize.Height - 31),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            this.Controls.Add(btn);
            this.Controls.Add(_lblProfileName);
        }

        private void btnLoadProfile_Click(object sender, EventArgs e)
        {
            var service = new ProfileFileService();
            var dlg = new OpenFileDialog
            {
                Title = "Load Wall Profile",
                Filter = "Profile Files (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = service.GetDefaultFolder()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadedProfile = service.Load(dlg.FileName);
                    _lblProfileName.Text = LoadedProfile.Name;
                    if (LoadedProfile.Defaults != null)
                        textBox1.Text = LoadedProfile.Defaults.PanelWidthMm.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading profile: " + ex.Message,
                        "SplitWalls", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textString = textBox1.Text;
            if (checkBox_1 && textString != "1220")
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
            else if (textString == "" && !checkBox_1)
            {
                MessageBox.Show("Por Favor elige las opciones correctas", "No has ingresado los Valores correctos!");
            }

            Options = new PanelOptions
            {
                AnchoPanel                = textString,
                MuroSinVentanas           = checkBox_2,
                MuroOsbConVentanas        = checkBox_3,
                MuroSmartPanelConVentanas = checkBox_6,
                TodoMuro                  = checkBox_7,
                Esquina1                  = checkBox_4,
                Esquina2OtroLado          = checkBox_5,
            };
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
        }

    }
}
