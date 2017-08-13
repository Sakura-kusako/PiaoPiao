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
    public partial class FormGame : Form
    {
        Form1 FormParent;

        public FormGame(Form1 FormParent)
        {
            InitializeComponent();
            this.FormParent = FormParent;
        }

        private void FormGame_Load(object sender, EventArgs e)
        {

        }
    }
}
