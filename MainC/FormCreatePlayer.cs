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
    public partial class FormCreatePlayer : Form
    {
        Image bg;
        Image mess_187x92;

        public FormCreatePlayer()
        {
            InitializeComponent();
        }

        private void CreatePlayer_Load(object sender, EventArgs e)
        {
            string root = GlobalB.GetRootPath();
            bg = Bitmap.FromFile(root + @"\Medias\Loading_Bk.png");
            mess_187x92 = Bitmap.FromFile(root + @"\Medias\Logon_Register.png");

            int w = myTextBox1.Width;
            int h = myTextBox1.Height;
            int x = myTextBox1.Location.X;
            int y = myTextBox1.Location.Y;
            //创建新图位图   
            Bitmap bitmap = new Bitmap(w,h);
            //创建作图区域   
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区   
            graphic.DrawImage(bg, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
            //从作图区生成新图   
            myTextBox1.BackImage = Image.FromHbitmap(bitmap.GetHbitmap());
        }

        private void FormCreatePlayer_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawImage(bg, new Rectangle(0, 0, 800, 600), new Rectangle(0, 0, 800, 600), GraphicsUnit.Pixel);
            g.DrawImage(mess_187x92, new Rectangle((800-187)/2, (600-92)/2, 187, 92), new Rectangle(0, 0, 187, 92), GraphicsUnit.Pixel);
        }
    }
}
