using Data.Globals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var xmlManager = Global.GetXmlManager();
            xmlManager.Load();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var formCreatePlayer = new FormCreatePlayer();
            formCreatePlayer.Show();
        }
    }
}
