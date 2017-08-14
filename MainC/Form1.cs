using Data.Globals;
using Room;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainC
{
    public partial class Form1 : Form
    {
        List<PlayerData> players = new List<PlayerData>();
        public FormGame formGame;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var xmlManager = Global.GetXmlManager();
            xmlManager.Load();
            Form1_Load_Player();
        }
        private void Form1_Load_Player()
        {
            var rootPath = GlobalB.GetRootPath() + @"\Setting\PlayerData";
            if (Directory.Exists(rootPath) == false) return;
            players.Clear();

            DirectoryInfo dirs = new DirectoryInfo(rootPath);
            foreach (var dir in dirs.GetDirectories())
            {
                var player = new PlayerData();
                bool poi = Data.XML.XmlPlayer.ReadPlayer(dir.FullName, player);
                if(poi == true)
                {
                    players.Add(player);
                    comboBox1.Items.Add(player.GetQQ() + " - " + player.GetName());
                }
            }
        }
        private void Save()
        {
            //保存所有数据
            SavePlayer();
        }
        private void SavePlayer()
        {
            //保存玩家数据
            string path = GlobalB.GetRootPath() + @"\Setting\";
            foreach (var player in players)
            {
                Data.XML.XmlPlayer.WritePlayer(path, player);
            }
        }            

        private void button1_Click(object sender, EventArgs e)
        {
            var formCreatePlayer = new FormCreatePlayer();
            formCreatePlayer.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(formGame == null || formGame.IsDisposed)
            {
                formGame = new FormGame(this);
                formGame.Show();
            }
            else
            {
                AddTextLine(textBox6, "游戏已启动");
            }
        }
        private void AddTextLine(TextBox tx,string str)
        {
            tx.AppendText(str + System.Environment.NewLine);
        }
        public void AddTextGame(string str)
        {
            AddTextLine(textBox5, str);
        }
        public void ShowText7(string str)
        {
            textBox7.Text = str;
        }
        public void ShowText8(string str)
        {
            textBox8.Text = str;
        }
        public void ShowText9(string str)
        {
            textBox9.Text = str;
        }
        public void ShowText10(string str)
        {
            textBox10.Text = str;
        }
        public void ShowText11(string str)
        {
            textBox11.Text = str;
        }
    }
}
