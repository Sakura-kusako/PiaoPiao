using Data.Globals;
using Room;
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

        }

        private void FormCreatePlayer_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawImage(bg, new Rectangle(0, 0, 800, 600), new Rectangle(0, 0, 800, 600), GraphicsUnit.Pixel);
            g.DrawImage(mess_187x92, new Rectangle((800-187)/2, (600-92)/2, 187, 92), new Rectangle(0, 0, 187, 92), GraphicsUnit.Pixel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string root = GlobalB.GetRootPath()+ @"\Setting\";

            Random rand = new Random();
            var time = DateTime.Now;
            string year = time.Year + "";
            string month = GlobalB.IntToString(time.Month, 1);
            string day = GlobalB.IntToString(time.Day, 1);
            string hour = GlobalB.IntToString(time.Hour, 1);
            string min = GlobalB.IntToString(time.Minute, 1);
            string sec = GlobalB.IntToString(time.Second, 1);
            string msec = GlobalB.IntToString(time.Millisecond, 2);
            string ran = GlobalB.IntToString((rand.Next() % 1000), 3);

            var player = new PlayerData();
            player.name = textBox2.Text;
            player.QQ = textBox1.Text;
            player.ExID = "01" + year + month + day + hour + min + sec + msec + ran;

            Data.XML.XmlPlayer.WritePlayer(root , player);
            this.Close();
        }
    }
}
