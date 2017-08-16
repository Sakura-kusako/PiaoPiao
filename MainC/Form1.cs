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
            if(Global.GetClientC() == null)
            {
                Global.SetClientC(new ClientPublic.ClientC());
            }
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
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
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
            if(comboBox1.Items.Count == 0)
            {
                AddTextLine(textBox6, "当前没有玩家数据，需要创建一个玩家");
                return;
            }
            if(comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= players.Count)
            {
                comboBox1.SelectedIndex = 0;
                AddTextLine(textBox6, "Error : 超出引索，按第一个玩家启动");
            }

            //复制数据
            var player = Global.GetPlayer();
            player.Clone(players[comboBox1.SelectedIndex]);
            AddTextLine(textBox6, "玩家加载成功");

            if ((formGame == null || formGame.IsDisposed) && Global.IsFormGameOpen == false)
            {
                AddTextLine(textBox6, "飘飘正在启动，可能需要几十秒的时间");
                formGame = new FormGame(this);
                formGame.Show();
                AddTextLine(textBox6, "飘飘启动成功");
            }
            else
            {
                AddTextLine(textBox6, "飘飘已启动");
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
        public void ShowText12(string str)
        {
            textBox12.Text = str;
        }
        public void ShowText13(string str)
        {
            textBox13.Text = str;
        }
        public void ShowText14(string str)
        {
            textBox14.Text = str;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.IsFormGameOpen)
                {
                    string port = textBox4.Text;
                    string ip = textBox3.Text;
                    int ret = 0;
                    byte[] ipbyte = new byte[4];

                    var strs = ip.Split('.');
                    if (strs.Length != 4)
                    {
                        AddTextLine(textBox6, "IP地址格式有误");
                        return;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        bool checks = byte.TryParse(strs[i],out ipbyte[i]);
                        if(checks == false)
                        {
                            AddTextLine(textBox6, "IP地址格式有误");
                            return;
                        }
                    }

                    bool check =  int.TryParse(port, out ret);
                    if(check == false || ret>65535)
                    {
                        AddTextLine(textBox6, "端口格式有误");
                        return;
                    }

                    var clientC = Global.GetClientC();
                    if (clientC == null || true)
                    {
                        clientC = new ClientPublic.ClientC(Global.GetPlayer().GetQQ(), Global.GetPlayer().GetExID());
                        Global.SetClientC(clientC);
                    }
                    if (clientC.IsConnect() == false)
                    {
                        {
                            clientC.IP = new System.Net.IPAddress(ipbyte);
                            clientC.Port = ret;
                        }

                        clientC.OpenClient();
                        clientC.AddData(Global.GetPlayer().GetSendData_All());
                        AddTextLine(textBox6, "尝试连接到 " + clientC.IP.ToString() + ":" + clientC.Port);
                    }
                    else
                    {
                        AddTextLine(textBox6, "连接已经打开");
                    }
                }
                else
                {
                    AddTextLine(textBox6, "飘飘尚未启动");
                }
            }
            catch (Exception ex)
            {
                AddTextLine(textBox6, ex.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (formGame != null && formGame.IsDisposed == false)
                formGame.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool c = checkBox1.Checked;
            Global.IsDebug = c;
        }
    }
}
